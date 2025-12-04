using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultPage : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image _blackOverlay;
    [SerializeField] private Image _popupBody;
    [SerializeField] private ParticleSystemController _particleController;
    [SerializeField] private Image _nextLevelButtonImage;
    [SerializeField] private TextMeshProUGUI _nextLevelButtonText;

    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _secretWordText;
    [SerializeField] private TextMeshProUGUI _comboText;

    [Header("Assets")]
    [SerializeField] private Sprite _spriteNextLevel;
    [SerializeField] private Sprite _spriteTryAgain;

    [Header("Settings")]
    [SerializeField] private Color _colorWin = new Color(0.2f, 0.26f, 0.3f);
    [SerializeField] private Color _colorLose = new Color(0.91f, 0.17f, 0.06f);

    [Header("Dependencies")]
    [SerializeField] private LevelGenerator _levelGenerator;
    [SerializeField] private WordBoardManager _wordBoardManager;

    private bool _isLevelComplete;
    private string _targetSecretWord;
    private int _revealCharIndex;

    public void ViewPage(bool isComplete, string secretWord)
    {
        _isLevelComplete = isComplete;
        _targetSecretWord = secretWord;
        _secretWordText.text = "";

        UpdateVisuals();
        PlayOpenAnimation();
        UpdateStats();
    }

    private void UpdateVisuals()
    {
        if (_isLevelComplete)
        {
            _nextLevelButtonImage.sprite = _spriteNextLevel;
            _titleText.text = "ÓÐÎÂÅÍÜ ÏÐÎÉÄÅÍ!";
            _titleText.color = _colorWin;
            _nextLevelButtonText.text = "ÄÀËÜØÅ";
            _particleController.RestartParticlePlayback();
        }
        else
        {
            _nextLevelButtonImage.sprite = _spriteTryAgain;
            _titleText.text = "ÑËÎÂÎ ÍÅ ÓÃÀÄÀÍÎ!";
            _titleText.color = _colorLose;
            _nextLevelButtonText.text = "ÅÙÅ ÐÀÇ";
        }

        if (!LevelGenerator.IsRandomGame)
        {
            _nextLevelButtonImage.sprite = _spriteNextLevel;
            _nextLevelButtonText.text = "Â ÌÅÍÞ";
        }
    }

    private void UpdateStats()
    {
        _comboText.text = $"ÒÅÊÓÙÀß ÑÅÐÈß: {SaveData.CurrentLvl}";

        if (SaveData.CurrentLvl > PlayerStats.combo)
        {
            PlayerStats.combo = SaveData.CurrentLvl;
        }
    }

    private void PlayOpenAnimation()
    {
        _blackOverlay.transform.localScale = new Vector3(0, 1, 0);
        _popupBody.transform.localScale = Vector3.zero;

        gameObject.SetActive(true);

        DOTween.Sequence()
            .Append(_blackOverlay.transform.DOScaleX(1, 0.2f).SetEase(Ease.InCirc))
            .Join(_popupBody.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack))
            .OnComplete(StartWordReveal);
    }

    private void StartWordReveal()
    {
        _revealCharIndex = 0;
        InvokeRepeating(nameof(RevealNextCharacter), 0f, 0.1f);
    }

    private void RevealNextCharacter()
    {
        if (_revealCharIndex < _targetSecretWord.Length)
        {
            _secretWordText.text += _targetSecretWord[_revealCharIndex];
            _revealCharIndex++;
        }
        else
        {
            CancelInvoke(nameof(RevealNextCharacter));
        }
    }

    public void OnClickNextLvl()
    {
        SaveData.EnteredWords = new List<string>();

        DOTween.Sequence()
            .Append(_popupBody.transform.DOScale(0, 0.3f).SetEase(Ease.OutCirc))
            .Join(_blackOverlay.transform.DOScaleX(0, 0.3f).SetEase(Ease.OutCirc))
            .OnComplete(HandleLevelTransition);
    }

    private void HandleLevelTransition()
    {
        if (LevelGenerator.IsRandomGame)
        {
            if (_isLevelComplete)
            {
                SaveData.CurrentLvl++;
                _wordBoardManager.secretWord = _levelGenerator.GetNextWord(SaveData.CurrentLvl);
                _wordBoardManager.InitializeGame();
            }
            else
            {
                SaveData.CurrentLvl = 0;
                _levelGenerator.StartGeneration();

                _wordBoardManager.secretWord = _levelGenerator.GetNextWord(0);
                SaveData.WordsArray = _levelGenerator.RandomArray;
                _wordBoardManager.InitializeGame();
            }
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    public void OnClickCloseMatch()
    {
        SaveData.IsPlayNow = false;

        DOTween.Sequence()
            .Append(_levelGenerator.LoaderImage.transform.DOScaleY(1, 0.5f).SetEase(Ease.InCirc))
            .OnComplete(() =>
            {
                SaveData.SaveGameState();
                SceneManager.LoadScene(0);
            });
    }
}