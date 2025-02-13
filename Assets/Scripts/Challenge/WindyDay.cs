using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindyDay : MonoBehaviour
{
    [SerializeField] private GameObject windTowardsScreen;
    [SerializeField] private GameObject windGoesLeft;
    [SerializeField] private GameObject windGoesRight;


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(windTowardsScreen.transform.position, windTowardsScreen.transform.forward * 5f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(windGoesLeft.transform.position, windGoesLeft.transform.right * 5f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(windGoesRight.transform.position, windGoesRight.transform.right * 5f);
    }
}
