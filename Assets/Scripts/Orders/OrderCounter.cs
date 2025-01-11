using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCounter : MonoBehaviour, IInteractable
{
    private OrderManager _orderManager;


    private void Start()
    {
        _orderManager = FindObjectOfType<OrderManager>();
    }


    public void Interact(InteractionDetector player)
    {
        if (!player.HasPotion()) return;

        PotionOutput output = player.GetPotion().GetComponent<PotionOutput>();

        _orderManager.FinishOrder(output);
        Actions.OnRemovePotion?.Invoke();
    }
}
