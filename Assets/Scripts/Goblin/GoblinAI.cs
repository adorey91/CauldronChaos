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
    private bool _isPerformingAction;
    [SerializeField] private float wanderRadius = 5f;
    [SerializeField] private float minWanderTime = 2f;
    [SerializeField] private float maxWanderTime = 5f;
    private bool _goblinActive;
    private Vector3 _currentDestination;

    [Header("Time Between Action")]
    [SerializeField] private float throwFromCrate;
    [SerializeField] private float throwFloorIngredient;

    [Header("SFX")]
    [SerializeField] private bool enableSfx;
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
    private bool _isFree = false;
    private float _noiseTimer;

    // References to the things the goblin can interact with - the only thing that changes is ingredients.
    private CrateHolder[] _crates;
    private List<GameObject> _ingredients;
    private CauldronInteraction[] _cauldrons;
    private QueueManager _queue;

    [Header("Slime Movement")]
    [SerializeField] private LayerMask slime;
    [SerializeField] private float slowMultiplier = 0.5f;
    private bool _isInSlime;
    private float _defaultSpeed;

    // Coroutines to handle the goblin behaviour
    private Coroutine _goblinBehaviour;
    private Coroutine _currentAction;

    private void Start()
    {
        if(agent != null)
            _defaultSpeed = agent.speed;

        _noiseTimer = Random.Range(noiseTimerMin, noiseTimerMax);
        _crates = FindObjectsOfType<CrateHolder>();
        _queue = FindObjectOfType<QueueManager>();
        _cauldrons = FindObjectsOfType<CauldronInteraction>();
    }

    //update loop to handle playing of idle sounds
    private void Update()
    {
        if(GameManager.Instance.gameState == GameState.Gameplay)
        {
            if (_noiseTimer < 0 && enableSfx)
            {
                SFXLibrary goblinSounds;

                if (_isFree)
                {
                    goblinSounds = goblinIdle;
                }
                else
                {
                    goblinSounds = goblinCry;
                }

                AudioManager.instance.sfxManager.PlaySFX(SFX_Type.GoblinSounds, goblinSounds.PickAudioClip(), true); //play audio clip
                _noiseTimer = Random.Range(noiseTimerMin, noiseTimerMax); //reset timer
            }

            if (enableSfx)
            {
                _noiseTimer -= Time.deltaTime;
            }
        }

        //Debug.Log(noiseTimer);
        if(Physics.Raycast(transform.position, Vector3.right, 1f, slime))
        {
            if (!_isInSlime)
            {
                _isInSlime = true;
                agent.speed *= slowMultiplier;
            }
        }
        else
        {
            if (_isInSlime)
            {
                _isInSlime = false;
                agent.speed = _defaultSpeed;
            }
        }
    }

    #region OnEnable / OnDisable / OnDestroy Events
    private void OnEnable()
    {
        Actions.OnStartGoblin += StartChaos;
        Actions.OnEndGoblin += EndChaos;
    }

    private void OnDisable()
    {
        Actions.OnStartGoblin -= StartChaos;
        Actions.OnEndGoblin -= EndChaos;
    }

    private void OnDestroy()
    {
        Actions.OnStartGoblin -= StartChaos;
        Actions.OnEndGoblin -= EndChaos;
    }
    #endregion

    private void StartChaos(bool isChallengeDay)
    {
        _goblinActive = true;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        agent.enabled = true;
        _goblinBehaviour = StartCoroutine(BehaviourLoop(isChallengeDay));
    }

    private void EndChaos()
    {
        _goblinActive = false;
        if (_goblinBehaviour != null)
            StopCoroutine(_goblinBehaviour);
    }

    private IEnumerator BehaviourLoop(bool isChallengeDay)
    {
        // if it's a challenge day goblin will be more active. else they need a wandering time
        _currentAction = null;
        while (true)
        {
            yield return new WaitForSeconds(actionCooldown);

            var roll = Random.Range(0f, 1f);

            if (_currentAction != null)
            {
                StopCoroutine(_currentAction);
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
                    _currentAction = StartCoroutine(GoblinWanders());
                else
                {
                    var newRoll = Random.Range(0f, 1f);
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
                if (_ingredients.Count > 0)
                    _currentAction = StartCoroutine(ThrowItems());
                break;
            case float n when n < 0.7f:
                _currentAction = StartCoroutine(ThrowIngredients());
                break;
            case float n when n < 0.9f:
                _currentAction = StartCoroutine(SlurpCauldron());
                break;
            case float n when n < 1f:
                if (_queue.AreThereCustomers() != 0)
                    _currentAction = StartCoroutine(ScareCustomer());
                break;
        }
    }

    private IEnumerator GoblinWanders()
    {
        Debug.Log("Goblin Wandering");
        if (!_isPerformingAction)
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
        _isPerformingAction = true;
        // finds a random action from the ingredients list
        var item = _ingredients[Random.Range(0, _ingredients.Count)];

        if (item == null)
            yield break;

        agent.SetDestination(item.transform.position);

        AudioManager.instance.sfxManager.StartConstantSFX(goblinMovement); //start movement sound
        while (!ReachedTarget())
        {
            yield return null;
        }

        AudioManager.instance.sfxManager.StopConstantSFX(); // stop movement sound

        var randomDir = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f)).normalized;
        item.transform.DOJump(randomDir, 1, 1, 0.5f);
        yield return new WaitForSeconds(throwFloorIngredient); // Simulated action time
        _isPerformingAction = false;
    }

    private IEnumerator ThrowIngredients()
    {
        _isPerformingAction = true;

        var crate = _crates[Random.Range(0, _crates.Length)];

        agent.SetDestination(crate.transform.position);
        var amount = Random.Range(1, 4);

        AudioManager.instance.sfxManager.StartConstantSFX(goblinMovement); //start movement sound
        while (!ReachedTarget())
        {
            yield return null;
        }
        AudioManager.instance.sfxManager.StopConstantSFX(); // stop movement sound

        AudioManager.instance.sfxManager.StartConstantSFX(rummageSound); //playing the sound for rummaging
        for (var i = 0; i < amount; i++)
        {
            crate.GoblinInteraction(goblinHands);
        }
        yield return new WaitForSeconds(throwFromCrate); // Simulated action time
        AudioManager.instance.sfxManager.StopConstantSFX(); //stop rummage sound
        _isPerformingAction = false;
    }


    private IEnumerator SlurpCauldron()
    {
        _isPerformingAction = true;

        var cauldron = _cauldrons[Random.Range(0, _cauldrons.Length)];

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
        _isPerformingAction = false;
    }

    private IEnumerator ScareCustomer()
    {
        _isPerformingAction = true;

        agent.SetDestination(_queue.transform.position);
        AudioManager.instance.sfxManager.StartConstantSFX(goblinMovement); //start movement sound
        while (!ReachedTarget())
        {
            yield return null;
        }
        AudioManager.instance.sfxManager.StopConstantSFX(); // stop movement sound

        _queue.ScareCustomer();
        AudioManager.instance.sfxManager.PlaySFX(SFX_Type.GoblinSounds, goblinScream.PickAudioClip(), true);

        yield return new WaitForSeconds(2f);
        _isPerformingAction = false;
    }
    #endregion

    #region Wandering Action
    private void PickNewDestination()
    {
        _currentDestination = RandomNavSphere(transform.position, wanderRadius, -1);
        agent.SetDestination(_currentDestination);
    }

    private Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        var randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        return NavMesh.SamplePosition(randomDirection, out var navHit, distance, layermask) ? navHit.position : origin; // If no valid point found, stay put
    }
    #endregion

    /// <summary> Used to find all the ingredients on the floor </summary>
    private void FindFloorItems()
    {
        _ingredients = new List<GameObject>();
        var ing = FindObjectsOfType<PickupObject>();
        var potions = FindObjectsOfType<PotionOutput>();

        foreach (var ingredient in ing)
        {
            if (!ingredient.AddedToCauldron() && !ingredient.isHeld)
                _ingredients.Add(ingredient.gameObject);
        }

        foreach (var potion in potions)
        {
            var pickup = potion.GetComponent<PickupObject>();

            if (!potion.givenToCustomer && !pickup.isHeld)
                _ingredients.Add(potion.gameObject);
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
