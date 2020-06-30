using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    public enum Finger
    {
        middle = 0,
        ring,
        index,
        pinky,
        thumb
    }

    public IKSolver2[] IK;
    public Transform[] raycasters;
    public float speed;
    public float cameraSensitivity;

    bool moveLegs;
    bool grabbing;

    Transform cam;
    Transform grabbedObject;
    Collider groundCollider;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;
        groundCollider = GameObject.Find("Ground").GetComponent<Collider>();
        moveLegs = false;
        grabbing = false;
    }

    void Update()
    {      
        CheckController();
        Vector3 zeroX = transform.eulerAngles;
        zeroX.x = 0;
        transform.eulerAngles = zeroX;
    }

    IEnumerator MoveLegs()
    {
        yield return new WaitForSeconds(0.25f);
        int i = 0;
        while(moveLegs)
        {
            if (!InputManager.ActiveDevice.LeftStick)
                break;
            yield return new WaitForSeconds(IK[i].timeOffset / InputManager.ActiveDevice.LeftStick.Vector.magnitude);
            StartCoroutine(IK[i].MoveFinger());

            i++;
            if (i >= IK.Length)
                i = 0;
        }
    }

    void CheckController()
    {
        if (InputManager.ActiveDevice.LeftStick)
            MoveCharacter();
        else
            StopCharacter();

        if (InputManager.ActiveDevice.RightTrigger)
        {
            ReadyGrab();

            if (InputManager.ActiveDevice.LeftTrigger)
                PerformGrab();
        }
        else
            UnReadyGrab();

        if (InputManager.ActiveDevice.LeftStickButton)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void MoveCharacter()
    {
        if (!moveLegs)
        {
            moveLegs = true;
            StartCoroutine(MoveLegs());
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x,
        Mathf.LerpAngle(transform.eulerAngles.y, InputManager.ActiveDevice.LeftStick.Angle + cam.eulerAngles.y, Time.deltaTime * cameraSensitivity * InputManager.ActiveDevice.LeftStick.Vector.magnitude),
        transform.eulerAngles.z);

        rb.velocity = new Vector3(transform.forward.x * speed, transform.forward.y * speed, transform.forward.z * speed) * InputManager.ActiveDevice.LeftStick.Vector.magnitude;
    }

    void StopCharacter()
    {
        rb.velocity = new Vector3(0, 0, 0);
        moveLegs = false;
    }

    void ReadyGrab()
    {
        if (!grabbing)
        {
            foreach (IKSolver2 i in IK)
                i.BeginGrabbing();

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, -90), Time.deltaTime * 20);
            StartCoroutine(SetGrabbingToTrue());
        }
    }

    void PerformGrab()
    {
        RaycastGrab();
    }

    IEnumerator SetGrabbingToTrue()
    {
        yield return new WaitForSeconds(0.5f);
        grabbing = true;
    }

    void UnReadyGrab()
    {
        if (grabbing)
        {
            StartCoroutine(BeginIKAgain());
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime * 20);

            if (grabbedObject != null)
            {
                grabbedObject.parent = null;
                grabbedObject = null;
            }
        }
    }

    IEnumerator BeginIKAgain()
    {
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < IK.Length; i++)
        {
            IK[i].DoIK();
            IK[i].EndGrabbing();
            RaycastFinger(i);
        }

        grabbing = false;
    }


    public void RaycastFingerUp(int index)
    {
        Transform transformUsed = raycasters[index];

        RaycastHit hit;
        Physics.Raycast(transformUsed.position, transformUsed.forward, out hit, 20, ~(1<<2));

        if(index == (int)Finger.thumb)
            IK[index].SetTarget(new Vector3(hit.point.x, hit.point.y + 4 * InputManager.ActiveDevice.LeftStick.Vector.magnitude, hit.point.z) + (transform.forward * 3) + (-transform.right * 3));
        else
            IK[index].SetTarget(new Vector3(hit.point.x, hit.point.y + 4 * InputManager.ActiveDevice.LeftStick.Vector.magnitude, hit.point.z) + (transform.forward * 5));
    }

    public void RaycastFinger(int index)
    {
        Transform transformUsed = raycasters[index];

        RaycastHit hit;
        Physics.Raycast(transformUsed.position, transformUsed.forward, out hit, 20, ~(1 << 2));

        if (index == (int)Finger.thumb)
            IK[index].SetTarget(new Vector3(hit.point.x, hit.point.y, hit.point.z) + (transform.forward * 3) + (-transform.right * 3));
        else
            IK[index].SetTarget(new Vector3(hit.point.x, hit.point.y, hit.point.z) + (transform.forward * 5));
    }

    public void RaycastGrab()
    {
        foreach(IKSolver2 i in IK)
        {
            RaycastHit hit = new RaycastHit();
            int pass = 0;

            do
            {
                if (i.fingerType == Finger.thumb)
                {
                    switch (pass)
                    {
                        case 0:
                            Physics.Raycast(i.transform.position, i.transform.up + i.transform.forward, out hit, 20, ~(1 << 2));
                            break;
                        case 1:
                            Physics.Raycast(i.transform.position, i.transform.up, out hit, 20, ~(1 << 2));
                            break;
                        case 2:
                            Physics.Raycast(i.transform.position, i.transform.forward, out hit, 20, ~(1 << 2));
                            break;
                    }
                }
                else
                {
                    switch (pass)
                    {
                        case 0:
                            Physics.Raycast(i.transform.position, -i.transform.up + -i.transform.forward, out hit, 20, ~(1 << 2));
                            break;
                        case 1:
                            Physics.Raycast(i.transform.position, -i.transform.up, out hit, 20, ~(1 << 2));
                            break;
                        case 2:
                            Physics.Raycast(i.transform.position, -i.transform.forward, out hit, 20, ~(1 << 2));
                            break;
                    }
                }
                pass++;
            } while (hit.collider == null && pass < 3);

            if (hit.collider != null)
            {
                if (!hit.collider.CompareTag("UnGrabbable"))
                {
                    i.SetTarget(hit.point);
                    i.EndGrabbing();
                    grabbedObject = hit.collider.gameObject.transform;
                    grabbedObject.SetParent(transform);
                    //Physics.IgnoreCollision(hit.collider, groundCollider);
                }
            }

            StartCoroutine(StopIkAfterGrab(i));
        }
    }

    IEnumerator StopIkAfterGrab(IKSolver2 i)
    {
        yield return new WaitForSeconds(0.5f);
        i.StopIK();
    }
}
