using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LetterCellManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _checkIcon;
    [SerializeField] private TextMeshProUGUI _letterText;

    [Header("State Assets")]
    [SerializeField] private Sprite _defaultStatus;
    [SerializeField] private Sprite _blackStatus;
    [SerializeField] private Sprite _orangeStatus;
    [SerializeField] private Sprite _greenStatus;
    [SerializeField] private Sprite _selectStatus;

    // Public Properties for external access
    public Image Background => _background;
    public Image CheckIcon => _checkIcon;
    public TextMeshProUGUI LetterText => _letterText;

    public Sprite DefaultStatus => _defaultStatus;
    public Sprite BlackStatus => _blackStatus;
    public Sprite OrangeStatus => _orangeStatus;
    public Sprite GreenStatus => _greenStatus;
    public Sprite SelectStatus => _selectStatus;

    public void PlayClickAnimation()
    {
        DOTween.Sequence()
            .Append(transform.DOScale(0.8f, 0.2f))
            .Append(transform.DOScale(1f, 0.2f).SetEase(Ease.InOutBack));
    }
}