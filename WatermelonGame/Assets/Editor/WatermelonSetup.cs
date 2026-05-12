using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// WatermelonGame > Full Setup 을 실행하면 Game 씬 + FruitConfig + Prefab 이 생성됩니다.
/// 씬은 Game 하나만 사용합니다. 박스/DangerZone 은 씬에 영구 배치됩니다.
/// </summary>
public static class WatermelonSetup
{
    // ── Box / World constants ───────────────────────────────────────────────
    const float BoxHalfW = 3f; // inner half-width
    const float BoxBottom = -5f;
    const float DangerY = 3.5f;
    const float SpawnY = 4.5f;
    const float CamOrtho = 6f;

    // ══════════════════════════════════════════════════════════════════════
    [MenuItem("WatermelonGame/🎮 Full Setup (Run This First)")]
    public static void FullSetup()
    {
        SetupFolders();
        var physMat = CreatePhysicsMaterial();
        var config = CreateFruitConfig();
        var prefab = CreateFruitBasePrefab(physMat);
        BuildGameScene(config, prefab);
        UpdateBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Setup complete! Open Assets/Scenes/Game.unity and press Play.");
    }

    // ══════════════════════════════════════════════════════════════════════
    // 1. Folders
    // ══════════════════════════════════════════════════════════════════════
    static void SetupFolders()
    {
        string[] dirs =
        {
            "Assets/Scripts/Core",
            "Assets/Scripts/Fruit",
            "Assets/Scripts/UI",
            "Assets/Scripts/Utils",
            "Assets/Editor",
            "Assets/Prefabs",
            "Assets/Materials",
            "Assets/Audio",
            "Assets/Scenes",
        };
        foreach (var d in dirs)
            if (!AssetDatabase.IsValidFolder(d))
                Directory.CreateDirectory(d);
        AssetDatabase.Refresh();
    }

