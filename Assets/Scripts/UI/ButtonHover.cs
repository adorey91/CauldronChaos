using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private bool isLevelSelectButton;
    private GameObject button;
    private Button thisButton;

    private void Awake()
    {
        button = this.gameObject;
        thisButton = button.GetComponent<Button>();
        if (thisButton == null)
            Debug.LogError("Button component missing on " + gameObject.name);
    }


    private void OnEnable()
    {
        thisButton.onClick.AddListener(ResizeButton);
    }

    private void OnDisable()
    {
        thisButton.onClick.RemoveListener(ResizeButton);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisButton == null || button == null) return; // Prevent null reference
        if (!thisButton.interactable) return; // Prevent interaction with disabled buttons

        button.transform.DOKill();
        // Ensure independent updates and smoother scaling.

        if (isLevelSelectButton)
            button.transform.SetAsLastSibling();

        button.transform.DOScale(1.3f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (thisButton == null || button == null) return; // Prevent null reference
        if (!thisButton.interactable) return; // Prevent interaction with disabled buttons

        if (isLevelSelectButton)
            button.transform.SetAsFirstSibling();

        button.transform.DOKill();
        // Ensure independent updates and smoother scaling.
        button.transform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (thisButton == null || button == null) return; // Prevent null reference
        if (!thisButton.interactable) return; // Prevent interaction with disabled buttons
        button.transform.DOKill();
        // Ensure independent updates and smoother scaling.
        if (isLevelSelectButton)
            button.transform.SetAsLastSibling();

        button.transform.DOScale(1.3f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (thisButton == null || button == null) return; // Prevent null reference
        if (!thisButton.interactable) return; // Prevent interaction with disabled buttons

        if (isLevelSelectButton)
            button.transform.SetAsFirstSibling();

        button.transform.DOKill();
        // Ensure independent updates and smoother scaling.
        button.transform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    public void ResizeButton()
    {
        if (isLevelSelectButton)
            button.transform.SetAsFirstSibling();

        button.transform.DOKill();
        // Ensure independent updates and smoother scaling.
        button.transform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
    }
}
