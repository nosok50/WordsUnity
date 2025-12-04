using DG.Tweening;
using UnityEngine;

public class LobbyIntroAnimationItem : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Vector3 _initialRotation;

    private void Start()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.eulerAngles;

        transform.localPosition = new Vector3(_initialPosition.x, _initialPosition.y - 500, _initialPosition.z);

        float randomZ = UnityEngine.Random.Range(-300f, 300f);
        transform.eulerAngles = new Vector3(_initialRotation.x, _initialRotation.y, _initialRotation.z + randomZ);

        LobbyMenuManager.OnIntroAnimationTriggered += PlayEntranceAnimation;
    }

    private void OnDestroy()
    {
        LobbyMenuManager.OnIntroAnimationTriggered -= PlayEntranceAnimation;
    }

    private void PlayEntranceAnimation()
    {
        transform.DOLocalMoveY(_initialPosition.y, UnityEngine.Random.Range(0.3f, 0.7f))
            .SetEase(Ease.OutCirc);

        transform.DORotate(_initialRotation, UnityEngine.Random.Range(0.3f, 0.7f))
            .SetEase(Ease.OutCirc);
    }
}