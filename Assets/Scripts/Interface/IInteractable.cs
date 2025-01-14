using UnityEngine;

public interface IInteractable
{
    public void Interact(InteractionDetector player);
    public bool CanBeInteractedWith(InteractionDetector player);
}
