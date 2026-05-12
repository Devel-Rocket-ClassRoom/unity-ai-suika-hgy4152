using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Builds a right-side panel showing each fruit's icon and merge score.
/// Attach to an empty GameObject inside the Canvas; assign fruitConfig in the Inspector.
/// The panel creates its own RectTransform layout and row elements at Start().
/// </summary>
public class FruitScorePanel : MonoBehaviour
{
    [SerializeField] FruitConfig fruitConfig;

    [Header("Layout")]
    [SerializeField] float panelWidth = 110f;
    [SerializeField] float rowHeight  = 42f;
    [SerializeField] float iconSize   = 30f;
    [SerializeField] float padding    = 6f;

    RectTransform _panel;

    void Start()
    {
        if (fruitConfig == null)
        {
            Debug.LogWarning("[FruitScorePanel] fruitConfig not assigned.");
            return;
        }
        BuildPanel();
    }

    void BuildPanel()
    {
        // ── Root panel ─────────────────────────────────────────────────────
        var panelGO = new GameObject("FruitScorePanel_Root");
        panelGO.transform.SetParent(transform, false);

        _panel = panelGO.AddComponent<RectTransform>();
        // Anchor to right side, vertically centered
        _panel.anchorMin = new Vector2(1f, 0.5f);
        _panel.anchorMax = new Vector2(1f, 0.5f);
        _panel.pivot     = new Vector2(1f, 0.5f);

        int count     = fruitConfig.MaxLevel;
        float totalH  = count * rowHeight + padding * 2f;
        _panel.sizeDelta = new Vector2(panelWidth, totalH);
        _panel.anchoredPosition = new Vector2(-4f, 0f);

        // Background image
        var bg = panelGO.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.45f);

        // Title label
        AddTitleLabel(panelGO.transform, totalH);

        // Rows (level 11 → 1, highest at top)
        for (int i = count; i >= 1; i--)
        {
            int rowIndex = count - i;
            float y = totalH * 0.5f - padding - rowHeight * rowIndex - rowHeight * 0.5f;
            AddRow(panelGO.transform, i, y);
        }
    }

    void AddTitleLabel(Transform parent, float panelHeight)
    {
        var go = new GameObject("Title");
        go.transform.SetParent(parent, false);

        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(1f, 1f);
        rt.pivot            = new Vector2(0.5f, 1f);
        rt.sizeDelta        = new Vector2(0f, 20f);
        rt.anchoredPosition = Vector2.zero;

        var txt = go.AddComponent<TextMeshProUGUI>();
        txt.text      = "점수표";
        txt.fontSize  = 11f;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color     = new Color(1f, 0.9f, 0.6f, 1f);
        txt.fontStyle = FontStyles.Bold;
    }

    void AddRow(Transform parent, int level, float centeredY)
    {
        var entry = fruitConfig.Get(level);
        int score = level * (level + 1) / 2;

        // Row container
        var rowGO = new GameObject($"Row_Lv{level}");
        rowGO.transform.SetParent(parent, false);

        var rt = rowGO.AddComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 0.5f);
        rt.anchorMax        = new Vector2(1f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.sizeDelta        = new Vector2(0f, rowHeight - 2f);
        rt.anchoredPosition = new Vector2(0f, centeredY);

        // ── Icon ────────────────────────────────────────────────────────────
        var iconGO = new GameObject("Icon");
        iconGO.transform.SetParent(rowGO.transform, false);

        var iconRT = iconGO.AddComponent<RectTransform>();
        iconRT.anchorMin        = new Vector2(0f, 0.5f);
        iconRT.anchorMax        = new Vector2(0f, 0.5f);
        iconRT.pivot            = new Vector2(0f, 0.5f);
        iconRT.sizeDelta        = new Vector2(iconSize, iconSize);
        iconRT.anchoredPosition = new Vector2(padding, 0f);

        var iconImg = iconGO.AddComponent<Image>();
        iconImg.sprite = entry.sprite != null ? entry.sprite : SpriteFactory.GetCircle();
        iconImg.color  = entry.color != Color.clear ? entry.color : Color.white;
        iconImg.preserveAspect = true;

        // ── Name + Score text ───────────────────────────────────────────────
        var textGO = new GameObject("Label");
        textGO.transform.SetParent(rowGO.transform, false);

        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin        = new Vector2(0f, 0f);
        textRT.anchorMax        = new Vector2(1f, 1f);
        textRT.pivot            = new Vector2(0.5f, 0.5f);
        float leftOffset = padding + iconSize + 4f;
        textRT.offsetMin        = new Vector2(leftOffset, 0f);
        textRT.offsetMax        = new Vector2(-padding, 0f);

        var txt = textGO.AddComponent<TextMeshProUGUI>();
        string name = string.IsNullOrEmpty(entry.fruitName) ? $"Lv.{level}" : entry.fruitName;
        txt.text      = $"{name}\n<size=9><color=#FFD97D>{score}pt</color></size>";
        txt.fontSize  = 10f;
        txt.alignment = TextAlignmentOptions.MidlineLeft;
        txt.color     = Color.white;
    }
}
