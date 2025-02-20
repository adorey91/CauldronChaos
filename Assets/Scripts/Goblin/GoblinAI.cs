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
    private NavMeshAgent agent;
    private bool isPerformingAction = false;
    private bool isScared = false;

    [Header("Time Between Action")]
    [SerializeField] private float throwFromCrate;
    [SerializeField] private float throwFloorIngredient;

    // References to the things the goblin can interact with - the only thing that changes is ingredients.
    private CrateHolder[] crates;
    private List<PickupObject> ingredients;
    private CauldronInteraction[] cauldrons;
    private QueueManager queue;

    // Coroutines to handle the goblin behaviour
    private Coroutine goblinBehaviour;
    private Coroutine currentAction;

    // Action to start the goblin chaos
    /// <summary> Event to start the goblin chaos </summary>
    public static System.Action <bool> StartGoblinChaos;
    /// <summary> Event to end the goblin chaos </summary>
    public static System.Action EndGoblinChaos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        crates = FindObjectsOfType<CrateHolder>();
        queue = FindObjectOfType<QueueManager>();
        cauldrons = FindObjectsOfType<CauldronInteraction>();
    }

    private void OnEnable()
    {
        StartGoblinChaos += StartChaos;
        EndGoblinChaos += EndChaos;
    }

    private void OnDisable()
    {
        StartGoblinChaos -= StartChaos;
        EndGoblinChaos -= EndChaos;
    }

    private void StartChaos(bool isChallengeDay)
    {
        goblinBehaviour = StartCoroutine(BehaviourLoop(isChallengeDay));
    }

    private void EndChaos()
    {
        if(goblinBehaviour != null)
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

            float roll = UnityEngine.Random.Range(0f, 1f);

            if(currentAction != null)
            {
                StopCoroutine(currentAction);
            }

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
    }

    #region Goblin Actions
    // Throwing items on the floor
    private IEnumerator ThrowItems()
    {
        isPerformingAction = true;

        PickupObject item = ingredients[Random.Range(0, ingredients.Count)];

        if (item == null)
            yield break;

        agent.SetDestination(item.transform.position);

        while (!ReachedTarget())
        {
            yield return null;
        }

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

        while (!ReachedTarget())
        {
            yield return null;
        }

        for(int i = 0; i < amount; i++)
        {
            crate.GoblinInteraction(goblinHands);
        }
        yield return new WaitForSeconds(throwFromCrate); // Simulated action time
        isPerformingAction = false;
    }


    private IEnumerator SlurpCauldron()
    {
        isPerformingAction = true;

        CauldronInteraction cauldron = cauldrons[Random.Range(0, cauldrons.Length)];

        agent.SetDestination(cauldron.transform.position);
        while (!ReachedTarget())
        {
            yield return null;
        }
        cauldron.GetComponent<CauldronInteraction>().GoblinInteraction();

        yield return new WaitForSeconds(2f);
        isPerformingAction = false;
    }

    private IEnumerator ScareCustomer()
    {
        isPerformingAction = true;

        agent.SetDestination(queue.transform.position);
        while (!ReachedTarget())
        {
            yield return null;
        }
        queue.ScareCustomer();

        yield return new WaitForSeconds(2f);
        isPerformingAction = false;
    }
    #endregion

    public void ScareAway()
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

    private void ResetScare()
    {
        isScared = false;
    }

    /// <summary> Used to find all the ingredients on the floor </summary>
    private void FindFloorItems()
    {
        ingredients = new List<PickupObject>();
        PickupObject[] ing = FindObjectsOfType<PickupObject>();

        foreach (PickupObject ingredient in ing)
        {
            if (!ingredient.AddedToCauldron())
                ingredients.Add(ingredient);
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
