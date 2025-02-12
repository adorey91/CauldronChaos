using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : Interactable
{
   public GameObject ingredientPrefab;

    //Unimplemented regular interact function
    public override void Interact()
    {
        throw new System.NotImplementedException();
    }

    //Function that holds interact functionality for the ingedient crate
    public override void Interact(PickupBehaviour playerPickup)
    {
        //Debug.Log("Create Interact called");

        //Exit function  if no ingredient is selected
        if(ingredientPrefab == null)
        {
            Debug.LogError("No ingredient prefab assigned to " + gameObject.name);
            return;
        }

        GameObject newIngredient;
        newIngredient = Instantiate(ingredientPrefab, playerPickup.GetHolderLocation()); //spawning new ingredient
        playerPickup.SetHeldObject(newIngredient.GetComponent<PickupObject>()); //adding manually to player's held slot
    }

    internal void GoblinInteraction(Transform goblin)
    {
        GameObject ingredient;
        ingredient = Instantiate(ingredientPrefab, goblin.position, Quaternion.identity);
        ingredient.transform.DOScale(new Vector3(1f, 1f, 1f), 1f); //DOTween animation for scaling the ingredient
        ingredient.transform.DOJump(goblin.position + new Vector3(0, 1, 0), 1, 1, 1); //DOTween animation for jumping the ingredient
    }
}
