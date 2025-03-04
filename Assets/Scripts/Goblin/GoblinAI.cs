using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinAI : MonoBehaviour
{
    [Header("Goblin Behaviour Settings")]
    [SerializeField] private float actionCooldown = 5f;
    [SerializeField] private Transform goblinHands;
    [SerializeField] private NavMeshAgent agent;
    private bool isPerformingAction = false;
    private bool isScared = false;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minWanderTime = 2f;
    [SerializeField] private float maxWanderTime = 5f;
    [SerializeField] private bool goblinActive = false;
    private Vector3 currentDestination;

    [Header("Time Between Action")]
    [SerializeField] private float throwFromCrate;
    [SerializeField] private float throwFloorIngredient;

    [Header("SFX")]
    [SerializeField] private bool enableSFX;
    [SerializeField] private float noiseTimerMin;
    [SerializeField] private float noiseTimerMax; 
    [SerializeField] private SFXLibrary goblinIdle;
    [SerializeField] private SFXLibrary goblinCry;
    [SerializeField] private SFXLibrary goblinScream;
    [SerializeField] private SFXLibrary goblinSlurp;
    [SerializeField] private SFXLibrary rummageStings;
    [SerializeField] private AudioClip rummageSound;
    [SerializeField] private AudioClip goblinMovement;

    //Idle SFX variables
    private bool isFree = false;
    private float noiseTimer;

    // References to the things the goblin can interact with - the only thing that changes is ingredients.
    private CrateHolder[] crates;
    private List<GameObject> ingredients;
    private CauldronInteraction[] cauldrons;
    private QueueManager queue;

    [Header("Slime Movement")]
    [SerializeField] private LayerMask slime;
    [SerializeField] private float slowMultiplier = 0.5f;
    private bool isInSlime = false;
    private float defaultSpeed;
    public Vector3 slimeDirection;

    // Coroutines to handle the goblin behaviour
    private Coroutine goblinBehaviour;
    private Coroutine currentAction;

    private void Start()
    {
        if(agent != null)
            defaultSpeed = agent.speed;

        noiseTimer = Random.Range(noiseTimerMin, noiseTimerMax);
        crates = FindObjectsOfType<CrateHolder>();
        queue = FindObjectOfType<QueueManager>();
        cauldrons = FindObjectsOfType<CauldronInteraction>();
    }

    //update loop to handle playing of idle sounds
    private void Update()
    {
        if(GameManager.instance.gameState == GameState.Gameplay)
        {
            if (noiseTimer < 0 && enableSFX)
            {
                SFXLibrary goblinSounds;

                if (isFree)
                {
                    goblinSounds = goblinIdle;
                }
                else
                {
                    goblinSounds = goblinCry;
                }

                AudioManager.instance.sfxManager.PlaySFX(SFX_Type.GoblinSounds, goblinSounds.PickAudioClip(), true); //play audio clip
                noiseTimer = Random.Range(noiseTimerMin, noiseTimerMax); //reset timer
            }

            if (enableSFX)
            {
                noiseTimer -= Time.deltaTime;
            }
        }

        //Debug.Log(noiseTimer);
        if(Physics.Raycast(transform.position, Vector3.right, out RaycastHit hit, 1f, slime))
        {
            if (!isInSlime)
            {
                isInSlime = true;
                agent.speed *= slowMultiplier;
            }
        }
        else
        {
            if (isInSlime)
            {
                isInSlime = false;
                agent.speed = defaultSpeed;
            }
        }
    }

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStartGoblin += StartChaos;
        Actions.OnEndGoblin += EndChaos;
        Actions.OnScareGoblin += ScareAway;
    }

    private void OnDisable()
    {
        Actions.OnStartGoblin -= StartChaos;
        Actions.OnEndGoblin -= EndChaos;
        Actions.OnScareGoblin -= ScareAway;
    }

    private void OnDestroy()
    {
        Actions.OnStartGoblin -= StartChaos;
        Actions.OnEndGoblin -= EndChaos;
        Actions.OnScareGoblin -= ScareAway;
    }
    #endregion

    private void StartChaos(bool isChallengeDay)
    {
        goblinActive = true;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        agent.enabled = true;
        goblinBehaviour = StartCoroutine(BehaviourLoop(isChallengeDay));
    }

    private void EndChaos()
    {
        goblinActive = false;
        if (goblinBehaviour != null)
            StopCoroutine(goblinBehaviour);
    }

    private IEnumerator BehaviourLoop(bool isChallengeDay)
    {
        // if its a challenge day goblin will be more active. else they need a wandering time
        currentAction = null;
        while (true)
        {
            yield return new WaitForSeconds(actionCooldown);

            if (isScared) continue;

            float roll = Random.Range(0f, 1f);

            if (currentAction != null)
            {
                StopCoroutine(currentAction);
                AudioManager.instance.sfxManager.StopConstantSFX(); //stop any constant SFX
            }

            if (isChallengeDay)
            {
                PickWhatAction(roll);
            }
            else
            {
                // if roll is less than or equal to this number the goblin will wander - if not, they will pick a new action to do.
                if (roll <= 0.3f)
                    currentAction = StartCoroutine(GoblinWanders());
                else
                {
                    float newRoll = Random.Range(0f, 1f);
                    PickWhatAction(newRoll);
                }
            }

        }
    }

    private void PickWhatAction(float roll)
    {
        switch (roll)
        {
            case float n when n < 0.4f:
                FindFloorItems();
                if (ingredients.Count > 0)
                    currentAction = StartCoroutine(ThrowItems());
                break;
            case float n when n < 0.7f:
                currentAction = StartCoroutine(ThrowIngredients());
                break;
            case float n when n < 0.9f:
                currentAction = StartCoroutine(SlurpCauldron());
                break;
            case float n when n < 1f:
                if (queue.AreThereCustomers() != 0)
                    currentAction = StartCoroutine(ScareCustomer());
                break;
        }
    }

    private IEnumerator GoblinWanders()
    {
        Debug.Log("Goblin Wandering");
        if (!isScared && !isPerformingAction)
        {
            PickNewDestination();

            while (!ReachedTarget())
            {
                yield return null;
            }

            // random delay before next move
            yield return new WaitForSeconds(Random.Range(minWanderTime, maxWanderTime));



        }
    }

    #region Goblin Actions
    // Throwing items on the floor
    private IEnumerator ThrowItems()
    {
        isPerformingAction = true;
        // finds a random action from the ingredients list
        GameObject item = ingredients[Random.Range(0, ingredients.Count)];

        if (item == null)
            yield break;

        agent.SetDestination(item.transform.position);

        AudioManager.instance.sfxManager.StartConstantSFX(goblinMovement); //start movement sound
        while (!ReachedTarget())
        {
            yield return null;
        }

        AudioManager.instance.sfxManager.StopConstantSFX(); // stop movement sound

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized;
        item.transform.DOJump(randomDir, 1, 1, 0.5f);
        yield return new WaitForSeconds(throwFloorIngredient); // Simulated action time
        isPerformingAction = false;
    }

    private IEnumerator ThrowIngredients()
    {
        isPerformingAction = true;

        CrateHolder crate = crates[Random.Range(0, crates.Length)];

        agent.SetDestination(crate.transform.position);
        int amount = Random.Range(1, 4);

        AudioManager.instance.sfxManager.StartConstantSFX(goblinMovement); //start movement sound
        while (!ReachedTarget())
        {
            yield return null;
        }
        AudioManager.instance.sfxManager.StopConstantSFX(); // stop movement sound

        AudioManager.instance.sfxManager.StartConstantSFX(rummageSound); //playing the sound for rummaging
        for (int i = 0; i < amount; i++)
        {
            crate.GoblinInteraction(goblinHands);
        }
        yield return new WaitForSeconds(throwFromCrate); // Simulated action time
        AudioManager.instance.sfxManager.StopConstantSFX(); //stop rummage sound
        isPerformingAction = false;
    }


    private IEnumerator SlurpCauldron()
    {
        isPerformingAction = true;

        CauldronInteraction cauldron = cauldrons[Random.Range(0, cauldrons.Length)];

        agent.SetDestination(cauldron.transform.position);
        AudioManager.instance.sfxManager.StartConstantSFX(goblinMovement); //start movement sound
        while (!ReachedTarget())
        {
            yield return null;
        }
        AudioManager.instance.sfxManager.StopConstantSFX(); // stop movement sound

        cauldron.GetComponent<CauldronInteraction>().GoblinInteraction();
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.GoblinSounds, goblinSlurp.PickAudioClip(), true);

        yield return new WaitForSeconds(2f);
        isPerformingAction = false;
    }

    private IEnumerator ScareCustomer()
    {
        isPerformingAction = true;

        agent.SetDestination(queue.transform.position);
        AudioManager.instance.sfxManager.StartConstantSFX(goblinMovement); //start movement sound
        while (!ReachedTarget())
        {
            yield return null;
        }
        AudioManager.instance.sfxManager.StopConstantSFX(); // stop movement sound

        queue.ScareCustomer();
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.GoblinSounds, goblinScream.PickAudioClip(), true);

        yield return new WaitForSeconds(2f);
        isPerformingAction = false;
    }

    /// <summary> Scares goblin, this should be called from when  </summary>
    private void ScareAway()
    {
        if (isPerformingAction)
        {
            Debug.Log("Goblin got scared by the broom!");
            StopAllCoroutines();
            isScared = true;
            isPerformingAction = false;
            Invoke("ResetScare", 5f);
        }
    }

    /// <summary> Resets the scare bool to allow the goblin to go back to his antics </summary>
    private void ResetScare()
    {
        isScared = false;
    }
    #endregion

    #region Wandering Action
    private void PickNewDestination()
    {
        currentDestination = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(currentDestination);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, distance, layermask))
        {
            return navHit.position;
        }
        return origin; // If no valid point found, stay put
    }
    #endregion

    /// <summary> Used to find all the ingredients on the floor </summary>
    private void FindFloorItems()
    {
        ingredients = new List<GameObject>();
        PickupObject[] ing = FindObjectsOfType<PickupObject>();
        PotionOutput[] potions = FindObjectsOfType<PotionOutput>();

        foreach (PickupObject ingredient in ing)
        {
            if (!ingredient.AddedToCauldron() && !ingredient.isHeld)
                ingredients.Add(ingredient.gameObject);
        }

        foreach (PotionOutput potion in potions)
        {
            PickupObject pickup = potion.GetComponent<PickupObject>();

            if (!potion.givenToCustomer && !pickup.isHeld)
                ingredients.Add(potion.gameObject);
        }
    }

    /// <summary> Check if the agent has reached the target </summary>
    private bool ReachedTarget()
    {
        return !agent.pathPending &&
               agent.remainingDistance <= agent.stoppingDistance &&
               (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f);
    }
}
