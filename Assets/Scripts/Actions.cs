using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public static class Actions
{
    // Interaction Actions
    public static Action OnToggleRecipeBook;
    public static Action OnRemovePotion;
    public static Action OnRemoveIngredient;

    // Shop Actions
    public static Action OnStartDay;
    public static Action OnEndDay;
    public static Action<bool, int> OnCustomerServed;
    public static Action OnNoCustomerServed;

    // Menu Actions
    public static Action<string> OnForceStateChange;
    public static Action<GameManager.GameState> OnStateChange; // used for UI changes
    public static Action<string> OnFirstSelect;

}
