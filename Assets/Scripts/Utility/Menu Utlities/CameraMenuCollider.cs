using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMenuCollider : MonoBehaviour
{
    public static Action<string> ReachedWaypoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            ReachedWaypoint?.Invoke("Door");
        }
        if(other.CompareTag("Calendar"))
        {
            ReachedWaypoint?.Invoke("Calendar");
        }
    }

}
