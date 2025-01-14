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

    public static Action OnStirClockwise;
    public static Action OnStirCounterClockwise;

    // Shop Actions
    public static Action OnStartDay;
    public static Action OnEndDay;
    public static Action<bool> OnCustomerServed;
    public static Action OnNoCustomerServed;



    // Recipe Book Actions
    public static Action NextPage;
    public static Action PreviousPage;

    // Menu Actions
    public static Action OnPause;

}
