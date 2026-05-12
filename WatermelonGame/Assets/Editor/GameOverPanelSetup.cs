using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// WatermelonGame > Rebuild Game Over Panel 을 실행하면
/// 현재 열려 있는 씬의 GameOverPanel 을 새 디자인으로 교체합니다.
/// </summary>
public static class GameOverPanelSetup
{
    [MenuItem("WatermelonGame/🎯 Rebuild Game Over Panel")]
    static void RebuildPanel()
    {
        var gameUI = Object.FindFirstObjectByType<GameUI>();
        if (gameUI == null)
        {
            EditorUtility.DisplayDialog(
                "오류",
                "씬에 GameUI 컴포넌트가 없습니다.\nFull Setup 을 먼저 실행하세요.",
                "확인"
            );
            return;
        }

        var canvasGO = gameUI.gameObject;

        // 기존 패널 제거
        var old = canvasGO.transform.Find("GameOverPanel");
        if (old != null)
            Undo.DestroyObjectImmediate(old.gameObject);

        // 새 패널 생성
        var panel = CreateGameOverPanel(canvasGO);

        // GameUI 직렬화 필드 연결
        var so = new SerializedObject(gameUI);
        so.FindProperty("gameOverPanel").objectReferenceValue = panel;
        so.FindProperty("finalScoreText").objectReferenceValue = panel
            .transform.Find("Card/FinalScoreText")
            ?.GetComponent<TMP_Text>();
        so.FindProperty("finalBestText").objectReferenceValue = panel
            .transform.Find("Card/FinalBestText")
            ?.GetComponent<TMP_Text>();
        so.ApplyModifiedProperties();

        // 버튼 콜백 연결
        WireButton(
            panel.transform.Find("Card/RestartButton")?.gameObject,
            gameUI,
            nameof(gameUI.OnRestartButton)
        );
        WireButton(
            panel.transform.Find("Card/TitleButton")?.gameObject,
            gameUI,
            nameof(gameUI.OnRestartButton)
        );

        EditorSceneManager.MarkSceneDirty(gameUI.gameObject.scene);
        Debug.Log("✅ Game Over Panel 재생성 완료! Ctrl+S 로 씬을 저장하세요.");
    }

    // ── 패널 계층 생성 (WatermelonSetup 에서도 재사용) ─────────────────────
    public static GameObject CreateGameOverPanel(GameObject canvas)
    {
        // 전체 화면 어두운 오버레이
        // Image 추가 시 RequireComponent 로 RectTransform 이 자동 생성됨
        var panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvas.transform, false);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.82f);
        var panelRT = (RectTransform)panel.transform;
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = panelRT.offsetMax = Vector2.zero;

        // 중앙 카드
        var card = new GameObject("Card");
        card.transform.SetParent(panel.transform, false);
        card.AddComponent<Image>().color = new Color(0.11f, 0.08f, 0.05f, 0.97f);
        var cardRT = (RectTransform)card.transform;
        cardRT.anchorMin = new Vector2(0.08f, 0.10f);
        cardRT.anchorMax = new Vector2(0.92f, 0.90f);
        cardRT.offsetMin = cardRT.offsetMax = Vector2.zero;

        // 제목 "GAME OVER"
        Pin(
            MakeTMP(card, "TitleText", "GAME OVER", 44, Hex("FF5252"), FontStyles.Bold),
            0.5f,
            0.87f,
            320,
            62
        );

        // 구분선 (Image 로 얇은 선)
        var divGO = new GameObject("Divider");
        divGO.transform.SetParent(card.transform, false);
        divGO.AddComponent<Image>().color = Hex("FF5252");
        var divRT = (RectTransform)divGO.transform;
        divRT.anchorMin = new Vector2(0.08f, 0.77f);
        divRT.anchorMax = new Vector2(0.92f, 0.77f);
        divRT.sizeDelta = new Vector2(0f, 2f);
        divRT.anchoredPosition = Vector2.zero;

        // 점수 라벨
        Pin(
            MakeTMP(card, "ScoreLabel", "SCORE", 15, Hex("AAAAAA"), FontStyles.Normal),
            0.5f,
            0.68f,
            260,
            26
        );

        // 최종 점수 (GameUI 에서 참조)
        Pin(
            MakeTMP(card, "FinalScoreText", "0", 52, Color.white, FontStyles.Bold),
            0.5f,
            0.56f,
            290,
            68
        );

        // 최고 기록 라벨
        Pin(
            MakeTMP(card, "BestLabel", "최고 기록", 14, Hex("AAAAAA"), FontStyles.Normal),
            0.5f,
            0.44f,
            260,
            24
        );

        // 최고 기록 값 (GameUI 에서 참조)
        Pin(
            MakeTMP(card, "FinalBestText", "BEST 0", 22, Hex("AAFFAA"), FontStyles.Normal),
            0.5f,
            0.35f,
            260,
            36
        );

        // 다시 하기 버튼
        Pin(
            MakeButton(card, "RestartButton", "다시 하기", 22, Hex("2E7D32")),
            0.5f,
            0.205f,
            210,
            58
        );

        // 처음으로 버튼
        Pin(MakeButton(card, "TitleButton", "처음으로", 20, Hex("4E342E")), 0.5f, 0.095f, 210, 50);

        panel.SetActive(false);
        return panel;
    }

    // ── 헬퍼 ──────────────────────────────────────────────────────────────
    // TextMeshProUGUI 의 RequireComponent(RectTransform) 으로 RT 자동 생성
    static GameObject MakeTMP(
        GameObject parent,
        string name,
        string text,
        int size,
        Color color,
        FontStyles style
    )
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    // Image 의 RequireComponent(RectTransform) 으로 RT 자동 생성
    static GameObject MakeButton(
        GameObject parent,
        string name,
        string label,
        int fontSize,
        Color bgColor
    )
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<Image>().color = bgColor;
        go.AddComponent<Button>();

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        var tRT = (RectTransform)textGO.transform;
        tRT.anchorMin = Vector2.zero;
        tRT.anchorMax = Vector2.one;
        tRT.offsetMin = tRT.offsetMax = Vector2.zero;

        return go;
    }

    // Image/TMP 추가 후 transform 이 이미 RectTransform 으로 대체되어 있음
    static void Pin(GameObject go, float ax, float ay, float w, float h)
    {
        var rt = (RectTransform)go.transform;
        rt.anchorMin = new Vector2(ax, ay);
        rt.anchorMax = new Vector2(ax, ay);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(w, h);
    }

    static void WireButton(GameObject btnGO, GameUI target, string methodName)
    {
        if (btnGO == null)
            return;
        var btn = btnGO.GetComponent<Button>();
        if (btn == null)
            return;
        btn.onClick.RemoveAllListeners();
        var action = (UnityAction)
            System.Delegate.CreateDelegate(typeof(UnityAction), target, methodName);
        UnityEventTools.AddPersistentListener(btn.onClick, action);
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
