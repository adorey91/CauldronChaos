using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindyDay : MonoBehaviour
{
    public enum WindDirection
    {
        None,
        GoingLeft,
        GoingRight,
        TowardsScreen,
    }
    public WindDirection windDirect;

    public float strength;
    public Vector3 direction;



    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    public void OnDrawGizmos()
    {
        Color gizmoColor;
        switch(windDirect)
        {
            case WindDirection.GoingLeft:  gizmoColor = Color.blue; break;
            case WindDirection.GoingRight: gizmoColor = Color.red; break;
            case WindDirection.TowardsScreen: gizmoColor = Color.green; break;
            default: gizmoColor = Color.cyan; break;
        }

        Gizmos.color = gizmoColor;
        Gizmos.DrawRay(transform.position, transform.forward * 5f);
    }
}
