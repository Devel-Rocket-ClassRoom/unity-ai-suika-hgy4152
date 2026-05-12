using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// WatermelonGame > Add Title Panel 을 실행하면
/// 현재 씬의 Canvas 에 시작 화면 패널을 추가합니다.
/// </summary>
public static class TitlePanelSetup
{
    [MenuItem("WatermelonGame/🏠 Add Title Panel")]
    static void AddTitlePanel()
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

        // 기존 타이틀 패널 제거
        var old = canvasGO.transform.Find("TitlePanel");
        if (old != null)
            Undo.DestroyObjectImmediate(old.gameObject);

        // 패널 생성
        var panel = CreateTitlePanel(canvasGO);
        var titleUI = panel.GetComponent<TitleUI>();

        // GameUI.titlePanel 연결
        var uiSO = new SerializedObject(gameUI);
        uiSO.FindProperty("titlePanel").objectReferenceValue = panel;
        uiSO.ApplyModifiedProperties();

        // TitleUI.bestScoreText 연결
        var tuSO = new SerializedObject(titleUI);
        tuSO.FindProperty("bestScoreText").objectReferenceValue = panel
            .transform.Find("Card/BestScoreText")
            ?.GetComponent<TMP_Text>();
        tuSO.ApplyModifiedProperties();

        // 시작 버튼 콜백 연결
        WireButton(
            panel.transform.Find("Card/StartButton")?.gameObject,
            titleUI,
            nameof(titleUI.OnStartButton)
        );

        // 타이틀 패널을 최상위로 (게임 오버 패널 위에)
        panel.transform.SetAsLastSibling();

        EditorSceneManager.MarkSceneDirty(gameUI.gameObject.scene);
        Debug.Log("✅ Title Panel 생성 완료! Ctrl+S 로 씬을 저장하세요.");
    }

    // ── 패널 계층 생성 ─────────────────────────────────────────────────────
    public static GameObject CreateTitlePanel(GameObject canvas)
    {
        // 전체 화면 배경 (크림색)
        var panel = new GameObject("TitlePanel");
        panel.transform.SetParent(canvas.transform, false);
        panel.AddComponent<Image>().color = Hex("FFF8E1");
        var panelRT = (RectTransform)panel.transform;
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = panelRT.offsetMax = Vector2.zero;

        // TitleUI 컴포넌트
        panel.AddComponent<TitleUI>();

        // 중앙 카드
        var card = new GameObject("Card");
        card.transform.SetParent(panel.transform, false);
        card.AddComponent<Image>().color = new Color(1f, 0.97f, 0.88f, 0f); // 투명 (배경과 동화)
        var cardRT = (RectTransform)card.transform;
        cardRT.anchorMin = new Vector2(0.08f, 0.15f);
        cardRT.anchorMax = new Vector2(0.92f, 0.92f);
        cardRT.offsetMin = cardRT.offsetMax = Vector2.zero;

        // 게임 제목
        Pin(
            MakeTMP(card, "TitleText", "수박 게임", 56, Hex("1B5E20"), FontStyles.Bold),
            0.5f,
            0.84f,
            340,
            80
        );

        // 부제목
        Pin(
            MakeTMP(
                card,
                "SubtitleText",
                "과일을 합쳐 수박을 만들어요!",
                18,
                Hex("4E342E"),
                FontStyles.Normal
            ),
            0.5f,
            0.72f,
            320,
            30
        );

        // 구분선
        var divGO = new GameObject("Divider");
        divGO.transform.SetParent(card.transform, false);
        divGO.AddComponent<Image>().color = Hex("A5D6A7");
        var divRT = (RectTransform)divGO.transform;
        divRT.anchorMin = new Vector2(0.15f, 0.64f);
        divRT.anchorMax = new Vector2(0.85f, 0.64f);
        divRT.sizeDelta = new Vector2(0f, 2f);
        divRT.anchoredPosition = Vector2.zero;

        // 최고 기록 (TitleUI 에서 갱신)
        Pin(
            MakeTMP(card, "BestScoreText", "최고 기록  0", 20, Hex("795548"), FontStyles.Normal),
            0.5f,
            0.54f,
            300,
            30
        );

        // 게임 시작 버튼
        Pin(
            MakeButton(card, "StartButton", "게임 시작", 24, Hex("2E7D32")),
            0.5f,
            0.37f,
            230,
            64
        );

        // 조작 안내
        Pin(
            MakeTMP(
                card,
                "HowToPlay",
                "마우스로 위치 조절\n클릭 또는 스페이스바로 드롭",
                14,
                Hex("9E9E9E"),
                FontStyles.Normal
            ),
            0.5f,
            0.18f,
            300,
            50
        );

        panel.SetActive(true);
        return panel;
    }

    // ── 헬퍼 ──────────────────────────────────────────────────────────────
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

    static void Pin(GameObject go, float ax, float ay, float w, float h)
    {
        var rt = (RectTransform)go.transform;
        rt.anchorMin = new Vector2(ax, ay);
        rt.anchorMax = new Vector2(ax, ay);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(w, h);
    }

    static void WireButton(GameObject btnGO, TitleUI target, string methodName)
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
