using DG.Tweening;
using Systems.Security;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PageEnterCode : MonoBehaviour
{
    // Static variable to pass data to the next scene
    public static string SecretWord;

    [Header("UI Containers")]
    [SerializeField] private GameObject _blackOverlay;
    [SerializeField] private GameObject _popupBody;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _inputCodeText;

    [Header("Dependencies")]
    [SerializeField] private Msg _messageSystem;

    private TouchScreenKeyboard _keyboard;

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        if (_keyboard == null) return;

        if (_keyboard.status == TouchScreenKeyboard.Status.Done ||
            _keyboard.status == TouchScreenKeyboard.Status.Canceled)
        {
            _inputCodeText.text = _keyboard.text;

            if (_keyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                Debug.Log("Keyboard canceled by user.");
            }

            _keyboard = null;
        }
    }

    public void OnClickOpenPage()
    {
        _blackOverlay.transform.localScale = new Vector3(0, 1, 0);
        _popupBody.transform.localScale = Vector3.zero;

        gameObject.SetActive(true);

        DOTween.Sequence()
            .Append(_blackOverlay.transform.DOScaleX(1, 0.2f).SetEase(Ease.InCirc))
            .Join(_popupBody.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack));
    }

    public void OnClickClosePage()
    {
        DOTween.Sequence()
            .Append(_popupBody.transform.DOScale(0, 0.3f).SetEase(Ease.OutCirc))
            .Join(_blackOverlay.transform.DOScaleX(0, 0.3f).SetEase(Ease.OutCirc))
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void OnClickEnterText()
    {
        _keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void OnClickEnterCode()
    {
        // Basic validation
        if (_inputCodeText.text.Equals("¬вести код...") || string.IsNullOrWhiteSpace(_inputCodeText.text))
        {
            _messageSystem.SendMsg("¬ведите код!");
            return;
        }

        // Decrypt and load game
        SecretWord = CipherManager.Decrypt(_inputCodeText.text);

        // Disable random mode for the custom code game
        LevelGenerator.IsRandomGame = false;

        SceneManager.LoadScene(1);
    }
}