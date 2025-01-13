using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class AbovePlayerUI : MonoBehaviour
{
    [SerializeField] private Sprite gamepadInteract;
    [SerializeField] private Sprite keyboardInteract;

    [SerializeField] private Sprite gamepadPickup;
    [SerializeField] private Sprite keyboardPickup;

    private Image _abovePlayerSprite;
    private Canvas _abovePlayerCanvas;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        _abovePlayerCanvas = GetComponentInParent<Canvas>();
        _abovePlayerSprite = GetComponent<Image>();
        DisableSprite();
    }

    private void Update()
    {
        _abovePlayerCanvas.transform.forward = Camera.main.transform.forward;
    }

    private void EnableSprite()
    {
        _abovePlayerSprite.enabled = true;
    }

    internal void DisableSprite()
    {
        _abovePlayerSprite.enabled = false;
    }

    internal void SetInteraction()
    {
        EnableSprite();

        if (Gamepad.all.Count > 0)
            _abovePlayerSprite.sprite = gamepadInteract;
        else
            _abovePlayerSprite.sprite = keyboardInteract;
    }

    internal void SetPickup()
    {
        EnableSprite();

        if (Gamepad.all.Count > 0)
            _abovePlayerSprite.sprite = gamepadPickup;
        else
            _abovePlayerSprite.sprite = keyboardPickup;
    }


}
