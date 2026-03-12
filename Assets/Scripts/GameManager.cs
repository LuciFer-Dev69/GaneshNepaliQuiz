using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class QuizQuestion {
    public string question;
    public string correct;
    public List<string> options;
    public string iconPath;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public int score = 0;
    public int totalQuestions = 10;
    public int currentQuestionIndex = 0;
    public List<QuizQuestion> questionPool = new List<QuizQuestion>();
    public List<QuizQuestion> quizSession = new List<QuizQuestion>();

    [Header("Leaderboard")]
    public int highWeight = 5;
    public List<int> topScores = new List<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScores();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartQuiz()
    {
        score = 0;
        currentQuestionIndex = 0;
        
        // Error prevention: Check if pool is empty
        if (questionPool.Count == 0) {
            Debug.LogError("No questions in pool! Make sure they are prepared in Menu.");
            return;
        }

        quizSession = new List<QuizQuestion>(questionPool);
        
        // Shuffle Question Pool
        for (int i = 0; i < quizSession.Count; i++)
        {
            QuizQuestion temp = quizSession[i];
            int r = Random.Range(i, quizSession.Count);
            quizSession[i] = quizSession[r];
            quizSession[r] = temp;
        }

        if (quizSession.Count > totalQuestions)
            quizSession.RemoveRange(totalQuestions, quizSession.Count - totalQuestions);
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public QuizQuestion GetNextQuestion()
    {
        if (currentQuestionIndex < quizSession.Count)
        {
            return quizSession[currentQuestionIndex++];
        }
        return null;
    }

    // Leaderboard Logic
    public void SaveCurrentScore()
    {
        topScores.Add(score);
        topScores.Sort((a, b) => b.CompareTo(a)); // Sort descending
        if (topScores.Count > 5) topScores.RemoveRange(5, topScores.Count - 5);
        
        for (int i = 0; i < topScores.Count; i++)
        {
            PlayerPrefs.SetInt("HighScore_" + i, topScores[i]);
        }
        PlayerPrefs.Save();
    }

    void LoadHighScores()
    {
        topScores.Clear();
        for (int i = 0; i < 5; i++)
        {
            if (PlayerPrefs.HasKey("HighScore_" + i))
                topScores.Add(PlayerPrefs.GetInt("HighScore_" + i));
        }
    }
}
