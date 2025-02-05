using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private GameObject button;

    private void Awake()
    {
        button = this.gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.transform.DOKill();
        button.transform.DOScale(1.3f, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.transform.DOKill();
        button.transform.DOScale(1f, 0.2f);
    }

    public void OnSelect(BaseEventData eventData)
    {
        button.transform.DOKill();
        button.transform.DOScale(1.3f, 0.2f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        button.transform.DOKill();
        button.transform.DOScale(1f, 0.2f);
    }
}
