using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI messageText;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            int score = GameManager.Instance.score;
            int total = GameManager.Instance.totalQuestions * 10;
            
            finalScoreText.text = "Final Score: " + score + " / " + total;

            // Win/Lose Condition (Point 6)
            if (score >= 70)
            {
                messageText.text = "Great! You know Nepali basics!";
                messageText.color = Color.green;
            }
            else
            {
                messageText.text = "Try again to improve!";
                messageText.color = Color.red;
            }
        }
    }
}
