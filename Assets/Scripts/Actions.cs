using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public static class Actions
{
    // Interaction Actions
    public static Action OnInteract;
    public static Action OnPickup;
    public static Action OnToggleRecipeBook;
    public static Action OnRemovePotion;
    public static Action OnRemoveIngredient;

    // Shop Actions
    public static Action OnStartDay;
    public static Action OnEndDay;
    public static Action<bool> OnCustomerServed;
    public static Action OnNoCustomerServed;

    // Menu Actions
    public static Action OnPause;

}
