using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class JsonProcessor : MonoBehaviour
{
    private const string SourceFileName = "defaultWords";
    private const string OutputExtension = ".json";

    private static readonly Regex WordExtractionRegex = new Regex(@"\d+\s+(\w+)", RegexOptions.Compiled);
    private static readonly Regex CleanupRegex = new Regex(@"[\s\d]", RegexOptions.Compiled);

    private void Start()
    {
        ProcessDefaultWords();
    }

    private void ProcessDefaultWords()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(SourceFileName);

        if (textAsset == null)
        {
            Debug.LogError($"[JsonProcessor] Failed to load resource: {SourceFileName}");
            return;
        }

        string processedContent = ExtractAndFilterWords(textAsset.text);
        SaveToFile(SourceFileName, processedContent);

        Debug.Log($"[JsonProcessor] Processing complete. File saved: {SourceFileName}{OutputExtension}");
    }

    private string ExtractAndFilterWords(string content)
    {
        if (string.IsNullOrEmpty(content)) return string.Empty;

        string[] lines = content.Split('\n');
        List<string> processedWords = new List<string>();

        foreach (string line in lines)
        {
            Match match = WordExtractionRegex.Match(line);

            if (match.Success)
            {
                string rawWord = match.Groups[1].Value;
                string cleanWord = CleanupRegex.Replace(rawWord, string.Empty);

                if (IsValidWord(cleanWord))
                {
                    processedWords.Add(cleanWord);
                }
            }
        }

        return string.Join("\n", processedWords);
    }

    private bool IsValidWord(string word)
    {
        return word.Length > 0 && word.Length <= 10;
    }

    private void SaveToFile(string fileName, string content)
    {
        string filePath = Path.Combine(Application.dataPath, "Resources", fileName + OutputExtension);

        try
        {
            File.WriteAllText(filePath, content);
        }
        catch (IOException e)
        {
            Debug.LogError($"[JsonProcessor] Error writing file: {e.Message}");
        }
    }
}