using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isPerformingAction = false;
    private float actionCooldown = 5f;
    private bool isScared = false;
    [SerializeField] private Transform hidingCorner;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(BehaviourLoop());
    }

    private IEnumerator BehaviourLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(actionCooldown);

            if (isScared) continue;

            float roll = Random.Range(0f, 1f);

            switch (roll)
            {
                case float n when n < 0.4f: StartCoroutine(ThrowItems()); break;
                case float n when n < 0.7f: StartCoroutine(ThrowIngredients()); break;
                case float n when n < 0.9f: StartCoroutine(SlurpCauldron()); break;
                case float n when n < 1f: StartCoroutine(ScareCustomer()); break;
            }
        }
    }

    IEnumerator ThrowItems()
    {
        isPerformingAction = true;
        Debug.Log("Goblin is throwing items!");
        // Find nearby items and apply force
        yield return new WaitForSeconds(3f); // Simulated action time
        isPerformingAction = false;
    }

    IEnumerator ThrowIngredients()
    {
        isPerformingAction = true;
        Debug.Log("Goblin is throwing ingredients!");
        // Find crate/shelf and spawn ingredients
        yield return new WaitForSeconds(3f);
        isPerformingAction = false;
    }

    IEnumerator SlurpCauldron()
    {
        isPerformingAction = true;
        Debug.Log("Goblin is slurping the cauldron!");
        // Reset the cauldron recipe
        yield return new WaitForSeconds(2f);
        isPerformingAction = false;
    }

    IEnumerator ScareCustomer()
    {
        isPerformingAction = true;
        Debug.Log("Goblin scared a customer!");
        // Remove a customer from the queue
        yield return new WaitForSeconds(2f);
        isPerformingAction = false;
    }

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

    private GameObject FindClosestObject(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;

        float minDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(obj.transform.position, position);
            if (distance < minDistance)
            {
                closest = obj;
                minDistance = distance;
            }
        }

        return closest;
    }
}
