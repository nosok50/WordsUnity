using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordBoardManager : MonoBehaviour
{
    public static event Action<char, int> OnLetterStatusChanged;

    [Header("Dependencies")]
    [SerializeField] private Msg _messageSystem;
    [SerializeField] private ResultPage _resultPage;
    [SerializeField] private LevelGenerator _levelGenerator;

    [Header("UI Components")]
    [SerializeField] private LetterCellManager _cellPrefab;
    [SerializeField] private GridLayoutGroup _gridContainer;
    [SerializeField] private TextMeshProUGUI _secretWordDebugText;

    [HideInInspector] public string secretWord;

    private List<LetterCellManager> _cellGrid = new List<LetterCellManager>();
    private List<LetterCellManager> _currentCheckRowCells;

    private int _wordLength;
    private int _rowStartIndex = 0;
    private int _rowEndIndex;
    private int _currentCellIndex = 0;
    private int _maxAttempts = 6;
    private float _fontSize;

    private int _animationIndex;
    private int _reverseIterator;
    private int _completedLettersCount;

    private void Awake()
    {
        OnLetterStatusChanged = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EnterWord();
        }
    }

    public void InitializeGame()
    {
        ClearGrid();

        _currentCellIndex = 0;
        _rowStartIndex = 0;
        _wordLength = secretWord.Length;
        _gridContainer.constraintCount = _wordLength;

        CalculateLayoutSettings();
        GenerateGrid();

        if (_cellGrid.Count > 0)
        {
            _cellGrid[_currentCellIndex].Background.sprite = _cellGrid[_currentCellIndex].SelectStatus;
        }

        _rowEndIndex = _currentCellIndex + _wordLength;

        OnLetterStatusChanged?.Invoke('0', 0);
    }

    private void ClearGrid()
    {
        foreach (Transform child in _gridContainer.transform)
        {
            Destroy(child.gameObject);
        }
        _cellGrid.Clear();
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < _maxAttempts; i++)
        {
            for (int j = 0; j < _wordLength; j++)
            {
                LetterCellManager cell = Instantiate(_cellPrefab, _gridContainer.transform);

                cell.transform.localScale = Vector3.zero;
                cell.transform.DOScale(1, 0.5f);

                cell.LetterText.text = "";
                cell.LetterText.fontSize = _fontSize;

                _cellGrid.Add(cell);
            }
        }
    }

    private void CalculateLayoutSettings()
    {
        int sizeX = 136;
        int sizeY = 172;
        _fontSize = 84.84f;
        _maxAttempts = 6;

        switch (_wordLength)
        {
            case 6:
                sizeX = 113;
                sizeY = 143;
                _fontSize = 68.24f;
                break;
            case 7:
                sizeX = 96;
                sizeY = 123;
                _fontSize = 68.24f;
                _maxAttempts = 6;
                break;
            case 8:
                sizeX = 84;
                sizeY = 107;
                _fontSize = 55.9f;
                _maxAttempts = 8;
                break;
            case 9:
                sizeX = 75;
                sizeY = 95;
                _fontSize = 68.24f;
                _maxAttempts = 8;
                break;
            case 10:
                sizeX = 67;
                sizeY = 86;
                _fontSize = 46.5f;
                _maxAttempts = 8;
                break;
        }

        _gridContainer.cellSize = new Vector2(sizeX, sizeY);
    }

    public void InputLetter(char letter)
    {
        bool canDelete = _currentCellIndex > _rowStartIndex;
        bool canInsert = _currentCellIndex < _rowEndIndex;

        if (canDelete && letter == '-')
        {
            HandleBackspace();
        }
        else if (canInsert && letter != '-')
        {
            HandleInsertion(letter);
        }
    }

    private void HandleBackspace()
    {
        if (_currentCellIndex != _cellGrid.Count)
        {
            _cellGrid[_currentCellIndex].Background.sprite = _cellGrid[_currentCellIndex].DefaultStatus;
        }

        _currentCellIndex--;

        LetterCellManager cell = _cellGrid[_currentCellIndex];
        cell.LetterText.text = "";
        cell.Background.sprite = cell.SelectStatus;
        cell.PlayClickAnimation();
    }

    private void HandleInsertion(char letter)
    {
        LetterCellManager cell = _cellGrid[_currentCellIndex];
        cell.LetterText.text = letter.ToString();
        cell.Background.sprite = cell.DefaultStatus;

        if (_currentCellIndex < _rowEndIndex)
        {
            _currentCellIndex++;

            if (_currentCellIndex < _rowEndIndex)
            {
                _cellGrid[_currentCellIndex].Background.sprite = _cellGrid[_currentCellIndex].SelectStatus;
                _cellGrid[_currentCellIndex].PlayClickAnimation();
            }
        }
    }

    public void EnterWord()
    {
        int enteredCharCount = 0;
        _currentCheckRowCells = new List<LetterCellManager>();
        string enteredWordReversed = "";

        int startIndex = _currentCellIndex - 1;
        int endIndex = (_currentCellIndex - 1) - (_wordLength - 1);

        if (_currentCellIndex == _rowEndIndex)
        {
            for (int i = startIndex; i >= endIndex; i--)
            {
                if (!string.IsNullOrWhiteSpace(_cellGrid[i].LetterText.text))
                {
                    enteredCharCount++;
                    _currentCheckRowCells.Add(_cellGrid[i]);
                    enteredWordReversed += _cellGrid[i].LetterText.text;
                }
            }
        }

        if (enteredCharCount != _wordLength)
        {
            _messageSystem.SendMsg("Word is incomplete :(");
            return;
        }

        string enteredWord = ReverseString(enteredWordReversed);

        _reverseIterator = _currentCheckRowCells.Count - 1;
        _completedLettersCount = 0;
        _animationIndex = 0;

        if (LevelGenerator.IsRandomGame)
        {
            if (IsWordInDictionary(enteredWord))
            {
                SaveData.EnteredWords.Add(enteredWord);
                InvokeRepeating(nameof(AnimateLetterReveal), 0, 0.2f);
            }
            else
            {
                _messageSystem.SendMsg("Этого слова нет в словаре");
            }
        }
        else
        {
            InvokeRepeating(nameof(AnimateLetterReveal), 0, 0.2f);
        }
    }

    private void AnimateLetterReveal()
    {
        if (_animationIndex >= _currentCheckRowCells.Count)
        {
            CancelInvoke(nameof(AnimateLetterReveal));
            return;
        }

        LetterCellManager currentCell = _currentCheckRowCells[_reverseIterator];
        char charToCheck = currentCell.LetterText.text[0];
        char secretChar = secretWord[_animationIndex];

        Sequence sequence = DOTween.Sequence();

        sequence.Append(currentCell.transform.DOScaleX(0, 0.3f).SetEase(Ease.InBack))
                .AppendCallback(() => UpdateCellStatus(currentCell, charToCheck, secretChar))
                .Append(currentCell.transform.DOScaleX(1, 0.3f).SetEase(Ease.OutBack));

        if (_animationIndex == _currentCheckRowCells.Count - 1)
        {
            sequence.OnComplete(FinishTurn);

            CancelInvoke(nameof(AnimateLetterReveal));
        }

        _reverseIterator--;
        _animationIndex++;
    }

    private void UpdateCellStatus(LetterCellManager cell, char inputChar, char targetChar)
    {
        if (inputChar == targetChar)
        {
            cell.Background.sprite = cell.GreenStatus;
            OnLetterStatusChanged?.Invoke(inputChar, 3);
            _completedLettersCount++;

            Debug.Log(_completedLettersCount);
        }
        else if (secretWord.Contains(inputChar))
        {
            cell.Background.sprite = cell.OrangeStatus;
            OnLetterStatusChanged?.Invoke(inputChar, 2);
        }
        else
        {
            cell.Background.sprite = cell.BlackStatus;
            OnLetterStatusChanged?.Invoke(inputChar, 1);
            cell.LetterText.color = ParseHexColor("#868686");
        }
    }

    private void FinishTurn()
    {
        CancelInvoke(nameof(AnimateLetterReveal));

        _rowStartIndex = _currentCellIndex;
        _rowEndIndex = _currentCellIndex + _wordLength;

        if (_currentCellIndex < (_cellGrid.Count - 1))
        {
            _cellGrid[_currentCellIndex].Background.sprite = _cellGrid[_currentCellIndex].SelectStatus;
        }

        if (_completedLettersCount >= _wordLength)
        {
            _resultPage.ViewPage(true, secretWord);
        }
        else if (_currentCellIndex >= (_cellGrid.Count - 1))
        {
            _resultPage.ViewPage(false, secretWord);
        }
    }

    public void FastEnterWord(string word)
    {
        foreach (char c in word)
        {
            InputLetter(c);
        }

        int enteredCharCount = 0;
        _currentCheckRowCells = new List<LetterCellManager>();

        int startIndex = _currentCellIndex - 1;
        int endIndex = (_currentCellIndex - 1) - (_wordLength - 1);

        for (int i = startIndex; i >= endIndex; i--)
        {
            if (!string.IsNullOrWhiteSpace(_cellGrid[i].LetterText.text))
            {
                enteredCharCount++;
                _currentCheckRowCells.Add(_cellGrid[i]);
            }
        }

        if (enteredCharCount != _wordLength) return;

        _reverseIterator = _currentCheckRowCells.Count - 1;
        _completedLettersCount = 0;
        _animationIndex = 0;

        while (_animationIndex < _currentCheckRowCells.Count)
        {
            LetterCellManager currentCell = _currentCheckRowCells[_reverseIterator];
            char charToCheck = currentCell.LetterText.text[0];
            char secretChar = secretWord[_animationIndex];

            currentCell.transform.DOScaleX(1, 0.3f).SetEase(Ease.OutBack);

            UpdateCellStatus(currentCell, charToCheck, secretChar);

            _reverseIterator--;
            _animationIndex++;
        }

        FinishTurn();
    }

    private string ReverseString(string input)
    {
        char[] charArray = input.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    private bool IsWordInDictionary(string targetWord)
    {
        return _levelGenerator.WordsHashSet.Contains(targetWord);
    }

    private Color ParseHexColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        Debug.LogError($"Invalid hex color: {hex}");
        return Color.white;
    }
}