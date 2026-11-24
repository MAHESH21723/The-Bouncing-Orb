using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Runs the level system: title, level select, playing, pause, level complete and level failed.
// Progress (stars per level) is saved with PlayerPrefs; finishing a level unlocks the next one.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public bool IsPlaying { get; private set; }

    public PlayerOrbController orb;
    public LevelBuilder builder;
    public CameraFollow cameraFollow;

    private int currentLevel;
    private Sprite starSprite;
    private Sprite arrowSprite;

    private Transform ui;
    private GameObject titlePanel, levelPanel, hud, pausePanel, completePanel, failedPanel, touchControls;
    private TextMeshProUGUI levelText, scoreText, toastText, completeBody, failedBody;
    private Image progressFill, hurtFlash;
    private Image[] hudStars = new Image[3];
    private Image[] completeStars = new Image[3];
    private Button nextButton;
    private readonly System.Collections.Generic.List<GameObject> levelButtons = new System.Collections.Generic.List<GameObject>();

    private static readonly Color StarOn = new Color(1f, 0.82f, 0.2f);
    private static readonly Color StarOff = new Color(1f, 1f, 1f, 0.18f);
    private static readonly Color Green = new Color(0.3f, 0.72f, 0.36f);
    private static readonly Color Blue = new Color(0.25f, 0.5f, 0.85f);
    private static readonly Color Grey = new Color(0.35f, 0.38f, 0.45f);

    private int Levels => LevelData.All.Length;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        starSprite = MakeStarSprite();
        arrowSprite = MakeArrowSprite();
        BuildInterface();
        ScoreManager.Instance.ScoreChanged += OnScoreChanged;
        builder.Build(0);
        orb.ResetAt(builder.StartPosition);
        cameraFollow.Snap();
        ShowOnly(titlePanel);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
        if (!IsPlaying)
        {
            if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                if (titlePanel.activeSelf) ShowLevelSelect();
                else if (completePanel.activeSelf) NextLevel();
                else if (failedPanel.activeSelf) StartLevel(currentLevel);
            }
            return;
        }

        float progress = Mathf.Clamp01(orb.transform.position.x / builder.FinishX);
        progressFill.rectTransform.anchorMax = new Vector2(progress, 1f);

        if (orb.transform.position.y < -4f) Fail("You fell into a pit!");
        else if (orb.transform.position.x >= builder.FinishX) Complete();
    }

    // ---------- Level flow ----------

    public void StartLevel(int index)
    {
        Time.timeScale = 1f;
        currentLevel = index;
        builder.Build(index);
        orb.ResetAt(builder.StartPosition);
        cameraFollow.Snap();
        ScoreManager.Instance.ResetScore();
        levelText.text = "Level " + (index + 1);
        ShowOnly(hud);
        touchControls.SetActive(UseTouchControls());
        IsPlaying = true;
        StartCoroutine(Toast("Level " + (index + 1) + "  -  reach the flag!", 1.6f));
    }

    private void Complete()
    {
        IsPlaying = false;
        ClearTouch();
        int score = ScoreManager.Instance.Score;
        int stars = StarsFor(currentLevel, score);
        int best = PlayerPrefs.GetInt(StarKey(currentLevel), 0);
        if (stars > best) PlayerPrefs.SetInt(StarKey(currentLevel), stars);
        PlayerPrefs.Save();

        for (int i = 0; i < 3; i++) completeStars[i].color = i < stars ? StarOn : StarOff;
        int target = LevelData.All[currentLevel].targetBounces;
        completeBody.text = "Bounces  <b>" + score + "</b>\n<size=75%>2 stars at " + target + "   3 stars at " + ThreeStar(target) + "</size>";
        bool last = currentLevel >= Levels - 1;
        nextButton.GetComponentInChildren<TextMeshProUGUI>().text = last ? "Levels" : "Next";
        ShowOnly(completePanel);
    }

    private void Fail(string reason)
    {
        IsPlaying = false;
        ClearTouch();
        failedBody.text = reason + "\n<size=80%>Bounces  " + ScoreManager.Instance.Score + "</size>";
        ShowOnly(failedPanel);
    }

    public void OnHitObstacle(Transform obstacle)
    {
        StartCoroutine(FlashHurt());
        StartCoroutine(Toast("Hit a red block - bounces reset!", 1.1f));
    }

    private void NextLevel()
    {
        if (currentLevel >= Levels - 1) ShowLevelSelect();
        else StartLevel(currentLevel + 1);
    }

    private void Pause()
    {
        if (!IsPlaying) return;
        IsPlaying = false;
        ClearTouch();
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    private void Resume()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        IsPlaying = true;
    }

    private void OnBack()
    {
        if (IsPlaying) Pause();
        else if (pausePanel.activeSelf) Resume();
        else if (levelPanel.activeSelf) ShowOnly(titlePanel);
        else if (completePanel.activeSelf || failedPanel.activeSelf) ShowLevelSelect();
    }

    private void ShowLevelSelect()
    {
        Time.timeScale = 1f;
        IsPlaying = false;
        RefreshLevelButtons();
        ShowOnly(levelPanel);
    }

    private void ShowOnly(GameObject panel)
    {
        foreach (var p in new[] { titlePanel, levelPanel, hud, pausePanel, completePanel, failedPanel })
            p.SetActive(p == panel);
    }

    private void OnScoreChanged(int score)
    {
        int target = LevelData.All[currentLevel].targetBounces;
        scoreText.text = "Bounces  <b>" + score + "</b>";
        hudStars[0].color = StarOn;
        hudStars[1].color = score >= target ? StarOn : StarOff;
        hudStars[2].color = score >= ThreeStar(target) ? StarOn : StarOff;
    }

    private static int ThreeStar(int target) => Mathf.CeilToInt(target * 1.5f);

    private static int StarsFor(int level, int score)
    {
        int target = LevelData.All[level].targetBounces;
        return score >= ThreeStar(target) ? 3 : score >= target ? 2 : 1;
    }

    private static string StarKey(int level) => "Stars_L" + (level + 1);

    private bool IsUnlocked(int level) => level == 0 || PlayerPrefs.GetInt(StarKey(level - 1), 0) > 0;

    private static bool UseTouchControls() => Application.isMobilePlatform || Input.touchSupported;

    private void ClearTouch()
    {
        PlayerOrbController.TouchHorizontal = 0f;
        PlayerOrbController.TouchJumpPressed = false;
    }

    private IEnumerator FlashHurt()
    {
        for (float t = 0f; t < 0.35f; t += Time.unscaledDeltaTime)
        {
            hurtFlash.color = new Color(0.95f, 0.15f, 0.1f, 0.3f * (1f - t / 0.35f));
            yield return null;
        }
        hurtFlash.color = Color.clear;
    }

    private IEnumerator Toast(string message, float seconds)
    {
        toastText.text = message;
        toastText.alpha = 1f;
        yield return new WaitForSeconds(seconds);
        for (float t = 0f; t < 0.4f; t += Time.deltaTime)
        {
            toastText.alpha = 1f - t / 0.4f;
            yield return null;
        }
        toastText.alpha = 0f;
    }

    // ---------- Interface ----------

    private void BuildInterface()
    {
        ui = UIKit.CreateCanvas("Game UI");

        hurtFlash = UIKit.Image("Hurt Flash", ui, Color.clear);
        hurtFlash.raycastTarget = false;
        UIKit.Stretch(hurtFlash.rectTransform);

        // HUD
        hud = UIKit.Group("HUD", ui);
        levelText = UIKit.HudText("Level", hud.transform, new Vector2(0, 1), new Vector2(40, -28), TextAlignmentOptions.Left);
        scoreText = UIKit.HudText("Score", hud.transform, new Vector2(0, 1), new Vector2(40, -88), TextAlignmentOptions.Left);
        for (int i = 0; i < 3; i++)
        {
            hudStars[i] = Icon("Star", hud.transform, starSprite, new Vector2(0, 1), new Vector2(330 + i * 54, -96), 48, StarOff);
        }
        var barBack = UIKit.Image("Progress", hud.transform, new Color(0f, 0f, 0f, 0.35f));
        UIKit.Place(barBack.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -44), new Vector2(620, 22));
        progressFill = UIKit.Image("Fill", barBack.transform, new Color(1f, 0.82f, 0.2f));
        progressFill.rectTransform.anchorMin = Vector2.zero;
        progressFill.rectTransform.anchorMax = new Vector2(0f, 1f);
        progressFill.rectTransform.offsetMin = progressFill.rectTransform.offsetMax = Vector2.zero;
        var flagLabel = UIKit.Text("Flag", barBack.transform, "FINISH", 22, TextAlignmentOptions.Right);
        UIKit.Place(flagLabel.rectTransform, new Vector2(1, 0.5f), new Vector2(0, 0), new Vector2(120, 22));
        flagLabel.fontStyle = FontStyles.Bold;
        flagLabel.outlineWidth = 0.25f;
        flagLabel.outlineColor = new Color32(10, 12, 18, 255);
        flagLabel.rectTransform.anchoredPosition = new Vector2(130, 0);
        Button(hud.transform, "II", new Vector2(1, 1), new Vector2(-40, -30), new Vector2(96, 96), new Color(0f, 0f, 0f, 0.4f), Pause);
        toastText = UIKit.Text("Toast", hud.transform, "", 42, TextAlignmentOptions.Center);
        UIKit.Place(toastText.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -110), new Vector2(1200, 70));
        toastText.outlineWidth = 0.2f;
        toastText.outlineColor = new Color32(10, 12, 18, 255);
        BuildTouchControls();

        // Title
        titlePanel = UIKit.Panel("Title", ui, new Vector2(1000, 620));
        UIKit.Screen(ui, titlePanel, "Bouncing Orb", new Color(1f, 0.72f, 0.35f), out var intro, "Play", ShowLevelSelect);
        intro.text = "Roll and jump to the flag. Every bounce on the ground scores a point - but touching a red block resets your bounces to zero!\n\n" +
                     "<color=#ffd38a><b>A / D</b> or arrows  roll      <b>Space</b>  jump</color>\n<size=85%>On a phone, use the buttons on screen.</size>";
        var credit = UIKit.Text("Credit", titlePanel.transform, "Made by <b>Maheshwaran</b>", 40, TextAlignmentOptions.Center);
        credit.color = UIKit.Cream;
        credit.outlineWidth = 0.22f;
        credit.outlineColor = new Color32(10, 12, 18, 255);
        UIKit.Place(credit.rectTransform, new Vector2(0.5f, 0), new Vector2(0, -70), new Vector2(900, 60));
        var version = UIKit.Text("Version", titlePanel.transform, "v" + Application.version, 28, TextAlignmentOptions.Right);
        version.color = new Color(1f, 1f, 1f, 0.55f);
        UIKit.Place(version.rectTransform, new Vector2(1, 0), new Vector2(-24, 16), new Vector2(200, 40));

        // Level select
        levelPanel = UIKit.Panel("Levels", ui, new Vector2(1240, 760));
        var choose = UIKit.Text("Heading", levelPanel.transform, "Choose a level", 76, TextAlignmentOptions.Center);
        choose.fontStyle = FontStyles.Bold;
        choose.color = UIKit.Cream;
        UIKit.Place(choose.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -36), new Vector2(1100, 100));
        for (int i = 0; i < Levels; i++)
        {
            int level = i;
            int col = i % 5, row = i / 5;
            var b = Button(levelPanel.transform, (i + 1).ToString(), new Vector2(0.5f, 1),
                new Vector2(-440 + col * 220, -170 - row * 230), new Vector2(180, 180), Blue, () => StartLevel(level));
            b.GetComponentInChildren<TextMeshProUGUI>().fontSize = 64;
            b.GetComponentInChildren<TextMeshProUGUI>().rectTransform.offsetMin = new Vector2(0, 40);
            for (int s = 0; s < 3; s++)
                Icon("Star", b.transform, starSprite, new Vector2(0.5f, 0), new Vector2(-46 + s * 46, 18), 40, StarOff);
            levelButtons.Add(b);
        }
        Button(levelPanel.transform, "Back", new Vector2(0.5f, 0), new Vector2(0, 40), new Vector2(300, 90), Grey, () => ShowOnly(titlePanel));
        var byline = UIKit.Text("Credit", levelPanel.transform, "by Maheshwaran", 30, TextAlignmentOptions.Right);
        byline.color = new Color(1f, 0.93f, 0.82f, 0.7f);
        UIKit.Place(byline.rectTransform, new Vector2(1, 0), new Vector2(-30, 24), new Vector2(400, 44));

        // Pause
        pausePanel = UIKit.Panel("Pause", ui, new Vector2(760, 560));
        Heading(pausePanel, "Paused", UIKit.Cream);
        Button(pausePanel.transform, "Resume", new Vector2(0.5f, 0.5f), new Vector2(0, 50), new Vector2(380, 96), Green, Resume);
        Button(pausePanel.transform, "Restart", new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(380, 96), UIKit.Accent, () => StartLevel(currentLevel));
        Button(pausePanel.transform, "Levels", new Vector2(0.5f, 0.5f), new Vector2(0, -170), new Vector2(380, 96), Grey, ShowLevelSelect);

        // Level complete
        completePanel = UIKit.Panel("Complete", ui, new Vector2(900, 640));
        Heading(completePanel, "Level complete!", new Color(0.55f, 0.92f, 0.45f));
        for (int i = 0; i < 3; i++)
            completeStars[i] = Icon("Star", completePanel.transform, starSprite, new Vector2(0.5f, 1), new Vector2(-120 + i * 120, -170), i == 1 ? 120 : 100, StarOff);
        completeBody = UIKit.Text("Body", completePanel.transform, "", 44, TextAlignmentOptions.Center);
        completeBody.color = UIKit.Cream;
        UIKit.Place(completeBody.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -300), new Vector2(820, 130));
        nextButton = Button(completePanel.transform, "Next", new Vector2(0.5f, 0), new Vector2(290, 50), new Vector2(250, 96), Green, NextLevel).GetComponent<Button>();
        Button(completePanel.transform, "Retry", new Vector2(0.5f, 0), new Vector2(0, 50), new Vector2(250, 96), UIKit.Accent, () => StartLevel(currentLevel));
        Button(completePanel.transform, "Levels", new Vector2(0.5f, 0), new Vector2(-290, 50), new Vector2(250, 96), Grey, ShowLevelSelect);

        // Level failed
        failedPanel = UIKit.Panel("Failed", ui, new Vector2(860, 520));
        Heading(failedPanel, "Oops!", new Color(1f, 0.5f, 0.42f));
        failedBody = UIKit.Text("Body", failedPanel.transform, "", 46, TextAlignmentOptions.Center);
        failedBody.color = UIKit.Cream;
        UIKit.Place(failedBody.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -180), new Vector2(800, 140));
        Button(failedPanel.transform, "Retry", new Vector2(0.5f, 0), new Vector2(150, 50), new Vector2(320, 96), UIKit.Accent, () => StartLevel(currentLevel));
        Button(failedPanel.transform, "Levels", new Vector2(0.5f, 0), new Vector2(-190, 50), new Vector2(300, 96), Grey, ShowLevelSelect);
    }

    private void BuildTouchControls()
    {
        touchControls = UIKit.Group("Touch Controls", hud.transform);
        var left = HoldButton("Left", new Vector2(0, 0), new Vector2(60, 50), arrowSprite, 180f);
        var right = HoldButton("Right", new Vector2(0, 0), new Vector2(290, 50), arrowSprite, 0f);
        AddHold(left, () => PlayerOrbController.TouchHorizontal = -1f, () => { if (PlayerOrbController.TouchHorizontal < 0f) PlayerOrbController.TouchHorizontal = 0f; });
        AddHold(right, () => PlayerOrbController.TouchHorizontal = 1f, () => { if (PlayerOrbController.TouchHorizontal > 0f) PlayerOrbController.TouchHorizontal = 0f; });

        var jump = UIKit.Image("Jump", touchControls.transform, new Color(1f, 1f, 1f, 0.28f));
        jump.sprite = builder.circleSprite;
        UIKit.Place(jump.rectTransform, new Vector2(1, 0), new Vector2(-60, 50), new Vector2(230, 230));
        var label = UIKit.Text("Label", jump.transform, "JUMP", 44, TextAlignmentOptions.Center);
        label.fontStyle = FontStyles.Bold;
        UIKit.Stretch(label.rectTransform);
        AddHold(jump.gameObject, () => PlayerOrbController.TouchJumpPressed = true, null);
        touchControls.SetActive(false);
    }

    private GameObject HoldButton(string name, Vector2 anchor, Vector2 pos, Sprite sprite, float angle)
    {
        var back = UIKit.Image(name, touchControls.transform, new Color(1f, 1f, 1f, 0.28f));
        back.sprite = builder.circleSprite;
        UIKit.Place(back.rectTransform, anchor, pos, new Vector2(200, 200));
        var arrow = UIKit.Image("Arrow", back.transform, new Color(1f, 1f, 1f, 0.9f));
        arrow.sprite = sprite;
        arrow.raycastTarget = false;
        arrow.rectTransform.sizeDelta = new Vector2(90, 90);
        arrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
        return back.gameObject;
    }

    private static void AddHold(GameObject go, System.Action down, System.Action up)
    {
        var trigger = go.AddComponent<EventTrigger>();
        void Add(EventTriggerType type, System.Action action)
        {
            if (action == null) return;
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }
        Add(EventTriggerType.PointerDown, down);
        Add(EventTriggerType.PointerUp, up);
        Add(EventTriggerType.PointerExit, up);
    }

    private void RefreshLevelButtons()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            var b = levelButtons[i];
            bool unlocked = IsUnlocked(i);
            int stars = PlayerPrefs.GetInt(StarKey(i), 0);
            b.GetComponent<Button>().interactable = unlocked;
            b.GetComponent<Image>().color = unlocked ? Blue : Grey;
            var icons = b.GetComponentsInChildren<Image>();
            int s = 0;
            foreach (var icon in icons)
            {
                if (icon.gameObject == b) continue;
                icon.color = s < stars ? StarOn : (unlocked ? StarOff : new Color(1f, 1f, 1f, 0.08f));
                s++;
            }
            b.GetComponentInChildren<TextMeshProUGUI>().alpha = unlocked ? 1f : 0.35f;
        }
    }

    private static void Heading(GameObject panel, string text, Color color)
    {
        var head = UIKit.Text("Heading", panel.transform, text, 84, TextAlignmentOptions.Center);
        head.fontStyle = FontStyles.Bold;
        head.color = color;
        var width = ((RectTransform)panel.transform).sizeDelta.x;
        UIKit.Place(head.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(width - 60, 110));
    }

    private static GameObject Button(Transform parent, string text, Vector2 anchor, Vector2 pos, Vector2 size, Color color, UnityEngine.Events.UnityAction onClick)
    {
        var image = UIKit.Image(text + " Button", parent, color);
        UIKit.Place(image.rectTransform, anchor, pos, size);
        image.rectTransform.pivot = new Vector2(0.5f, anchor.y == 1 ? 1f : anchor.y == 0 ? 0f : 0.5f);
        if (anchor.x == 1) image.rectTransform.pivot = new Vector2(1f, image.rectTransform.pivot.y);
        var button = image.gameObject.AddComponent<Button>();
        button.onClick.AddListener(onClick);
        var label = UIKit.Text("Label", image.transform, text, 46, TextAlignmentOptions.Center);
        label.fontStyle = FontStyles.Bold;
        UIKit.Stretch(label.rectTransform);
        return image.gameObject;
    }

    private static Image Icon(string name, Transform parent, Sprite sprite, Vector2 anchor, Vector2 pos, float size, Color color)
    {
        var image = UIKit.Image(name, parent, color);
        image.sprite = sprite;
        image.raycastTarget = false;
        UIKit.Place(image.rectTransform, anchor, pos, new Vector2(size, size));
        image.rectTransform.pivot = new Vector2(0.5f, anchor.y);
        return image;
    }

    // A five-pointed star, drawn into a texture so no image file is needed.
    private static Sprite MakeStarSprite()
    {
        const int n = 128;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        var points = new Vector2[10];
        for (int i = 0; i < 10; i++)
        {
            float r = i % 2 == 0 ? 0.48f : 0.2f;
            float a = Mathf.PI / 2f + i * Mathf.PI / 5f;
            points[i] = new Vector2(0.5f + Mathf.Cos(a) * r, 0.5f + Mathf.Sin(a) * r - 0.02f);
        }
        return PolygonSprite(tex, n, points);
    }

    private static Sprite MakeArrowSprite()
    {
        const int n = 64;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        return PolygonSprite(tex, n, new[] { new Vector2(0.2f, 0.1f), new Vector2(0.9f, 0.5f), new Vector2(0.2f, 0.9f) });
    }

    private static Sprite PolygonSprite(Texture2D tex, int n, Vector2[] poly)
    {
        var pixels = new Color32[n * n];
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                // 4 samples per pixel for smooth edges.
                int hits = 0;
                for (int s = 0; s < 4; s++)
                {
                    var p = new Vector2((x + 0.25f + 0.5f * (s % 2)) / n, (y + 0.25f + 0.5f * (s / 2)) / n);
                    if (Inside(poly, p)) hits++;
                }
                pixels[y * n + x] = new Color32(255, 255, 255, (byte)(hits * 255 / 4));
            }
        }
        tex.SetPixels32(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
    }

    private static bool Inside(Vector2[] poly, Vector2 p)
    {
        bool inside = false;
        for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
        {
            if ((poly[i].y > p.y) != (poly[j].y > p.y) &&
                p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x)
                inside = !inside;
        }
        return inside;
    }
}
