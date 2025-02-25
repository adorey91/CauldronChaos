using UnityEngine;
using static WindyDay;

public class PickupObject : MonoBehaviour
{
    [Header("Ingredient")]
    public RecipeStepSO recipeIngredient;
    public Color ingredientColor;

    [Header("Pick Up")]
    public bool isHeld = false; //bool tracking if the pickup is held
    private Transform targetPos = null; //transform tarcking the target position of the pickup
    private Rigidbody rb; //rigidbody component of the pickup
    private bool addedToCauldron = false;

    [Header("SFX")]
    [SerializeField] private AudioClip pickUpSFX;
    [SerializeField] private AudioClip dropSFX;

    // Windy Day Variables
    private bool inWindZone = false;
    private Vector3 windDirection;
    private WindyDay windArea;
    private WindyDay.WindDirection windDir;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        //moves pickup towards the target position if the positions do not match
        if (isHeld && rb.position != targetPos.position)
        {
            rb.MovePosition(targetPos.position);
        }
    }

    private void FixedUpdate()
    {
        if (inWindZone && !addedToCauldron && !isHeld)
        {
            rb.AddForce(windDirection * windArea.strength);
        }
    }

    //Function that picks up the pickup
    public void PickUp(Transform targetPos)
    {
        //setting held to true and removing gravity
        isHeld = true;
        rb.isKinematic = true;
        //rb.useGravity = false;

        //setting target position & parenting
        this.targetPos = targetPos;
        transform.position = targetPos.position;
        transform.parent = targetPos;

        //playing SFX
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ItemInteraction, pickUpSFX, true);
    }

    //Function that drops the pickup
    public void Drop()
    {
        //settomg held to false and enabling gravity
        isHeld = false;
        rb.isKinematic = false;
        //rb.useGravity = true;

        //removing target position & parent
        targetPos = null;
        transform.parent = null;

        //playing SFX
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.ItemInteraction, dropSFX, true);
    }

    public bool AddedToCauldron()
    {
        return addedToCauldron;
    }

    //Mutator method that marks the ingredient as added to the cauldron
    public void AddToCauldron()
    {
        addedToCauldron = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WindArea"))
        {
            inWindZone = true;
            windArea = other.GetComponent<WindyDay>();
            windDir = windArea.windDirect;
            windDirection = windArea.direction;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(windArea != null)
        {
            if (windArea.windDirect != windDir)
            {
                windDirection = windArea.direction;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WindArea"))
        {
            inWindZone = false;
            windArea = null;
        }
    }

}
