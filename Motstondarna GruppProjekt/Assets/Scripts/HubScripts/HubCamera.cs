using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubCamera : MonoBehaviour
{
    public Transform targetPoint;


    [SerializeField] Vector3 offsett;
    [SerializeField] float smoothSpeed;

    
    void FixedUpdate()
    {
        Vector3 _targetPos = targetPoint.position + offsett;

        transform.position = Vector3.Lerp(transform.position, _targetPos, smoothSpeed);
    }
}
