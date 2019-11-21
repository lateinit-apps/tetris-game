using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int score = 0;
    private int lines;
    private int level = 1;

    public int linesPerLevel = 5;

    public Text linesText;
    public Text levelText;
    public Text scoreText;

    private const int minLines = 1;
    private const int maxLines = 1;

    public void ScoreLines(int n)
    {
        n = Mathf.Clamp(n, minLines, maxLines);

        switch (n)
        {
            case 1:
                score += 40 * level;
                break;
            case 2:
                score += 100 * level;
                break;
            case 3:
                score += 300 * level;
                break;
            case 4:
                score += 1200 * level;
                break;
        }

        UpdateUIText(); 
    }

    public void Reset()
    {
        level = 1;
        lines = linesPerLevel * level;
    }

    private void UpdateUIText()
    {
        if (linesText)
        {
            linesText.text = lines.ToString();
        }
        if (levelText)
        {
            levelText.text = level.ToString();
        }
        if (scoreText)
        {
            scoreText.text = PadZero(score, 5);
        }
    }

    private string PadZero(int n, int padDigits)
    {
        string nStr = n.ToString();

        while (nStr.Length < padDigits)
        {
            nStr = "0" + nStr;
        }

        return nStr;
    }
}
