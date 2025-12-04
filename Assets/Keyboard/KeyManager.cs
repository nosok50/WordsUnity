using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class KeyManager : MonoBehaviour
{
    private enum KeyStatus
    {
        Default = 0,
        Incorrect = 1,
        WrongPosition = 2,
        Correct = 3
    }

    [Header("UI References")]
    [SerializeField] private Sprite[] _buttonStates;
    [SerializeField] private TextMeshProUGUI _keyName;

    private WordBoardManager _wordBoardManager;
    private Image _keyImage;
    private char _assignedChar;
    private bool _isInitialized;

    private void Awake()
    {
        _keyImage = GetComponent<Image>();
    }

    private void OnDestroy()
    {
        if (_isInitialized && _assignedChar != '-')
        {
            WordBoardManager.OnLetterStatusChanged -= OnLetterStatusUpdated;
        }
    }

    public void Initialize(char character, WordBoardManager manager)
    {
        _assignedChar = character;
        _wordBoardManager = manager;
        _keyName.text = character.ToString();

        if (_assignedChar != '-')
        {
            WordBoardManager.OnLetterStatusChanged += OnLetterStatusUpdated;
        }

        _isInitialized = true;
    }

    public void OnClickInsertKey()
    {
        if (_wordBoardManager != null)
        {
            _wordBoardManager.InputLetter(_assignedChar);
        }
    }

    private void OnLetterStatusUpdated(char letter, int statusIndex)
    {
        if (statusIndex == 0)
        {
            _keyImage.sprite = _buttonStates[0];
            return;
        }
        if (_assignedChar != letter) return;

        UpdateVisualState((KeyStatus)statusIndex);
    }

    private void UpdateVisualState(KeyStatus status)
    {
        int spriteIndex = (int)status;

        if (spriteIndex >= 0 && spriteIndex < _buttonStates.Length)
        {
            _keyImage.sprite = _buttonStates[spriteIndex];
        }
        
    }
}