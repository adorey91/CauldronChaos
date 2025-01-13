using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class AbovePlayerUI : MonoBehaviour
{
    [Header("Keyboard Settings")]
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private string keyboardPickupText;
    [SerializeField] private string keyboardInteractText;
    [SerializeField] private string keyboardClockwiseText;
    [SerializeField] private string keyboardCounterClockwiseText;

    [Header("Gamepad Settings")]
    [SerializeField] private Sprite gamepadPickup;
    [SerializeField] private Sprite gamepadInteract;
    [SerializeField] private Sprite gamepadClockwise;
    [SerializeField] private Sprite gamepadCounterClockwise;

    [Header("Stirring Sprites")]
    [SerializeField] private Image clockwiseImage;
    [SerializeField] private Image counterClockwiseImage;
    private TextMeshProUGUI _clockwiseText;
    private TextMeshProUGUI _counterClockwiseText;

    [Header("Game Object Above Player")]
    [SerializeField] private GameObject _abovePlayerGO;
    private Image _abovePlayerSprite;
    private Canvas _abovePlayerCanvas;
    private RectTransform _abovePlayerRectTransform;
    private TextMeshProUGUI _abovePlayerText;


    private Camera mainCamera;

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();

        _abovePlayerCanvas = GetComponentInParent<Canvas>();
        _abovePlayerSprite = _abovePlayerGO.GetComponent<Image>();
        _abovePlayerRectTransform = _abovePlayerGO.GetComponent<RectTransform>();
        _abovePlayerText = _abovePlayerGO.GetComponentInChildren<TextMeshProUGUI>();

        _clockwiseText = clockwiseImage.GetComponentInChildren<TextMeshProUGUI>();
        _counterClockwiseText = counterClockwiseImage.GetComponentInChildren<TextMeshProUGUI>();

        DisableSprite();
    }

    private void Update()
    {
        _abovePlayerCanvas.transform.forward = Camera.main.transform.forward;
    }

    #region InteractionPickup
    private void EnableSprite()
    {
        _abovePlayerSprite.enabled = true;
    }

    internal void DisableSprite()
    {
        _abovePlayerSprite.enabled = false;
        _abovePlayerText.text = "";
    }

    internal void SetInteraction()
    {
        EnableSprite();
        SetWidth(0.5f);

        if (Gamepad.all.Count > 0)
        {
            _abovePlayerText.text = "";
            _abovePlayerSprite.sprite = gamepadInteract;

        }
        else
        {
            _abovePlayerSprite.sprite = keyboardSprite;
            _abovePlayerText.text = keyboardInteractText;
        }
    }

    internal void SetPickup()
    {
        EnableSprite();

        if (Gamepad.all.Count > 0)
        {
            SetWidth(0.5f);
            _abovePlayerText.text = "";
            _abovePlayerSprite.sprite = gamepadPickup;
        }
        else
        {
            SetWidth(1.5f);
            _abovePlayerSprite.sprite = keyboardSprite;
            _abovePlayerText.text = keyboardPickupText;
        }

        _abovePlayerSprite.preserveAspect = true;
    }
    #endregion

    #region Stirring
    private void EnableStirringSprites()
    {
        clockwiseImage.enabled = true;
        counterClockwiseImage.enabled = true;
        _clockwiseText.text = "";
        _counterClockwiseText.text = "";
    }

    internal void SetStirring()
    {
        EnableStirringSprites();

        if (Gamepad.all.Count > 0)
        {
            clockwiseImage.sprite = gamepadClockwise;
            counterClockwiseImage.sprite = gamepadCounterClockwise;
        }
        else
        {
            clockwiseImage.sprite = keyboardSprite;
            counterClockwiseImage.sprite = keyboardSprite;
            _clockwiseText.text = keyboardClockwiseText;
            _counterClockwiseText.text = keyboardCounterClockwiseText;
        }
    }

    internal void DisableStirring()
    {
        clockwiseImage.enabled = false;
        counterClockwiseImage.enabled = false;
        _clockwiseText.text = "";
        _counterClockwiseText.text = "";
    }
    #endregion

    private void SetWidth(float width)
    {
        if (_abovePlayerRectTransform == null) return;

        _abovePlayerRectTransform.sizeDelta = new Vector2(width, 0.5f);
    }
}
