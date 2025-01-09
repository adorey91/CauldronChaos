using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : MonoBehaviour
{
    [SerializeField] private GameObject ingredientPrefab;

    public GameObject PickUp()
    {
        return ingredientPrefab;
    }
}
