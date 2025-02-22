using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraMenuCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch(other.tag)
        {
            case "Door":
                Actions.ReachedWaypoint?.Invoke("Door");
                break;
            case "Calendar":
                Actions.ReachedWaypoint?.Invoke("Calendar");
                break;
        }
    }
}
