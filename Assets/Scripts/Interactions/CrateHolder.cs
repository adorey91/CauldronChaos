using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateHolder : Interactable
{
    public GameObject ingredientPrefab;
    public enum CrateType { Bottle, Mushroom, RabbitFoot, EyeOfBasilisk, Mandrake, TrollBone };
    public CrateType crateType;


    public void Start()
    {
        if (ingredientPrefab == null)
        {
            switch (crateType)
            {
                case CrateType.Bottle: ingredientPrefab = LoadPrefab("Assets/Prefabs/Ingredient_Prefabs/Bottle_Prefab.prefab"); break;
                case CrateType.Mushroom: ingredientPrefab = LoadPrefab("Assets/Prefabs/Ingredient_Prefabs/Mushroom.prefab"); break;
                case CrateType.RabbitFoot: ingredientPrefab = LoadPrefab("Assets/Prefabs/Ingredient_Prefabs/Rabbit_Foot_Prefab.prefab"); break;
                case CrateType.EyeOfBasilisk: ingredientPrefab = LoadPrefab("Assets/Prefabs/Ingredient_Prefabs/Eye_of_Basilisk_Prefab.prefab"); break;
                case CrateType.Mandrake: ingredientPrefab = LoadPrefab("Assets/Prefabs/Ingredient_Prefabs/Mandrake.prefab"); break;
                case CrateType.TrollBone: ingredientPrefab = LoadPrefab("Assets/Prefabs/Ingredient_Prefabs/Troll_Bone.prefab"); break;
            }
        }
    }

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
        if (ingredientPrefab == null)
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

        Vector3 randomPosition = new(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));

        ingredient.transform.DOScale(new Vector3(1f, 1f, 1f), 1f); //DOTween animation for scaling the ingredient
        ingredient.transform.DOJump(goblin.position + randomPosition, 1, 1, 1); //DOTween animation for jumping the ingredient
    }

    private GameObject LoadPrefab(string path)
    {
        GameObject prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError("Prefab not found at path: " + path);
            return null;
        }
        return prefab;
    }
}
