using DG.Tweening;
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
                case CrateType.Bottle: ingredientPrefab = LoadPrefab("Ingredient_Prefabs/Bottle_Prefab"); break;
                case CrateType.Mushroom: ingredientPrefab = LoadPrefab("Ingredient_Prefabs/Mushroom"); break;
                case CrateType.RabbitFoot: ingredientPrefab = LoadPrefab("Ingredient_Prefabs/Rabbit_Foot_Prefab"); break;
                case CrateType.EyeOfBasilisk: ingredientPrefab = LoadPrefab("Ingredient_Prefabs/Eye_of_Basilisk_Prefab"); break;
                case CrateType.Mandrake: ingredientPrefab = LoadPrefab("Ingredient_Prefabs/Mandrake"); break;
                case CrateType.TrollBone: ingredientPrefab = LoadPrefab("Ingredient_Prefabs/Troll_Bone"); break;
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
        GameObject prefab = Resources.Load<GameObject>(path);

        if (prefab == null)
        {
            Debug.LogError("Prefab not found at path: " + path);
            return null;
        }
        return prefab;
    }
}
