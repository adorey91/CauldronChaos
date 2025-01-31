using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDroppedItems : MonoBehaviour
{

    private void OnEnable()
    {
        Actions.OnEndDay += RemoveAllDroppedItems;
    }

    private void OnDisable()
    {
        Actions.OnEndDay -= RemoveAllDroppedItems;
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ingredient"))
        {
            other.transform.parent = transform;
        }
    }

    private void RemoveAllDroppedItems()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
