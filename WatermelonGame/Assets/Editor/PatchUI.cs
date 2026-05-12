using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// WatermelonGame > 🔧 Add Missing UI Elements
/// Full Setup 이후 씬에서 누락된 TitlePanel / DangerFillBar 를 추가하고
/// GameUI 필드를 자동으로 연결합니다.
/// </summary>
public static class PatchUI
{
    [MenuItem("WatermelonGame/🔧 Add Missing UI Elements (TitlePanel + DangerFill)")]
    static void Patch()
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
        var so = new SerializedObject(gameUI);
        bool dirty = false;

        // ── 1. TitlePanel ──────────────────────────────────────────────────
        if (canvasGO.transform.Find("TitlePanel") == null)
        {
            var panel = TitlePanelSetup.CreateTitlePanel(canvasGO);
            var titleUI = panel.GetComponent<TitleUI>();

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

            // GameUI.titlePanel 연결
            so.FindProperty("titlePanel").objectReferenceValue = panel;
            panel.transform.SetAsLastSibling();
            dirty = true;
            Debug.Log("✅ TitlePanel 추가됨");
        }
        else
        {
            // 이미 있으면 GameUI 참조가 비어 있는 경우만 보정
            if (so.FindProperty("titlePanel").objectReferenceValue == null)
            {
                so.FindProperty("titlePanel").objectReferenceValue = canvasGO
                    .transform.Find("TitlePanel")
                    ?.gameObject;
                dirty = true;
            }
        }

        // ── 2. DangerFillBar ───────────────────────────────────────────────
        if (so.FindProperty("dangerFill").objectReferenceValue == null)
        {
            var existing = canvasGO.transform.Find("DangerFillBar");
            // 기존 오브젝트가 Image only(구버전)면 제거 후 재생성
            if (existing != null && existing.GetComponent<Slider>() == null)
            {
                Undo.DestroyObjectImmediate(existing.gameObject);
                existing = null;
            }

            Slider slider;
            if (existing != null)
            {
                slider = existing.GetComponent<Slider>();
            }
            else
            {
                var go = CreateDangerFillBar(canvasGO);
                slider = go.GetComponent<Slider>();
                Debug.Log("✅ DangerFillBar 추가됨");
            }
            so.FindProperty("dangerFill").objectReferenceValue = slider;
            dirty = true;
        }

        if (dirty)
        {
            so.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(gameUI.gameObject.scene);
            Debug.Log("✅ Missing UI 패치 완료! Ctrl+S 로 씬을 저장하세요.");
        }
        else
        {
            Debug.Log("ℹ️ 모든 UI 요소가 이미 존재합니다. 변경 없음.");
        }
    }

    // ── DangerFillBar ──────────────────────────────────────────────────────
    // 위험 타이머(0~1) 를 나타내는 얇은 빨간 수평 Slider.
    // GameUI.Update() 에서 slider.value = DangerZone.DangerProgress 로 구동됨.
    static GameObject CreateDangerFillBar(GameObject canvas)
    {
        var go = new GameObject("DangerFillBar");
        go.transform.SetParent(canvas.transform, false);

        // Image 먼저 추가 → RectTransform 생성
        var bg = go.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.15f, 0.5f);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = new Vector2(0f, 0.866f);
        rt.anchorMax = new Vector2(1f, 0.876f);
        rt.offsetMin = rt.offsetMax = Vector2.zero;

        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(go.transform, false);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = new Color(1f, 0.1f, 0.1f, 0.85f);
        var fillRT = (RectTransform)fillGO.transform;
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;

        var slider = go.AddComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.fillRect = fillRT;

        return go;
    }

    // ── Button wiring ──────────────────────────────────────────────────────
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
}
