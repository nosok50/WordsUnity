using DG.Tweening;
using System.Text.RegularExpressions;
using Systems.Security;
using TMPro;
using UnityEngine;

public class PageCreateCode : MonoBehaviour
{
    [Header("UI Containers")]
    [SerializeField] private GameObject _blackZone;
    [SerializeField] private GameObject _body;
    [SerializeField] private GameObject _partCreateCode;
    [SerializeField] private GameObject _partCopyCode;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI _textPrintWord;
    [SerializeField] private TextMeshProUGUI _textGeneratedCode;

    [Header("References")]
    [SerializeField] private Msg _msg;

    private TouchScreenKeyboard _keyboard;
    private const string WordValidationPattern = "^[А-Яа-я]{3,10}$";

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
            _textPrintWord.text = _keyboard.text;
            _keyboard = null;
        }
    }

    public void OnClickOpenPage()
    {
        _blackZone.transform.localScale = new Vector3(0, 1, 0);
        _body.transform.localScale = Vector3.zero;

        gameObject.SetActive(true);
        _partCreateCode.SetActive(true);
        _partCopyCode.SetActive(false);

        DOTween.Sequence()
            .Append(_blackZone.transform.DOScaleX(1, 0.2f).SetEase(Ease.InCirc))
            .Join(_body.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack));
    }

    public void OnClickClosePage()
    {
        DOTween.Sequence()
            .Append(_body.transform.DOScale(0, 0.3f).SetEase(Ease.OutCirc))
            .Join(_blackZone.transform.DOScaleX(0, 0.3f).SetEase(Ease.OutCirc))
            .OnComplete(() => gameObject.SetActive(false));
    }

    public void OnClickEnterText()
    {
        _keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    public void OnClickCreateCode()
    {
        string originalWord = _textPrintWord.text;

        if (Regex.IsMatch(originalWord, WordValidationPattern))
        {
            // Using CipherManager from previous context
            _textGeneratedCode.text = CipherManager.Encrypt(originalWord);

            _partCreateCode.SetActive(false);
            _partCopyCode.SetActive(true);
        }
        else
        {
            HandleValidationError(originalWord);
        }
    }

    private void HandleValidationError(string input)
    {
        if (input.Equals("Ввести слово...") || string.IsNullOrEmpty(input))
        {
            _msg.SendMsg("Введите слово :((");
        }
        else
        {
            _msg.SendMsg("Введенное слово не соответствует требования (Смотри описание)");
        }
    }

    public void OnClickCopyCode()
    {
        CopyToClipboard(_textGeneratedCode.text);
    }

    private void CopyToClipboard(string textToCopy)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            CopyAndroid(textToCopy);
        }
        else
        {
            GUIUtility.systemCopyBuffer = textToCopy;
        }

        Debug.Log($"Copied to clipboard: {textToCopy}");
        _msg.SendMsg("Код скопирован");
    }

    private void CopyAndroid(string textToCopy)
    {
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject clipboardManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "clipboard"))
            using (AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData"))
            using (AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newPlainText", "Unity Text", textToCopy))
            {
                clipboardManager.Call("setPrimaryClip", clipData);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Android copy failed: {e.Message}");
        }
    }
}