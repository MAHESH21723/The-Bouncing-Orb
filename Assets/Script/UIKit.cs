using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Small helpers that build the game's interface from code (canvas, panels, text, buttons).
public static class UIKit
{
    public static readonly Color Cream = new Color(1f, 0.96f, 0.86f);
    public static readonly Color Accent = new Color(0.95f, 0.55f, 0.15f);
    public static readonly Color PanelColor = new Color(0.08f, 0.1f, 0.14f, 0.88f);

    public static Transform CreateCanvas(string name)
    {
        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
        var canvasObject = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return canvasObject.transform;
    }

    public static GameObject Panel(string name, Transform parent, Vector2 size)
    {
        var image = Image(name, parent, PanelColor);
        var rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        return image.gameObject;
    }

    public static GameObject Group(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        Stretch((RectTransform)go.transform);
        return go;
    }

    public static Image Image(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var image = go.GetComponent<Image>();
        image.color = color;
        return image;
    }

    public static TextMeshProUGUI Text(string name, Transform parent, string text, float size, TextAlignmentOptions align)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var label = go.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.alignment = align;
        label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }

    // A heading + body text + button, the layout used by the title, win and lose screens.
    public static TextMeshProUGUI Screen(Transform parent, GameObject panel, string heading, Color headingColor, out TextMeshProUGUI body, string button, UnityAction onClick)
    {
        var size = ((RectTransform)panel.transform).sizeDelta;
        var head = Text("Heading", panel.transform, heading, 88, TextAlignmentOptions.Center);
        head.fontStyle = FontStyles.Bold;
        head.color = headingColor;
        Place(head.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -45), new Vector2(size.x - 60, 120));
        body = Text("Body", panel.transform, "", 36, TextAlignmentOptions.Top);
        body.color = new Color(1f, 0.93f, 0.82f, 0.95f);
        Place(body.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -175), new Vector2(size.x - 120, size.y - 340));
        Button(button, panel.transform, button, new Vector2(0, 55), onClick);
        return head;
    }

    public static void Button(string name, Transform parent, string text, Vector2 bottomOffset, UnityAction onClick)
    {
        var image = Image(name, parent, Accent);
        var rect = image.rectTransform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.sizeDelta = new Vector2(380, 100);
        rect.anchoredPosition = bottomOffset;
        image.gameObject.AddComponent<Button>().onClick.AddListener(onClick);
        var label = Text("Label", image.transform, text, 48, TextAlignmentOptions.Center);
        label.fontStyle = FontStyles.Bold;
        Stretch(label.rectTransform);
    }

    public static TextMeshProUGUI HudText(string name, Transform parent, Vector2 anchor, Vector2 position, TextAlignmentOptions align)
    {
        var label = Text(name, parent, "", 44, align);
        Place(label.rectTransform, anchor, position, new Vector2(700, 64));
        label.outlineWidth = 0.18f;
        label.outlineColor = new Color32(10, 12, 18, 255);
        return label;
    }

    public static void Place(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
    {
        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    public static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
}
