using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public static class Actions
{
    // Interaction Actions
    #region Interaction Actions
    public static Action OnToggleRecipeBook;
    public static Action OnRemovePotion;
    public static Action OnRemoveIngredient;
    #endregion

    // Shop Actions
    #region Shop Actions
    public static Action OnStartDay;
    public static Action OnEndDay;
    public static Action<bool, int> OnCustomerServed;
    public static Action OnNoCustomerServed;
    public static Action FilledOrder;
    public static Action<PotionOutput> OnCheckCustomers;
    #endregion

    // Day Actions
    #region Day Actions
    public static Action<int> OnSetDay;
    public static Action<string> OnDayText; // used for day countdown panel. explaination of the day.
    public static Action OnStartDayCountdown;
    #endregion

    // Game Manager Actions
    #region Game Manager Actions
    public static Action<string> OnForceStateChange;
    public static Action<GameManager.GameState> OnStateChange; // used for UI changes
    #endregion

    // Gameplay Actions
    #region Gameplay Actions
    public static Action UpdateLevelButtons;
    public static Action<int> OnSetUnlockedDays;
    public static Action<int[]> OnSetScore;
    public static Action OnResetValues;
    #endregion

    // Save Manager Actions
    #region Save Manager Actions
    public static Action<bool> OnSaveExist;
    public static Action<int, int, bool> OnSaveDay;
    public static Action OnSaveDeleted;
    public static Action OnDeleteSaveFile;
    #endregion

    // Actions for First Select
    #region First Select Actions
    public static Action OnFirstSelect;
    public static Action OnRemoveSelection;
    public static Action<GameObject> OnSelectRecipeButton;
    public static Action<UiObject.Page> OnSetUiLocation;
    #endregion

    // Actions for Menu
    #region Menu Actions
    public static Action OnOpenSettingsAction;
    public static Action<bool> SetCursorVisibility;
    public static Action<bool> OnActivateHowToPlay;
    public static Action OnDeactivateHowToPlay;
    public static Action<string> ReachedWaypoint;
    public static Action OnCloseDebugMenu;
    #endregion

    // Challenge Actions
    #region Challenge Actions
    public static Action<int> OnStartChallenge; // used for ChallengeManager.cs
    public static Action OnResetChallenge;
    public static Action<PhysicMaterial, Texture> OnApplyFoorMaterial; // used for Challenge 1 - Floor.cs
    public static Action<bool> OnIceDay; // used for Challenge 1 - Floor.cs
    public static Action OnStartCauldron; // used for Challenge 2 - CauldronMovement.cs
    public static Action OnEndCauldron; // used for Challenge 2 - CauldronMovement.cs
    public static Action<bool> OnStartGoblin; // used for Challenge 3 - GoblinAI.cs - set if it's the first challenge day or not.
    public static Action OnEndGoblin; // used for Challenge 3 - GoblinAI.cs
    public static Action OnScareGoblin; // used for Challenge 3 - GoblinAI.cs
    public static Action OnStartWindy; // used for Challenge 4 - WindyDay.cs
    public static Action OnStopWindy; // used for Challenge 4 - WindyDay.cs
    public static Action OnStartSlime;  // used for Challenge 5 - SlimeTrail.cs
    public static Action OnEndSlime; // used for Challenge 5 - SlimeTrail.cs
    #endregion
}
