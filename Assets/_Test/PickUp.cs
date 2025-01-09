using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField] private CrateHolder mushroomHolder;
    [SerializeField] private CrateHolder bottleHolder;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            Instantiate(mushroomHolder.PickUp(), mushroomHolder.transform.position + (mushroomHolder.transform.up * 0.75f), Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Instantiate(bottleHolder.PickUp(), bottleHolder.transform.position + (bottleHolder.transform.up * 0.75f), Quaternion.identity);
        }
    }
}
