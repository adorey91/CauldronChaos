using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderManagerUi : MonoBehaviour
{
    [SerializeField] private GameObject orderUiHolder;
    [SerializeField] private GameObject orderUiPrefab;

    internal GameObject orderUi;

    internal void GenerateOrderUI(RecipeSO assignedOrder)
    {
        orderUi = Instantiate(orderUiPrefab, orderUiHolder.transform);
        orderUi.transform.GetChild(0).GetComponent<Image>().sprite = assignedOrder.potionIcon;
    }

    internal GameObject GetOrderUI() => orderUi;

    internal void RemoveOrderUI(GameObject orderUi)
    {
        Destroy(orderUi);
    }
}
