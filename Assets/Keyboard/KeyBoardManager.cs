using UnityEngine;

public class KeyBoardManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private WordBoardManager _wordBoardManager;
    [SerializeField] private GameObject _boardContainer;

    [Header("Prefabs")]
    [SerializeField] private KeyManager _keyPrefab;
    [SerializeField] private GameObject _rowPrefab;
    [SerializeField] private KeyManager _deleteKeyPrefab;

    private readonly string[] _keyboardLayout = {
        "…÷” ≈Õ√ÿŸ«’⁄",
        "‘€¬¿œ–ŒÀƒ∆›",
        "ﬂ◊—Ã»“‹¡ﬁ"
    };

    private void Start()
    {
        GenerateKeyboard();
    }

    private void GenerateKeyboard()
    {
        Transform lastRowTransform = null;

        foreach (string rowData in _keyboardLayout)
        {
            GameObject rowObj = Instantiate(_rowPrefab, _boardContainer.transform);
            lastRowTransform = rowObj.transform;

            foreach (char symbol in rowData)
            {
                CreateKey(_keyPrefab, lastRowTransform, symbol);
            }
        }

        // Instantiate delete key in the last row
        if (lastRowTransform != null)
        {
            CreateKey(_deleteKeyPrefab, lastRowTransform, '-');
        }
    }

    private void CreateKey(KeyManager prefab, Transform parent, char symbol)
    {
        KeyManager keyInstance = Instantiate(prefab, parent);
        keyInstance.Initialize(symbol, _wordBoardManager);
    }
}