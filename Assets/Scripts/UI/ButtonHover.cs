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

    private void Awake()
    {
        button = this.gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        button.transform.DOKill();
        button.transform.DOScale(1.3f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            if(isLevelSelectButton)
            {
                button.transform.SetAsLastSibling();
            }
        });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isLevelSelectButton)
            button.transform.SetAsFirstSibling();
        
        button.transform.DOKill();
        button.transform.DOScale(1f, 0.2f);
        button.transform.DOLocalMoveZ(0f, 0.2f).SetUpdate(true); // Moves back
    }

    public void OnSelect(BaseEventData eventData)
    {
        button.transform.DOKill();
        button.transform.DOScale(1.3f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            if (isLevelSelectButton)
            {
                button.transform.SetAsLastSibling();
            }
        });
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (isLevelSelectButton)
            button.transform.SetAsFirstSibling();

        button.transform.DOKill();
        button.transform.DOScale(1f, 0.2f).SetUpdate(true);
    }
}
