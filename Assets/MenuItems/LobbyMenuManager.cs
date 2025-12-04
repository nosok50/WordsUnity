using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenuManager : MonoBehaviour
{
    public static event Action OnIntroAnimationTriggered;

    [Header("UI References")]
    [SerializeField] private Transform _logoContainer;
    [SerializeField] private TextMeshProUGUI _textMaxCombo;
    [SerializeField] private Image _loaderImage;

    [Header("Settings")]
    [SerializeField] private float _logoAnimationInterval = 10f;
    [SerializeField] private float _cellSwapSpeed = 0.1f;

    private List<Transform> _logoItems = new List<Transform>();
    private int _currentSwapIndex;

    private void Awake()
    {
        OnIntroAnimationTriggered = null;

        foreach (Transform logoPart in _logoContainer)
        {
            _logoItems.Add(logoPart);
        }

        InvokeRepeating(nameof(AnimateLogo), _logoAnimationInterval, _logoAnimationInterval);

        DOTween.Sequence()
            .SetDelay(1f)
            .OnComplete(() => OnIntroAnimationTriggered?.Invoke());
    }

    private void Start()
    {
        _loaderImage.transform.DOScaleY(0, 0.5f).SetEase(Ease.OutCirc);

        _textMaxCombo.text = $"ксвьхи явер: {PlayerStats.combo}";
    }

    private void AnimateLogo()
    {
        _currentSwapIndex = 0;
        InvokeRepeating(nameof(SwapLogoCells), 0, _cellSwapSpeed);
    }

    private void SwapLogoCells()
    {
        if (_currentSwapIndex < _logoItems.Count)
        {
            Transform currentItem = _logoItems[_currentSwapIndex];

            DOTween.Sequence()
                .Append(currentItem.DOScaleX(0, 0.3f).SetEase(Ease.InBack))
                .Append(currentItem.DOScaleX(1, 0.3f).SetEase(Ease.OutBack));

            _currentSwapIndex++;
        }
        else
        {
            CancelInvoke(nameof(SwapLogoCells));
        }
    }

    public void OnClick_RandomGame()
    {
        DOTween.Sequence()
            .Append(_loaderImage.transform.DOScaleY(1, 0.5f).SetEase(Ease.InCirc))
            .AppendCallback(LoadGameScene);
    }

    private void LoadGameScene()
    {
        LevelGenerator.IsRandomGame = true;
        SceneManager.LoadScene(1);
    }

    public void OnClick_CreateWord() { }
    public void OnClick_InputWord() { }
}