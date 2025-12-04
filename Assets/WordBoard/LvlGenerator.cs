using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    public static bool IsRandomGame = true;

    [Header("Dependencies")]
    [SerializeField] private WordBoardManager _wordBoardManager;
    [SerializeField] private TextMeshProUGUI _textTitle;
    [SerializeField] private Image _loaderImage;

    // Public property for ResultPage to access the loader
    public Image LoaderImage => _loaderImage;

    [HideInInspector] public string[] Words;
    [HideInInspector] public string[] RandomArray;

    public HashSet<string> WordsHashSet { get; private set; }

    private void Start()
    {
        DOTween.Sequence()
            .Append(_loaderImage.transform.DOScaleY(0, 0.5f).SetEase(Ease.OutCirc))
            .AppendCallback(InitializeLevel);
    }

    private void InitializeLevel()
    {
        LoadWordsDictionary();

        if (!SaveData.IsPlayNow)
        {
            SetupNewGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void LoadWordsDictionary()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("russian_nouns");

        if (jsonFile == null)
        {
            Debug.LogError("[LevelGenerator] Failed to load 'russian_nouns' resource.");
            return;
        }

        string[] jsonWords = jsonFile.text.Split('\n');

        List<string> combinedWordsList = new List<string>(Words ?? new string[0]);
        combinedWordsList.AddRange(jsonWords);

        Words = combinedWordsList.ToArray();
        WordsHashSet = new HashSet<string>(Words, StringComparer.OrdinalIgnoreCase);
    }

    private void SetupNewGame()
    {
        if (IsRandomGame)
        {
            SaveData.IsPlayNow = true;
            SaveData.EnteredWords = new List<string>();

            StartGeneration();

            SaveData.WordsArray = RandomArray;
            SaveData.CurrentLvl = 0;

            _wordBoardManager.secretWord = GetNextWord(0);
            _wordBoardManager.InitializeGame();
        }
        else
        {
            _wordBoardManager.secretWord = PageEnterCode.SecretWord;
            PlayerStats.combo = 0;

            _wordBoardManager.InitializeGame();
            _textTitle.text = "CODE MODE";
        }
    }

    private void ResumeGame()
    {
        RandomArray = SaveData.WordsArray;
        _wordBoardManager.secretWord = GetNextWord(SaveData.CurrentLvl);
        _wordBoardManager.InitializeGame();

        if (SaveData.EnteredWords != null)
        {
            foreach (var word in SaveData.EnteredWords)
            {
                _wordBoardManager.FastEnterWord(word);
            }
        }
    }

    // Renamed from PrepareRandomWords and made public for ResultPage
    public void StartGeneration()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("defaultWords");
        string[] sourceWords = jsonFile != null ? jsonFile.text.Split('\n') : new string[0];

        if (sourceWords.Length == 0)
        {
            Debug.LogError("[LevelGenerator] 'defaultWords' is empty or missing.");
        }

        RandomArray = new string[sourceWords.Length];
        Array.Copy(sourceWords, RandomArray, sourceWords.Length);

        System.Random rng = new System.Random();
        int n = RandomArray.Length;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            string value = RandomArray[k];
            RandomArray[k] = RandomArray[n];
            RandomArray[n] = value;
        }
    }

    public string GetNextWord(int index)
    {
        if (IsRandomGame)
        {
            _textTitle.text = $"LEVEL {index + 1}";
        }

        if (RandomArray != null && index >= 0 && index < RandomArray.Length)
        {
            return RandomArray[index].ToUpper();
        }

        return "ERROR";
    }
}