using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuAnimation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Vector3 defaultScale;
    float downScale;

    void Start()
    {
        defaultScale = transform.localScale;
        downScale = defaultScale.x*0.7f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(downScale, 0.1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(defaultScale.x, 0.2f).SetEase(Ease.InOutBack);
    }
}
