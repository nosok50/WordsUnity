using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Msg : MonoBehaviour
{
    public TextMeshProUGUI textMsg;

    Vector2 currentPosition;
    int duration;
    private void Start()
    {
        currentPosition = transform.localPosition;
        transform.localPosition = new Vector3(currentPosition.x, 350f,0);
        InvokeRepeating("Doration", 0, 1);
    }

    public void SendMsg(string text)
    {
        textMsg.text = text;
        duration = 5;
        transform.localPosition = new Vector3(currentPosition.x, 350f, 0);
        transform.DOLocalMoveY(currentPosition.y, 0.4f).SetEase(Ease.OutBack);
    }

    void Doration()
    {
        if (duration > 0)
        {
            duration--;
            if (duration <= 0)
            {
                transform.DOLocalMoveY((currentPosition.y + 350), 0.4f).SetEase(Ease.InBack);
            }
        }
    }
}
