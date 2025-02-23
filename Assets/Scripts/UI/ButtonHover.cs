using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private GameObject button;
    [SerializeField] private bool isLevelSelectButton;
    private Button thisButton;

    private void Awake()
    {
        button = this.gameObject;
        thisButton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (thisButton.interactable)
        {
            button.transform.DOKill();
            // Ensure independent updates and smoother scaling.
            button.transform.DOScale(1.3f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (isLevelSelectButton)
                {
                    button.transform.SetAsLastSibling();
                }
            });
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (thisButton.interactable)
        {
            if (isLevelSelectButton)
                button.transform.SetAsFirstSibling();

            button.transform.DOKill();
            // Ensure independent updates and smoother scaling.
            button.transform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (thisButton.interactable)
        {
            button.transform.DOKill();
            // Ensure independent updates and smoother scaling.
            button.transform.DOScale(1.3f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (isLevelSelectButton)
                {
                    button.transform.SetAsLastSibling();
                }
            });
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (thisButton.interactable)
        {
            if (isLevelSelectButton)
                button.transform.SetAsFirstSibling();

            button.transform.DOKill();
            // Ensure independent updates and smoother scaling.
            button.transform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
        }
    }
}
