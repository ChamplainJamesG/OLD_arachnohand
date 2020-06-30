using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowLine : MonoBehaviour
{
    private Transform parent;

    void Start()
    {
        parent = transform.parent;
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if(parent != null)
            Debug.DrawLine(transform.position, parent.position, Color.green);
    }
}
