using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : MonoBehaviour, IPickupable
{
    public enum IngredientObject { Mushroom, EyeOfBasilisk, MandrakeRoot, RabbitFoot, TrollBone }
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


    public bool AlreadyActive()
    {
        return false;
    }

    public void Drop(Transform newParent)
    {
        return;
    }
    
    public void Pickup(InteractionDetector player)
    {
        if (player.HasIngredient() || player.HasPotion()) return;

        player.AddIngredient(ingredientPrefab);
    }
}