    // ══════════════════════════════════════════════════════════════════════
    // 2. Physics Material
    // ══════════════════════════════════════════════════════════════════════
    static PhysicsMaterial2D CreatePhysicsMaterial()
    {
        const string path = "Assets/Materials/FruitPhysics.physicsMaterial2D";
        var mat = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(path);
        if (mat != null)
            return mat;

        mat = new PhysicsMaterial2D("FruitPhysics");
        mat.bounciness = 0.2f;
        mat.friction = 0.5f;
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 3. FruitConfig ScriptableObject
    // ══════════════════════════════════════════════════════════════════════
    static FruitConfig CreateFruitConfig()
    {
        const string path = "Assets/FruitConfig.asset";
        var existing = AssetDatabase.LoadAssetAtPath<FruitConfig>(path);
        if (existing != null)
            return existing;

        var cfg = ScriptableObject.CreateInstance<FruitConfig>();
        cfg.fruits = new FruitConfig.FruitEntry[]
        {
            new()
            {
                fruitName = "체리",
                radius = 0.18f,
                scoreValue = 1,
            },
            new()
            {
                fruitName = "딸기",
                radius = 0.26f,
                scoreValue = 3,
            },
            new()
            {
                fruitName = "포도",
                radius = 0.36f,
                scoreValue = 6,
            },
            new()
            {
                fruitName = "데코퐁",
                radius = 0.48f,
                scoreValue = 10,
            },
            new()
            {
                fruitName = "감",
                radius = 0.62f,
                scoreValue = 15,
            },
            new()
            {
                fruitName = "사과",
                radius = 0.78f,
                scoreValue = 21,
            },
            new()
            {
                fruitName = "배",
                radius = 0.96f,
                scoreValue = 28,
            },
            new()
            {
                fruitName = "복숭아",
                radius = 1.16f,
                scoreValue = 36,
            },
            new()
            {
                fruitName = "파인애플",
                radius = 1.38f,
                scoreValue = 45,
            },
            new()
            {
                fruitName = "멜론",
                radius = 1.62f,
                scoreValue = 55,
            },
            new()
            {
                fruitName = "수박",
                radius = 1.90f,
                scoreValue = 66,
            },
        };
        AssetDatabase.CreateAsset(cfg, path);
        return cfg;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 4. FruitBase Prefab
    // ══════════════════════════════════════════════════════════════════════
    static GameObject CreateFruitBasePrefab(PhysicsMaterial2D physMat)
    {
        const string path = "Assets/Prefabs/FruitBase.prefab";
        var existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        var go = new GameObject("FruitBase");

        var sr = go.AddComponent<SpriteRenderer>();
        sr.color = Color.white;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 3f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.5f;
        col.sharedMaterial = physMat;

        go.AddComponent<Fruit>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ══════════════════════════════════════════════════════════════════════
    // 5. Sprite Assets
    // ══════════════════════════════════════════════════════════════════════
    static Sprite CreateOrLoadSquare()
    {
        const string path = "Assets/Sprites/WhiteSquare.png";
        EnsureFolder("Assets/Sprites");
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null)
            return existing;

        var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var px = new Color[16];
        for (int i = 0; i < 16; i++)
            px[i] = Color.white;
        tex.SetPixels(px);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        ConfigureSprite(path, 4); // 4px → ppu=4 → scale(1,1)=1×1 world unit
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // 속이 빈 테두리 사각형 — 박스 경계선 전용
    static Sprite CreateOrLoadBorderRect()
    {
        const string path = "Assets/Sprites/BoxBorder.png";
        const int res = 64;
        const int border = 5; // 테두리 두께(px)
        EnsureFolder("Assets/Sprites");
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null)
            return existing;

        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        var px = new Color[res * res];
        for (int y = 0; y < res; y++)
        for (int x = 0; x < res; x++)
        {
            bool isBorder = x < border || x >= res - border || y < border || y >= res - border;
            px[y * res + x] = isBorder ? Color.white : Color.clear;
        }
        tex.SetPixels(px);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        ConfigureSprite(path, 64); // 64px → ppu=64 → scale(1,1)=1×1 world unit
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static Sprite CreateOrLoadCircle()
    {
        const string path = "Assets/Sprites/WhiteCircle.png";
        EnsureFolder("Assets/Sprites");
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null)
            return existing;

        const int res = 128;
        float c = res * 0.5f,
            r = c - 1f;
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        var px = new Color[res * res];
        for (int y = 0; y < res; y++)
        for (int x = 0; x < res; x++)
        {
            float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(c, c));
            px[y * res + x] = d <= r ? Color.white : Color.clear;
        }
        tex.SetPixels(px);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        ConfigureSprite(path, 128); // 128px → ppu=128 → scale(1,1)=1×1 world unit
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // pixelsPerUnit = 텍스처 해상도로 맞추면 scale(1,1) 시 1×1 월드유닛
    // → BoxCollider2D size=Vector2.one 과 완전히 일치
    static void ConfigureSprite(string path, int pixelsPerUnit)
    {
        var imp = AssetImporter.GetAtPath(path) as TextureImporter;
        imp.textureType = TextureImporterType.Sprite;
        imp.spriteImportMode = SpriteImportMode.Single;
        imp.alphaIsTransparency = true;
        imp.filterMode = FilterMode.Bilinear;
        imp.spritePixelsPerUnit = pixelsPerUnit;
        imp.SaveAndReimport();
    }

    static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            var parts = path.Split('/');
            var parent = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var full = parent + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(full))
                    AssetDatabase.CreateFolder(parent, parts[i]);
                parent = full;
            }
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // 6. Game Scene
    // ══════════════════════════════════════════════════════════════════════
    static void BuildGameScene(FruitConfig config, GameObject fruitPrefab)
    {
        const string path = "Assets/Scenes/Game.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var squareSprite = CreateOrLoadSquare();
        var circleSprite = CreateOrLoadCircle();
        var borderSprite = CreateOrLoadBorderRect();

        // ── Camera ─────────────────────────────────────────────────────────
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = CamOrtho;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Hex("FFF8E1");
        camGO.transform.position = new Vector3(0, 0, -10);
        camGO.AddComponent<CameraShake>();

        // ── Managers ───────────────────────────────────────────────────────
        var mgrGO = new GameObject("GameRoot");
        mgrGO.AddComponent<GameManager>();
        mgrGO.AddComponent<ScoreManager>();

        // ── FruitSpawner ───────────────────────────────────────────────────
        var spawnerGO = new GameObject("FruitSpawner");
        var spawner = spawnerGO.AddComponent<FruitSpawner>();
        SetPrivateField(spawner, "config", config);
        SetPrivateField(spawner, "fruitBasePrefab", fruitPrefab);
        SetPrivateField(spawner, "spawnY", SpawnY);
        SetPrivateField(spawner, "minX", -2.5f);
        SetPrivateField(spawner, "maxX", 2.5f);

        // Drop guide LineRenderer
        var guideGO = new GameObject("DropGuide");
        guideGO.transform.SetParent(spawnerGO.transform);
        var lr = guideGO.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.04f;
        lr.endWidth = 0.04f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(1, 1, 1, 0.4f);
        lr.endColor = new Color(1, 1, 1, 0.1f);
        lr.useWorldSpace = true;
        SetPrivateField(spawner, "dropGuide", lr);

        // ── Box Container ──────────────────────────────────────────────────
        var containerGO = new GameObject("GameContainer");

        // Box background (크림색 배경)
        float boxH = DangerY - BoxBottom;
        float boxCenterY = (DangerY + BoxBottom) * 0.5f;
        var bgGO = new GameObject("BoxBackground");
        bgGO.transform.SetParent(containerGO.transform);
        bgGO.transform.position = new Vector3(0, boxCenterY, 0.1f);
        bgGO.transform.localScale = new Vector3(BoxHalfW * 2f, boxH, 1f);
        var bgSr = bgGO.AddComponent<SpriteRenderer>();
        bgSr.sprite = squareSprite;
        bgSr.color = new Color(1f, 0.97f, 0.88f, 1f); // #FFF8E1

        // 박스 테두리 — 갈색 속빈 사각형 하나로 경계 표시
        var borderGO = new GameObject("BoxBorder");
        borderGO.transform.SetParent(containerGO.transform);
        borderGO.transform.position = new Vector3(0, boxCenterY, 0f);
        borderGO.transform.localScale = new Vector3(BoxHalfW * 2f, boxH, 1f);
        var borderSr = borderGO.AddComponent<SpriteRenderer>();
        borderSr.sprite = borderSprite;
        borderSr.color = Hex("4E342E");
        borderSr.sortingOrder = 2;

        // 물리 벽 + 시각
        CreateWall(
            containerGO,
            "LeftWall",
            squareSprite,
            new Vector3(-(BoxHalfW + 0.1f), 0, 0),
            new Vector2(0.2f, 14f)
        );
        CreateWall(
            containerGO,
            "RightWall",
            squareSprite,
            new Vector3(BoxHalfW + 0.1f, 0, 0),
            new Vector2(0.2f, 14f)
        );
        CreateWall(
            containerGO,
            "Bottom",
            squareSprite,
            new Vector3(0, BoxBottom - 0.1f, 0),
            new Vector2(BoxHalfW * 2 + 0.4f, 0.2f)
        );

        // Danger Zone trigger
        var dzGO = new GameObject("DangerZone");
        dzGO.transform.SetParent(containerGO.transform);
        dzGO.transform.position = new Vector3(0, DangerY, 0);
        var dzCol = dzGO.AddComponent<BoxCollider2D>();
        dzCol.isTrigger = true;
        dzCol.size = new Vector2(BoxHalfW * 2, 0.5f);
        var dz = dzGO.AddComponent<DangerZone>();
        SetPrivateField(dz, "timeout", 3f);

        // Danger Line visual — 빨간 가로선
        var dlVisGO = new GameObject("DangerLineVisual");
        dlVisGO.transform.SetParent(containerGO.transform);
        dlVisGO.transform.position = new Vector3(0, DangerY, 0);
        var dlSr = dlVisGO.AddComponent<SpriteRenderer>();
        dlSr.sprite = squareSprite;
        dlSr.color = new Color(1f, 0.15f, 0.15f, 0.75f);
        dlVisGO.transform.localScale = new Vector3(BoxHalfW * 2f, 0.06f, 1f);

        // ── Canvas / UI ────────────────────────────────────────────────────
        var (canvas, canvasGO) = MakeCanvas("Canvas", 540, 960);
        var gameUI = canvasGO.AddComponent<GameUI>();

        // HUD Panel
        var hudGO = new GameObject("HUDPanel");
        hudGO.transform.SetParent(canvasGO.transform, false);
        SetStretch(hudGO, new Vector2(0, 0.88f), new Vector2(1, 1));

        var bestText = MakeTMPText(hudGO, "BestScoreText", "BEST 0", 18, Hex("555555"));
        var scoreText = MakeTMPText(hudGO, "CurrentScoreText", "0", 28, Hex("1B5E20"));
        PositionRect(bestText, new Vector2(0.5f, 0.7f), new Vector2(300, 30));
        PositionRect(scoreText, new Vector2(0.5f, 0.2f), new Vector2(300, 50));
        scoreText.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

        // Next fruit panel
        var nextPanelGO = new GameObject("NextPanel");
        nextPanelGO.transform.SetParent(canvasGO.transform, false);
        var nextRT = nextPanelGO.AddComponent<RectTransform>();
        nextRT.anchorMin = new Vector2(0f, 0.82f);
        nextRT.anchorMax = new Vector2(0f, 0.82f);
        nextRT.pivot = new Vector2(0, 0.5f);
        nextRT.anchoredPosition = new Vector2(16, 0);
        nextRT.sizeDelta = new Vector2(80, 90);

        var nextLabel = MakeTMPText(nextPanelGO, "NextLabel", "NEXT", 14, Hex("555555"));
        PositionRect(nextLabel, new Vector2(0.5f, 0.85f), new Vector2(70, 22));

        var nextImgGO = new GameObject("NextFruitImage");
        nextImgGO.transform.SetParent(nextPanelGO.transform, false);
        var nextImg = nextImgGO.AddComponent<Image>();
        nextImg.sprite = circleSprite;
        nextImg.color = Hex("FF1744"); // 체리 색상(Lv.1) — 런타임에 GameUI가 갱신
        nextImg.preserveAspect = true;
        var nextImgRT = nextImgGO.GetComponent<RectTransform>();
        nextImgRT.anchorMin = new Vector2(0.5f, 0.3f);
        nextImgRT.anchorMax = new Vector2(0.5f, 0.3f);
        nextImgRT.pivot = new Vector2(0.5f, 0.5f);
        nextImgRT.anchoredPosition = Vector2.zero;
        nextImgRT.sizeDelta = new Vector2(50, 50);

        // Pause button
        var pauseBtnGO = MakeButton(canvasGO, "PauseButton", "II", 20);
        var pauseBtnRT = pauseBtnGO.GetComponent<RectTransform>();
        pauseBtnRT.anchorMin = new Vector2(1, 1);
        pauseBtnRT.anchorMax = new Vector2(1, 1);
        pauseBtnRT.pivot = new Vector2(1, 1);
        pauseBtnRT.anchoredPosition = new Vector2(-12, -12);
        pauseBtnRT.sizeDelta = new Vector2(60, 40);

        // GameOver panel (hidden) — GameOverPanelSetup 과 동일한 구조로 생성
        var goPanel = GameOverPanelSetup.CreateGameOverPanel(canvasGO);
        var finalScore = goPanel.transform.Find("Card/FinalScoreText")?.gameObject;
        var finalBest = goPanel.transform.Find("Card/FinalBestText")?.gameObject;
        var restartBtn = goPanel.transform.Find("Card/RestartButton")?.gameObject;
        var titleBtn = goPanel.transform.Find("Card/TitleButton")?.gameObject;

        // Pause panel (hidden)
        var pausePanel = MakePanel(canvasGO, "PausePanel", new Color(0, 0, 0, 0.6f));
        pausePanel.SetActive(false);
        var resumeBtn = MakeButton(pausePanel, "ResumeButton", "RESUME", 22);
        var restartBtn2 = MakeButton(pausePanel, "RestartButton2", "RESTART", 22);
        var titleBtn2 = MakeButton(pausePanel, "TitleButton2", "TITLE", 22);
        PositionRect(resumeBtn, new Vector2(0.5f, 0.6f), new Vector2(180, 55));
        PositionRect(restartBtn2, new Vector2(0.5f, 0.48f), new Vector2(180, 55));
        PositionRect(titleBtn2, new Vector2(0.5f, 0.36f), new Vector2(180, 55));

        // Danger fill bar (red fill showing DangerZone timer progress)
        var dangerFillGO = CreateDangerFillBar(canvasGO);

        // Title panel (covers entire screen at game start)
        var titlePanel = TitlePanelSetup.CreateTitlePanel(canvasGO);
        var titleUI = titlePanel.GetComponent<TitleUI>();
        var tuSO = new UnityEditor.SerializedObject(titleUI);
        tuSO.FindProperty("bestScoreText").objectReferenceValue = titlePanel
            .transform.Find("Card/BestScoreText")
            ?.GetComponent<TMP_Text>();
        tuSO.ApplyModifiedProperties();
        WireTitleButton(
            titlePanel.transform.Find("Card/StartButton")?.gameObject,
            titleUI,
            nameof(titleUI.OnStartButton)
        );
        titlePanel.transform.SetAsLastSibling();

        // Wire GameUI serialized fields
        SetPrivateField(gameUI, "currentScoreText", scoreText.GetComponent<TMP_Text>());
        SetPrivateField(gameUI, "bestScoreText", bestText.GetComponent<TMP_Text>());
        SetPrivateField(gameUI, "nextFruitImage", nextImg);
        SetPrivateField(gameUI, "fruitConfig", config);
        SetPrivateField(gameUI, "titlePanel", titlePanel);
        SetPrivateField(gameUI, "gameOverPanel", goPanel);
        SetPrivateField(gameUI, "finalScoreText", finalScore.GetComponent<TMP_Text>());
        SetPrivateField(gameUI, "finalBestText", finalBest.GetComponent<TMP_Text>());
        SetPrivateField(gameUI, "pausePanel", pausePanel);
        SetPrivateField(gameUI, "dangerFill", dangerFillGO.GetComponent<Slider>());

        // Wire button OnClick events
        WireButton(pauseBtnGO, gameUI, nameof(gameUI.OnPauseButton));
        WireButton(restartBtn, gameUI, nameof(gameUI.OnRestartButton));
        WireButton(titleBtn, gameUI, nameof(gameUI.OnRestartButton)); // Title = Restart (단일 씬)
        WireButton(resumeBtn, gameUI, nameof(gameUI.OnResumeButton));
        WireButton(restartBtn2, gameUI, nameof(gameUI.OnRestartButton));
        WireButton(titleBtn2, gameUI, nameof(gameUI.OnRestartButton));

        MakeEventSystem();

        EditorSceneManager.SaveScene(scene, path);
        Debug.Log("Game scene built → Assets/Scenes/Game.unity");
    }

