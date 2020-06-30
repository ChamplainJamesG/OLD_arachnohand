using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class CameraFollow : MonoBehaviour
{
    public float sensitivity;
    public Transform target;
    public float distFromTarget;

    float yaw;
    float pitch;

    void Start()
    {
        
    }

    void Update()
    {
        yaw += InputManager.ActiveDevice.RightStick.Vector.x * sensitivity;
        pitch -= InputManager.ActiveDevice.RightStick.Vector.y * sensitivity;

        transform.eulerAngles = new Vector3(pitch, yaw);

        transform.position = target.position - (transform.forward * distFromTarget); //+ new Vector3(0,10,0) ;
    }
}
