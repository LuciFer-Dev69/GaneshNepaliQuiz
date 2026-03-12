using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameAutomator : EditorWindow
{
    private static Color panelColor = new Color(0, 0, 0, 0.75f);
    private static Color buttonColor = new Color(0.18f, 0.18f, 0.18f, 1f); // Slightly darker for "Pro" feel

    [MenuItem("Tools/Build Nepali Game (Final Professional)")]
    public static void BuildGame()
    {
        var questions = GetOfficialQuestions();

        CreateMenuScene(questions);
        CreateQuizScene();
        CreateResultScene();
        CreateInstructionsScene();
        CreateCreditsScene();

        List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
        string[] scenes = { "Menu", "Quiz", "Result", "Instructions", "Credits" };
        foreach (var s in scenes) buildScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/" + s + ".unity", true));
        EditorBuildSettings.scenes = buildScenes.ToArray();

        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        Debug.Log("FINAL Polish Complete: Hitboxes (300x80), Color Tints, and Cursor feedback added!");
    }

    static List<QuizQuestion> GetOfficialQuestions() {
        return new List<QuizQuestion> {
            new QuizQuestion { question = "What is the Nepali word for Hello?", correct = "Namaste", options = new List<string>{"Namaste", "Dhanyabad", "Pani"}, iconPath = "greetingicon.jpg" },
            new QuizQuestion { question = "What is the Nepali word for Thank You?", correct = "Dhanyabad", options = new List<string>{"Pani", "Dhanyabad", "Namaste"}, iconPath = "greetingicon.jpg" },
            new QuizQuestion { question = "What is the Nepali word for Water?", correct = "Pani", options = new List<string>{"Pani", "Khana", "Mitho"}, iconPath = "greetingicon.jpg" },
            new QuizQuestion { question = "What is the Nepali word for Food?", correct = "Khana", options = new List<string>{"Khana", "Pani", "Mitho"}, iconPath = "1.png" },
            new QuizQuestion { question = "What is the Nepali word for Good?", correct = "Ramro", options = new List<string>{"Ramro", "Mitho", "Thulo"}, iconPath = "3doticon.png" },
            new QuizQuestion { question = "What is the Nepali word for Big?", correct = "Thulo", options = new List<string>{"Thulo", "Sano", "Ramro"}, iconPath = "3doticon.png" },
            new QuizQuestion { question = "What is the Nepali word for Small?", correct = "Sano", options = new List<string>{"Thulo", "Sano", "Ramro"}, iconPath = "3doticon.png" },
            new QuizQuestion { question = "What is the Nepali word for One?", correct = "Ek", options = new List<string>{"Ek", "Dui", "Tin"}, iconPath = "1.png" },
            new QuizQuestion { question = "What is the Nepali word for Two?", correct = "Dui", options = new List<string>{"Tin", "Dui", "Ek"}, iconPath = "2.png" },
            new QuizQuestion { question = "What is the Nepali word for Three?", correct = "Tin", options = new List<string>{"Dui", "Char", "Tin"}, iconPath = "3.png" }
        };
    }

    static void CreateMenuScene(List<QuizQuestion> pool)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject canvasObj = CreateCanvas();
        CreateImage(canvasObj, "BG", "MainMenu.jpg", true);

        GameObject gmObj = new GameObject("GameManager");
        var gm = gmObj.AddComponent<GameManager>();
        gm.questionPool = pool;

        GameObject pnl = CreatePanel(canvasObj, "MainPnl", new Vector2(-250, 0), new Vector2(450, 550));
        CreateTMP(pnl, "T", "LEARN NEPALI", 55, new Vector2(0, 180), FontStyles.Bold);
        CreateCleanButton(pnl, "START GAME", new Vector2(0, 70), "Quiz");
        CreateCleanButton(pnl, "HOW TO PLAY", new Vector2(0, -40), "Instructions");
        CreateCleanButton(pnl, "EXIT", new Vector2(0, -150), "", true);

        GameObject scorePnl = CreatePanel(canvasObj, "Leaderboard", new Vector2(250, 0), new Vector2(300, 450));
        CreateTMP(scorePnl, "LT", "HIGH SCORES", 28, new Vector2(0, 180), FontStyles.Bold);
        var lb = scorePnl.AddComponent<LeaderboardDisplay>();
        lb.scoreTexts = new TextMeshProUGUI[5];
        for(int i=0; i<5; i++) {
            var txtObj = CreateTMP(scorePnl, "Score_" + i, (i+1) + ". ---", 18, new Vector2(0, 100 - (i*40)));
            lb.scoreTexts[i] = txtObj.GetComponent<TextMeshProUGUI>();
        }

        CreateMuteButton(canvasObj);
        SetupAudioManager();
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Menu.unity");
    }

    static void CreateQuizScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject canvasObj = CreateCanvas();
        CreateImage(canvasObj, "BG", "GameplayBackground.jpg", true);

        GameObject mainPnl = CreatePanel(canvasObj, "QuizPanel", Vector2.zero, new Vector2(1000, 650));
        
        GameObject barBg = CreatePanel(mainPnl, "ProgressBar", new Vector2(0, 280), new Vector2(650, 15));
        barBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        GameObject barFill = new GameObject("Fill");
        barFill.transform.SetParent(barBg.transform);
        var fillImg = barFill.AddComponent<Image>();
        fillImg.color = Color.green;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0;
        RectTransform fillRt = barFill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero; fillRt.anchorMax = Vector2.one; fillRt.offsetMin = Vector2.zero; fillRt.offsetMax = Vector2.zero;

        var timer = CreateTMP(mainPnl, "Timer", "10s", 32, new Vector2(420, 270), FontStyles.Bold);
        var progress = CreateTMP(mainPnl, "Progress", "1 / 10", 22, new Vector2(-420, 270));
        var charIcon = CreateImage(mainPnl, "Icon", "greetingicon.jpg", false, new Vector2(0, 160), new Vector2(200, 200)).GetComponent<Image>();
        var qText = CreateTMP(mainPnl, "Q", "Loading...", 38, new Vector2(0, 10), FontStyles.Bold).GetComponent<TextMeshProUGUI>();
        var feed = CreateTMP(mainPnl, "Feedback", "", 40, new Vector2(0, -270)).GetComponent<TextMeshProUGUI>();

        Button[] btns = new Button[3];
        TextMeshProUGUI[] bTexts = new TextMeshProUGUI[3];
        for(int i=0; i<3; i++) {
            // High-Precision Hitbox Size (Requirement 5)
            var bObj = CreateCleanButton(mainPnl, "Opt", new Vector2(-320 + (i * 320), -140), "", false, new Vector2(300, 90));
            btns[i] = bObj.GetComponent<Button>();
            bTexts[i] = bObj.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        }

        var qm = mainPnl.AddComponent<QuizManager>();
        qm.questionText = qText;
        qm.timerText = timer.GetComponent<TextMeshProUGUI>();
        qm.progressText = progress.GetComponent<TextMeshProUGUI>();
        qm.feedbackText = feed;
        qm.characterIcon = charIcon;
        qm.answerButtons = btns;
        qm.buttonTexts = bTexts;
        qm.progressBarFill = fillImg;

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Quiz.unity");
    }

    static void CreateResultScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject canvasObj = CreateCanvas();
        CreateImage(canvasObj, "BG", "MainMenu.jpg", true);
        GameObject pnl = CreatePanel(canvasObj, "ResultPnl", Vector2.zero, new Vector2(700, 500));
        
        var scoreTxt = CreateTMP(pnl, "Score", "SCORE: 0 / 100", 50, new Vector2(0, 120), FontStyles.Bold).GetComponent<TextMeshProUGUI>();
        var msgTxt = CreateTMP(pnl, "Msg", "", 32, new Vector2(0, 20)).GetComponent<TextMeshProUGUI>();

        var rm = pnl.AddComponent<ResultManager>();
        rm.finalScoreText = scoreTxt;
        rm.messageText = msgTxt;

        CreateCleanButton(pnl, "PLAY AGAIN", new Vector2(0, -100), "Quiz");
        CreateCleanButton(pnl, "MAIN MENU", new Vector2(0, -200), "Menu");

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Result.unity");
    }

    static GameObject CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f; 
        canvasObj.AddComponent<GraphicRaycaster>();
        if (!Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>()) {
            var es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            System.Type inputSystemModule = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModule != null) es.AddComponent(inputSystemModule);
            else es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        return canvasObj;
    }

    static GameObject CreatePanel(GameObject parent, string name, Vector2 pos, Vector2 size)
    {
        GameObject pnl = new GameObject(name);
        pnl.transform.SetParent(parent.transform);
        var img = pnl.AddComponent<Image>();
        img.color = panelColor;
        img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        img.type = Image.Type.Sliced;
        RectTransform rt = pnl.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        return pnl;
    }

    static GameObject CreateTMP(GameObject parent, string name, string text, float size, Vector2 pos, FontStyles style = FontStyles.Normal)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform);
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = style;
        tmp.color = Color.white;
        RectTransform rt = textObj.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = new Vector2(size * 25, size * 5);
        return textObj;
    }

    static GameObject CreateImage(GameObject parent, string name, string file, bool isBg, Vector2 pos = default, Vector2 size = default)
    {
        GameObject i = new GameObject(name);
        i.transform.SetParent(parent.transform);
        var img = i.AddComponent<Image>();
        img.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/" + file);
        if (isBg) {
            img.rectTransform.anchorMin = Vector2.zero; img.rectTransform.anchorMax = Vector2.one;
            img.rectTransform.offsetMin = Vector2.zero; img.rectTransform.offsetMax = Vector2.zero;
        } else {
            img.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            img.rectTransform.anchoredPosition = pos; img.rectTransform.sizeDelta = size;
            img.preserveAspect = true;
        }
        return i;
    }

    static GameObject CreateCleanButton(GameObject parent, string label, Vector2 pos, string target = "", bool isQuit = false, Vector2 size = default)
    {
        if (size == default) size = new Vector2(300, 85);
        GameObject btnObj = new GameObject(label + " Button");
        btnObj.transform.SetParent(parent.transform);
        var img = btnObj.AddComponent<Image>();
        img.color = buttonColor;
        img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        img.type = Image.Type.Sliced;
        img.raycastTarget = true;

        var btn = btnObj.AddComponent<Button>();
        btn.transition = Selectable.Transition.ColorTint; // Requirement 1
        
        // Setup Professional Tints
        var colors = btn.colors;
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        colors.selectedColor = buttonColor;
        btn.colors = colors;

        btnObj.AddComponent<ButtonAnimator>(); 

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos; rt.sizeDelta = size;
        
        CreateTMP(btnObj, "Label", label, size.y * 0.4f, Vector2.zero, FontStyles.Bold);
        
        var ctrl = parent.GetComponentInParent<Canvas>().gameObject.GetComponent<SceneController>() ?? parent.GetComponentInParent<Canvas>().gameObject.AddComponent<SceneController>();
        if (!isQuit) UnityEditor.Events.UnityEventTools.AddStringPersistentListener(btn.onClick, ctrl.LoadScene, target);
        else UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(btn.onClick, ctrl.QuitGame);
        return btnObj;
    }

    static void SetupAudioManager()
    {
        if (Object.FindObjectsByType<AudioManager>(FindObjectsSortMode.None).Length == 0) {
            GameObject audioMgr = new GameObject("PersistentAudio");
            var source = audioMgr.AddComponent<AudioSource>();
            source.playOnAwake = true; source.loop = true;
            source.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/tune.mp3");
            audioMgr.AddComponent<AudioManager>();
        }
    }

    static void CreateMuteButton(GameObject canvas)
    {
        GameObject muteObj = new GameObject("Mute Button");
        muteObj.transform.SetParent(canvas.transform);
        muteObj.AddComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/pause.png");
        muteObj.AddComponent<MuteButtonScript>();
        var btn = muteObj.AddComponent<Button>();
        
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(btn.onClick, muteObj.GetComponent<MuteButtonScript>().ToggleGameMute);
        
        RectTransform rt = muteObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(-20, -20);
        rt.sizeDelta = new Vector2(65, 65);
    }

    static void CreateInstructionsScene() {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject canvasObj = CreateCanvas();
        CreateImage(canvasObj, "BG", "GameplayBackground.jpg", true);
        GameObject pnl = CreatePanel(canvasObj, "Pnl", Vector2.zero, new Vector2(750, 500));
        CreateTMP(pnl, "T", "HOW TO PLAY", 45, new Vector2(0, 200), FontStyles.Bold);
        CreateTMP(pnl, "C", "1. Answer within 10 seconds.\n2. Hover over buttons for hand cursor.\n3. Click accurately on the centered hitboxes.\n4. Get 70+ to enter the Leaderboard!", 26, new Vector2(0, 0));
        CreateCleanButton(pnl, "BACK", new Vector2(0, -180), "Menu", false, new Vector2(200, 65));
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Instructions.unity");
    }

    static void CreateCreditsScene() {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        GameObject canvasObj = CreateCanvas();
        CreateImage(canvasObj, "BG", "MainMenu.jpg", true);
        GameObject pnl = CreatePanel(canvasObj, "Pnl", Vector2.zero, new Vector2(750, 400));
        CreateTMP(pnl, "T", "CREDITS", 45, new Vector2(0, 140), FontStyles.Bold);
        CreateTMP(pnl, "C", "Multimedia Team Project\nDeveloped with Pro Hitbox Calibration", 28, new Vector2(0, 0));
        CreateCleanButton(pnl, "MAIN MENU", new Vector2(0, -130), "Menu", false, new Vector2(200, 65));
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Credits.unity");
    }
}
