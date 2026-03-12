using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardDisplay : MonoBehaviour
{
    public TextMeshProUGUI[] scoreTexts;

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (GameManager.Instance == null) return;

        List<int> scores = GameManager.Instance.topScores;
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (i < scores.Count)
            {
                scoreTexts[i].text = (i + 1) + ". " + scores[i] + " pts";
            }
            else
            {
                scoreTexts[i].text = (i + 1) + ". ---";
            }
        }
    }
}
