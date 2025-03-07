using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomerBehaviour : MonoBehaviour
{
    // Order details
    [Header("Order Details")]
    public RecipeSO requestedOrder;
    public string customerName;
    public Transform customerHands;
    private CustomTimer tipTimer;
    private bool giveTip;

    [Header("UI for Order")]
    [SerializeField] private GameObject orderUiPrefab;
    private Image orderIcon;
    private Transform orderUiParent;
    private bool hasShownOrder;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    [SerializeField] private Animator animator;
    private Vector3 targetPosition;
    private bool leavingQueue;
    internal bool hasJoinedQueue;

    private GameObject orderUiInstance;

    private void Start()
    {
        tipTimer = new CustomTimer(2, true);
        moveSpeed = 3f;
        animator.speed = 1f;
    }

    private void Update()
    {
        if (tipTimer.UpdateTimer())
            giveTip = false;

        if (!leavingQueue)
            MoveToTarget();
    }

    #region Order Events
    internal void AssignOrder(RecipeSO order, Transform parent)
    {
        this.requestedOrder = order;
        orderUiParent = parent;
        giveTip = true;
    }

    private void DisplayOrderUI()
    {
        orderUiInstance = Instantiate(orderUiPrefab, orderUiParent);
        // Find the Image component in the instantiated object, not globally

        Transform child = orderUiInstance.transform.GetChild(0);
        Transform text = orderUiInstance.transform.GetChild(1); // this is for testing only
        orderIcon = child.GetComponent<Image>();
        TextMeshProUGUI order = text.GetComponent<TextMeshProUGUI>();

        if (orderIcon == null)
        {
            Debug.LogError("OrderIcon (Image) not found inside orderUiInstance!");
            return;
        }

        if (requestedOrder.potionIcon != null)
        {
            order.enabled = false;
            orderIcon.sprite = requestedOrder.potionIcon;
        }
        else
        {
            order.enabled = true;
            order.text = requestedOrder.recipeName;
            orderIcon.enabled = false;
        }
        
        tipTimer.StartTimer();
    }

    internal RecipeSO HasOrder()
    {
        return requestedOrder;
    }

    internal void OrderComplete()
    {
        if (giveTip == true)
            Actions.OnCustomerServed?.Invoke(true, requestedOrder.points);
        else
            Actions.OnCustomerServed?.Invoke(false, requestedOrder.points);
    }
    #endregion

    #region Positioning
    internal void SetTarget(Vector3 position)
    {
        animator.SetBool("isWalking", true);
        targetPosition = position;
        leavingQueue = false;
    }

    // Leave the queue and call a callback once finished
    internal void LeaveQueue(Vector3 exitPosition, System.Action onExitComplete)
    {
        leavingQueue = true;
        Destroy(orderUiInstance);
        StartCoroutine(LeaveAndExit(exitPosition, onExitComplete));
    }

    private void MoveToTarget()
    {
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            animator.SetBool("isWalking", true);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 270, 0);
        }
        else
        {
            transform.rotation = Quaternion.identity;
            hasJoinedQueue = true;
            animator.SetBool("isWalking", false);

            if (!hasShownOrder)
            {
                hasShownOrder = true;
                DisplayOrderUI();
            }
        }
    }

    private IEnumerator LeaveAndExit(Vector3 exitPosition, System.Action onExitComplete)
    {
        animator.SetBool("isWalking", true);
        float stepDistance = 1.2f; // Distance to step back
        Vector3 backwardStep = transform.position - transform.forward * stepDistance; // Step back 1 unit

        transform.rotation = Quaternion.Euler(0, 180, 0);

        // Step 1: Move 1 unit backward
        while (Vector3.Distance(transform.position, backwardStep) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, backwardStep, moveSpeed * Time.deltaTime);
            yield return null;
        }


        transform.rotation = Quaternion.Euler(0, 90, 0); // Rotate -90 degrees

        // Step 3: Move to the exit
        while (Vector3.Distance(transform.position, exitPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, exitPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Notify that the customer has left
        onExitComplete?.Invoke();
    }

    internal void ScareAway()
    {
        animator.speed = 2f;
        moveSpeed = 12f;
    }
    #endregion
}
