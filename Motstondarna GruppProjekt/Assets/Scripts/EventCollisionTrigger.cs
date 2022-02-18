using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCollisionTrigger : MonoBehaviour
{

    public UnityEvent unityEvent;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            unityEvent.Invoke();
            Destroy(gameObject);
        }
    }
}
