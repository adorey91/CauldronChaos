using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : Interactable
{
    public enum IngredientObject { Mushroom, EyeOfBasilisk, MandrakeRoot, RabbitFoot, TrollBone, Bottle }
    [SerializeField] private IngredientObject ingredient;

    [Header("Crate Topper Objects")]
    public GameObject crateTopMushroom;
    public GameObject crateTopEyeOfBasilisk;
    public GameObject crateTopMandrakeRoot;
    public GameObject crateTopRabbitFoot;
    public GameObject crateTopTrollBone;

   public GameObject ingredientPrefab;

    private void ChangeTop()
    {
        // 
    }

    //Unimplemented regular interact function
    public override void Interact()
    {
        throw new System.NotImplementedException();
    }

    //Function that holds interact functionality for the ingedient crate
    public override void Interact(PickupBehaviour playerPickup)
    {
        //Exit function  if no ingredient is selected
        if(ingredientPrefab == null)
        {
            Debug.LogError("No ingredient prefab assigned to " + gameObject.name);
            return;
        }

        GameObject newIngredient;
        newIngredient = Instantiate(ingredientPrefab); //spawning new ingredient
        playerPickup.SetHeldObject(newIngredient.GetComponent<PickupObject>()); //adding manually to player's held slot
    }
}
