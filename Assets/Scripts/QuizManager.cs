using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI feedbackText;
    public Image characterIcon;
    public Image progressBarFill; 
    public Button[] answerButtons;
    public TextMeshProUGUI[] buttonTexts;

    [Header("Settings")]
    public float timeLimit = 10f;
    private float currentTime;
    private bool isAnswering = false;
    private Coroutine timerCoroutine;

    private QuizQuestion currentQuestion;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartQuiz();
            LoadNextQuestion();
        }
    }

    void LoadNextQuestion()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        currentQuestion = GameManager.Instance.GetNextQuestion();

        if (currentQuestion == null)
        {
            if (GameManager.Instance != null) GameManager.Instance.SaveCurrentScore();
            UnityEngine.SceneManagement.SceneManager.LoadScene("Result");
            return;
        }

        // Setup UI
        questionText.text = currentQuestion.question;
        
        // Dynamic sprite loading
        #if UNITY_EDITOR
        characterIcon.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/" + currentQuestion.iconPath);
        #else
        characterIcon.sprite = Resources.Load<Sprite>(currentQuestion.iconPath.Split('.')[0]);
        #endif
        
        progressText.text = "Question " + GameManager.Instance.currentQuestionIndex + " / " + GameManager.Instance.totalQuestions;
        if (progressBarFill != null) progressBarFill.fillAmount = (float)GameManager.Instance.currentQuestionIndex / GameManager.Instance.totalQuestions;
        
        feedbackText.text = "";

        // Shuffle Options
        List<string> options = new List<string>(currentQuestion.options);
        for (int i = 0; i < options.Count; i++)
        {
            string temp = options[i];
            int r = Random.Range(i, options.Count);
            options[i] = options[r];
            options[r] = temp;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = true;
            buttonTexts[i].text = options[i];
            
            string selectedOption = options[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(selectedOption));
        }

        currentTime = timeLimit;
        isAnswering = true;
        timerCoroutine = StartCoroutine(TimerCountdown());
    }

    IEnumerator TimerCountdown()
    {
        while (currentTime > 0 && isAnswering)
        {
            currentTime -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(currentTime).ToString();
            yield return null;
        }

        if (currentTime <= 0 && isAnswering)
        {
            isAnswering = false;
            ShowFeedback(false, true);
        }
    }

    void OnAnswerSelected(string answer)
    {
        if (!isAnswering) return;
        isAnswering = false;
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        // Prevent double-click spam immediately (Requirement 3)
        foreach (var b in answerButtons) b.interactable = false;

        bool isCorrect = (answer == currentQuestion.correct);
        if (isCorrect) GameManager.Instance.AddScore(10);
        
        ShowFeedback(isCorrect, false);
    }

    void ShowFeedback(bool isCorrect, bool isTimeout)
    {
        foreach (var b in answerButtons) b.interactable = false;

        if (isTimeout) feedbackText.text = "❌ Out of Time!";
        else feedbackText.text = isCorrect ? "✔ Correct! +10" : "❌ Incorrect!";

        feedbackText.color = isCorrect ? Color.green : Color.red;
        Invoke("LoadNextQuestion", 1.5f);
    }
}
