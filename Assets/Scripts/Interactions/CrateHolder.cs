using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : MonoBehaviour, IInteractable
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

    public void Interact(InteractionDetector player)
    {
        if (player.HasPotion()) return;
        if(player.GetRecipeStep()) return;

        if(ingredientPrefab == null)
        {
            Debug.LogError("No ingredient prefab assigned to " + gameObject.name);
            return;
        }

        GameObject newIngredient;
        newIngredient = Instantiate(ingredientPrefab);

        player.PickUpIngredient(newIngredient);
    }
}