    // ══════════════════════════════════════════════════════════════════════
    // 7. Build Settings
    // ══════════════════════════════════════════════════════════════════════
    static void UpdateBuildSettings()
    {
        var scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true) };
        EditorBuildSettings.scenes = scenes;
    }

    // ══════════════════════════════════════════════════════════════════════
    // Helpers
    // ══════════════════════════════════════════════════════════════════════
    static void CreateWall(GameObject parent, string name, Sprite sprite, Vector3 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Hex("4E342E");

        var col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

    // ── DangerFill ─────────────────────────────────────────────────────────
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

        // Fill
        var fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(go.transform, false);
        var fillImg = fillGO.AddComponent<Image>();
        fillImg.color = new Color(1f, 0.1f, 0.1f, 0.85f);
        var fillRT = (RectTransform)fillGO.transform;
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = Vector2.one;
        fillRT.offsetMin = fillRT.offsetMax = Vector2.zero;

        // Slider
        var slider = go.AddComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.fillRect = fillRT;

        return go;
    }

    static void WireButton(GameObject btnGO, GameUI target, string methodName)
    {
        var btn = btnGO.GetComponent<Button>();
        if (btn == null)
            return;
        var action = (UnityAction)
            System.Delegate.CreateDelegate(typeof(UnityAction), target, methodName);
        UnityEventTools.AddPersistentListener(btn.onClick, action);
    }

    static void WireTitleButton(GameObject btnGO, TitleUI target, string methodName)
    {
        if (btnGO == null)
            return;
        var btn = btnGO.GetComponent<Button>();
        if (btn == null)
            return;
        var action = (UnityAction)
            System.Delegate.CreateDelegate(typeof(UnityAction), target, methodName);
        UnityEventTools.AddPersistentListener(btn.onClick, action);
    }

    static (Canvas, GameObject) MakeCanvas(string name, float refW, float refH)
    {
        var go = new GameObject(name);
        var c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = go.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(refW, refH);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return (c, go);
    }

    static GameObject MakeTMPText(
        GameObject parent,
        string name,
        string content,
        int size,
        Color color
    )
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    static GameObject MakeButton(GameObject parent, string name, string label, int fontSize)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        var img = go.AddComponent<Image>();
        img.color = Hex("2E7D32");
        go.AddComponent<Button>();

        var textGO = new GameObject("Text");
        textGO.transform.SetParent(go.transform, false);
        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        var tRT = textGO.GetComponent<RectTransform>();
        tRT.anchorMin = Vector2.zero;
        tRT.anchorMax = Vector2.one;
        tRT.offsetMin = tRT.offsetMax = Vector2.zero;

        return go;
    }

    static GameObject MakePanel(GameObject parent, string name, Color bgColor)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        SetStretch(go, Vector2.zero, Vector2.one);
        return go;
    }

    static void MakeEventSystem()
    {
        var es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
    }

    static void SetStretch(GameObject go, Vector2 anchorMin, Vector2 anchorMax)
    {
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static void PositionRect(GameObject go, Vector2 anchor, Vector2 size)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt == null)
            return;
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
    }

    // SerializedObject 를 통해 private [SerializeField] 필드 설정
    static void SetPrivateField(Object target, string fieldName, object value)
    {
        var so = new SerializedObject(target);
        var sp = so.FindProperty(fieldName);
        if (sp == null)
        {
            Debug.LogWarning($"Field not found: {fieldName}");
            return;
        }

        switch (value)
        {
            case float f:
                sp.floatValue = f;
                break;
            case int i:
                sp.intValue = i;
                break;
            case bool b:
                sp.boolValue = b;
                break;
            case string s:
                sp.stringValue = s;
                break;
            case Object o:
                sp.objectReferenceValue = o;
                break;
            case Color c:
                sp.colorValue = c;
                break;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }
}
