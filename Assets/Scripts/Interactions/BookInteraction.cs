using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookInteraction : MonoBehaviour, IInteractable
{
    public void Interact(InteractionDetector player)
    {

        Actions.OnToggleRecipeBook?.Invoke();
    }

    public bool CanBeInteractedWith(InteractionDetector player)
    {
        return true;
    }
}
