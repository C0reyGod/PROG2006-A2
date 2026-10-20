using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Builds the six picture-book scenes into Assets/Scenes/MidAutumn.
public static class MidAutumnSceneBuilder
{
    const string ArtDir = "Assets/Art/Storybook";
    const string SceneDir = "Assets/Scenes/MidAutumn";
    const string FlagPath = ArtDir + "/BUILD_NOW.txt";

    static readonly Color Ink = new Color(0.10f, 0.13f, 0.28f, 1f);
    static readonly Color InkPressed = new Color(0.06f, 0.08f, 0.16f, 1f);
    static readonly Color Gold = new Color(1f, 0.84f, 0.28f, 1f);
    static readonly Color GoldPressed = new Color(0.72f, 0.55f, 0.12f, 1f);
    static readonly Color Cream = new Color(1f, 0.97f, 0.88f, 1f);
    static readonly Color Night = new Color(0.07f, 0.09f, 0.20f, 0.94f);
    static readonly Color HintYellow = new Color(1f, 0.86f, 0.35f, 1f);

    static DefaultControls.Resources ui;
    static TMP_FontAsset font;

    [MenuItem("MidAutumn/Build Story Scenes")]
    public static void BuildAll()
    {
        AssetDatabase.Refresh();
        AssetDatabase.Refresh();
        font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (font == null)
        {
            Debug.LogError("Liberation Sans font is missing.");
            return;
        }

        ui = new DefaultControls.Resources
        {
            standard = LoadBuiltin("UI/Skin/UISprite.psd"),
            background = LoadBuiltin("UI/Skin/Background.psd"),
            inputField = LoadBuiltin("UI/Skin/InputFieldBackground.psd"),
            knob = LoadBuiltin("UI/Skin/Knob.psd"),
            checkmark = LoadBuiltin("UI/Skin/Checkmark.psd"),
            dropdown = LoadBuiltin("UI/Skin/DropdownArrow.psd"),
            mask = LoadBuiltin("UI/Skin/UIMask.psd")
        };

        Directory.CreateDirectory(SceneDir);
        var village = Spr("bg_village_night");
        var forest = Spr("bg_forest_edge");
        var dark = Spr("bg_dark_forest");
        var terrace = Spr("bg_moon_terrace");
        var doudou = Spr("doudou_idle");
        var doudouHold = Spr("doudou_hold");
        var doudouCry = Spr("doudou_cry");
        var doudouHappy = Spr("doudou_happy");
        var lian = Spr("lianlian_idle");
        var lianCry = Spr("lianlian_cry");
        var lianHappy = Spr("lianlian_happy");
        var moon = Spr("moon_full");
        var chipped = Spr("moon_chipped");
        var moonSad = Spr("moon_sad");
        var moonHappy = Spr("moon_happy");
        var light = Spr("light_orb");
        var bush = Spr("bush");
        var rabbit = Spr("jade_rabbit");
        var goddess = Spr("change");
        var cake = Spr("mooncake");
        var path = Spr("glow_path");

        BuildHome(village, doudou, lian, moon);
        BuildMoon(village, doudou, doudouHold, lian, moon, chipped, light);
        BuildChase(forest, doudou, lian, light, bush);
        BuildLost(dark, doudouCry, doudouHappy, lianCry, lianHappy, bush, path, moonSad, moonHappy);
        BuildGoddess(terrace, doudou, lian, moon, chipped, goddess, rabbit, cake, light);
        BuildCredits(village, doudouHappy, lianHappy, moon, rabbit, goddess);

        EditorBuildSettings.scenes = new[]
        {
            Scene(StoryPages.Home),
            Scene(StoryPages.MoonGazing),
            Scene(StoryPages.ChasingLight),
            Scene(StoryPages.LostAndFound),
            Scene(StoryPages.MoonGoddess),
            Scene(StoryPages.Credits)
        };

        PlayerSettings.productName = "A Mid-Autumn Adventure";
        PlayerSettings.defaultScreenWidth = 960;
        PlayerSettings.defaultScreenHeight = 600;
        PlayerSettings.defaultWebScreenWidth = 960;
        PlayerSettings.defaultWebScreenHeight = 600;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.DefaultCompany.MidAutumnAdventure");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, "com.DefaultCompany.MidAutumnAdventure");

        EditorSceneManager.OpenScene(SceneDir + "/Home.unity");
        AssetDatabase.SaveAssets();
        Debug.Log("Mid-Autumn scenes are ready.");
    }

    [InitializeOnLoadMethod]
    static void AutoBuildOnce()
    {
        EditorApplication.delayCall += AutoBuildTick;
    }

    static void AutoBuildTick()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall += AutoBuildTick;
            return;
        }
        if (!File.Exists(FlagPath))
            return;
        try
        {
            BuildAll();
            if (File.Exists(FlagPath))
                File.Delete(FlagPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Mid-Autumn build failed: " + ex);
        }
    }

    static EditorBuildSettingsScene Scene(string name)
    {
        return new EditorBuildSettingsScene(SceneDir + "/" + name + ".unity", true);
    }

    static void BuildHome(Sprite bg, Sprite doudou, Sprite lian, Sprite moon)
    {
        var page = Begin(StoryPages.Home, "", "", "", StoryPages.MoonGazing, true, false);
        var logo = Actor(page.stage, "TitleLogo", Spr("title_logo"), new Vector2(560f, 130f), Vector2.zero, false);
        var logoRt = logo.rectTransform;
        logoRt.anchorMin = logoRt.anchorMax = new Vector2(0.5f, 1f);
        logoRt.pivot = new Vector2(0.5f, 1f);
        logoRt.anchoredPosition = new Vector2(0f, -6f);
        Actor(page.stage, "Moon", moon, new Vector2(86, 86), new Vector2(0, -18), false);
        Actor(page.stage, "Doudou", doudou, new Vector2(168, 196), new Vector2(-155, -36), false);
        Actor(page.stage, "Lianlian", lian, new Vector2(168, 196), new Vector2(155, -36), false);
        var start = PictureButton(page.stage, "Start", Spr("ui_button_start"), new Vector2(300, 86), true);
        page.nav.startButton = start;
        End(page);
    }

    static void BuildMoon(Sprite bg, Sprite doudou, Sprite hold, Sprite lian, Sprite moon, Sprite chipped, Sprite light)
    {
        var page = Begin(StoryPages.MoonGazing,
            "On a bright Mid-Autumn night, best friends Doudou and Lianlian are admiring the beautiful, round moon. Suddenly, the magical moon starts to twinkle. Wait, what is happening up there?",
            "Click the moon", StoryPages.Home, StoryPages.ChasingLight, false, true);
        var miss = Miss(page.stage);
        var hand = new Vector2(-160f, -52f);
        var sky = new Vector2(0f, 36f);
        var chippedGo = Actor(page.stage, "MoonChipped", chipped, new Vector2(132, 132), sky, false);
        chippedGo.gameObject.SetActive(false);
        var full = SpriteButton(page.stage, "Moon", moon, new Vector2(132, 132), sky);
        var idle = Actor(page.stage, "Doudou", doudou, new Vector2(170, 200), new Vector2(-160, -24), false);
        var holding = Actor(page.stage, "DoudouHold", hold, new Vector2(170, 200), new Vector2(-160, -24), false);
        holding.gameObject.SetActive(false);
        Actor(page.stage, "Lianlian", lian, new Vector2(170, 200), new Vector2(155, -24), false);
        var piece = Actor(page.stage, "LightPiece", light, new Vector2(58, 58), hand, false);
        piece.gameObject.SetActive(false);
        var task = page.root.AddComponent<MoonTask>();
        task.moonButton = full;
        task.missButton = miss;
        task.fullMoon = full.gameObject;
        task.chippedMoon = chippedGo.gameObject;
        task.lightPiece = piece.gameObject;
        task.idlePose = idle.gameObject;
        task.holdPose = holding.gameObject;
        task.lightRect = piece.rectTransform;
        task.moonPos = sky;
        task.handPos = hand;
        task.hint = page.hint;
        UnityEventTools.AddPersistentListener(full.onClick, task.ClickMoon);
        if (miss != null)
            UnityEventTools.AddPersistentListener(miss.onClick, task.ClickMiss);
        End(page);
    }

    static void BuildChase(Sprite bg, Sprite doudou, Sprite lian, Sprite light, Sprite bush)
    {
        var page = Begin(StoryPages.ChasingLight,
            "A glowing piece of the moon falls from the sky and flies straight into the dark, spooky forest. Doudou and Lianlian must chase the magical light before it gets away!",
            "Where is it hiding?", StoryPages.MoonGazing, StoryPages.LostAndFound, false, true);
        var miss = Miss(page.stage);
        Actor(page.stage, "Doudou", doudou, new Vector2(160, 190), new Vector2(-170, -30), false);
        Actor(page.stage, "Lianlian", lian, new Vector2(160, 190), new Vector2(165, -30), false);
        var spots = new[] { new Vector2(-185f, -70f), new Vector2(0f, -56f), new Vector2(185f, -70f) };
        var orb = SpriteButton(page.stage, "Light", light, new Vector2(78, 78), spots[0]);
        orb.transform.SetAsLastSibling();
        var task = page.root.AddComponent<LightTask>();
        task.pageId = StoryPages.ChasingLight;
        task.lightButton = orb;
        task.missButton = miss;
        task.lightRect = orb.GetComponent<RectTransform>();
        task.bushSprite = bush;
        task.spots = spots;
        task.hint = page.hint;
        End(page);
    }

    static void BuildLost(Sprite bg, Sprite cryA, Sprite happyA, Sprite cryB, Sprite happyB, Sprite bush, Sprite path, Sprite moonSad, Sprite moonHappy)
    {
        var page = Begin(StoryPages.LostAndFound,
            "The forest is too dark, and the two little mooncakes feel lost and scared. They start to cry, but wait! A warm light is hiding right behind those thick bushes.",
            "Move the bushes", StoryPages.ChasingLight, StoryPages.MoonGoddess, false, true);
        var miss = Miss(page.stage);
        var sky = Actor(page.stage, "SkyMoon", moonSad, new Vector2(92, 92), new Vector2(0, 102), false);
        var sky2 = Actor(page.stage, "SkyMoon2", moonHappy, new Vector2(92, 92), new Vector2(0, 102), false);
        sky2.gameObject.SetActive(false);
        var glow = Actor(page.stage, "GlowPath", path, new Vector2(780, 340), new Vector2(0, -72), false);
        glow.gameObject.SetActive(false);
        var happyD = Actor(page.stage, "DoudouHappy", happyA, new Vector2(160, 190), new Vector2(-150, -10), false);
        var happyL = Actor(page.stage, "LianlianHappy", happyB, new Vector2(160, 190), new Vector2(150, -10), false);
        happyD.gameObject.SetActive(false);
        happyL.gameObject.SetActive(false);
        var cryD = Actor(page.stage, "Doudou", cryA, new Vector2(160, 190), new Vector2(-150, -10), false);
        var cryL = Actor(page.stage, "Lianlian", cryB, new Vector2(160, 190), new Vector2(150, -10), false);
        var b0 = BushActor(page.stage, "BushA", bush, new Vector2(200, 130), new Vector2(-170, -78));
        var b1 = BushActor(page.stage, "BushB", bush, new Vector2(210, 136), new Vector2(8, -92));
        var b2 = BushActor(page.stage, "BushC", bush, new Vector2(190, 126), new Vector2(175, -72));
        b1.GetComponent<RectTransform>().localScale = new Vector3(-1f, 1f, 1f);
        var task = page.root.AddComponent<BushTask>();
        task.missButton = miss;
        task.bushes = new[] { b0, b1, b2 };
        task.showWhenDone = new[] { glow.gameObject, happyD.gameObject, happyL.gameObject };
        task.hideWhenDone = new[] { cryD.gameObject, cryL.gameObject };
        task.moonBefore = sky.gameObject;
        task.moonAfter = sky2.gameObject;
        task.hint = page.hint;
        glow.transform.SetSiblingIndex(1);
        End(page);
    }

    static void BuildGoddess(Sprite bg, Sprite doudou, Sprite lian, Sprite moon, Sprite chipped, Sprite goddess, Sprite rabbit, Sprite cake, Sprite light)
    {
        var page = Begin(StoryPages.MoonGoddess,
            "Following the light, they discover the Moon Goddess, Chang'e, and her cute Jade Rabbit! By sharing a yummy mooncake, they can make the broken moon completely whole again.",
            "Give the mooncake to the rabbit", StoryPages.LostAndFound, StoryPages.Credits, false, true);
        var miss = Miss(page.stage);
        var sky = new Vector2(0f, 78f);
        var full = Actor(page.stage, "MoonFull", moon, new Vector2(118, 118), sky, false);
        var broken = Actor(page.stage, "MoonChipped", chipped, new Vector2(118, 118), sky, false);
        full.gameObject.SetActive(false);
        Actor(page.stage, "Change", goddess, new Vector2(150, 280), new Vector2(200, 0), false);
        var rabbitImg = Actor(page.stage, "JadeRabbit", rabbit, new Vector2(130, 160), new Vector2(40, -35), false);
        Actor(page.stage, "Doudou", doudou, new Vector2(145, 175), new Vector2(-195, -20), false);
        Actor(page.stage, "Lianlian", lian, new Vector2(130, 160), new Vector2(-55, -30), false);
        var given = Actor(page.stage, "MooncakeGiven", cake, new Vector2(54, 54), new Vector2(78, -8), false);
        given.gameObject.SetActive(false);
        var glow = Actor(page.stage, "ReturningLight", light, new Vector2(48, 48), new Vector2(40, -10), false);
        glow.gameObject.SetActive(false);
        var held = SpriteButton(page.stage, "Mooncake", cake, new Vector2(58, 58), new Vector2(-145, 8));
        var task = page.root.AddComponent<GiftTask>();
        task.missButton = miss;
        task.mooncakeInHand = held.gameObject;
        task.mooncakeGiven = given.gameObject;
        task.chippedMoon = broken.gameObject;
        task.fullMoon = full.gameObject;
        task.rabbitBody = rabbitImg.rectTransform;
        task.returningLight = glow.rectTransform;
        task.moonPoint = broken.rectTransform;
        task.hint = page.hint;
        var drag = held.gameObject.AddComponent<GiftDrag>();
        drag.task = task;
        drag.rabbit = rabbitImg.rectTransform;
        End(page);
    }

    static void BuildCredits(Sprite bg, Sprite doudou, Sprite lian, Sprite moon, Sprite rabbit, Sprite goddess)
    {
        var page = Begin(StoryPages.Credits,
            "Courage and sharing bring magical completeness. Happy Mid-Autumn Festival to you all!",
            "", StoryPages.MoonGoddess, "", false, true);
        Actor(page.stage, "Moon", moon, new Vector2(100, 100), new Vector2(0, 70), false);
        Actor(page.stage, "Change", goddess, new Vector2(120, 230), new Vector2(210, -10), false);
        Actor(page.stage, "JadeRabbit", rabbit, new Vector2(110, 140), new Vector2(70, -40), false);
        Actor(page.stage, "Doudou", doudou, new Vector2(150, 180), new Vector2(-160, -15), false);
        Actor(page.stage, "Lianlian", lian, new Vector2(150, 180), new Vector2(-10, -20), false);
        var replay = PictureButton(page.stage, "Replay", Spr("ui_button_replay"), new Vector2(280, 78), true);
        page.nav.replayButton = replay;
        var note = Body(page.stage, "Name: Your Name\nPictures made for this book.\nFont: Liberation Sans.", 15f, Cream);
        note.gameObject.name = "CreditNote";
        note.gameObject.SetActive(false);
        note.alignment = TextAlignmentOptions.Left;
        note.fontSize = 15f;
        note.fontSizeMax = 15f;
        var noteRt = note.rectTransform;
        noteRt.anchorMin = noteRt.anchorMax = new Vector2(0f, 1f);
        noteRt.pivot = new Vector2(0f, 1f);
        noteRt.anchoredPosition = new Vector2(16f, -12f);
        noteRt.sizeDelta = new Vector2(250f, 72f);
        var shadow = note.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.08f, 0.08f, 0.16f, 0.85f);
        shadow.effectDistance = new Vector2(1.2f, -1.2f);
        End(page);
    }

    class Page
    {
        public GameObject root;
        public Canvas canvas;
        public RectTransform stage;
        public RectTransform panel;
        public Button next;
        public TMP_Text story;
        public TMP_Text hint;
        public NavButtons nav;
        public string sceneName;
    }

    static Page Begin(string sceneName, string story, string hint, string back, string next, bool nextOn, bool backOn)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.06f, 0.07f, 0.16f);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        camGo.AddComponent<AudioListener>();
        var urp = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalAdditionalCameraData, Unity.RenderPipelines.Universal.Runtime");
        if (urp != null && camGo.GetComponent(urp) == null)
            camGo.AddComponent(urp);

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        new GameObject("BgmPlayer", typeof(AudioSource), typeof(BgmPlayer));

        var canvasGo = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960f, 600f);
        scaler.matchWidthOrHeight = 0.5f;

        string bgName = sceneName == StoryPages.Home || sceneName == StoryPages.MoonGazing || sceneName == StoryPages.Credits
            ? "bg_village_night"
            : sceneName == StoryPages.ChasingLight ? "bg_forest_edge"
            : sceneName == StoryPages.LostAndFound ? "bg_dark_forest"
            : "bg_moon_terrace";
        var background = Stretch(canvasGo.transform, "Background", Spr(bgName));
        background.raycastTarget = false;
        background.preserveAspect = false;

        bool showBubble = sceneName != StoryPages.Home;
        float bottom = showBubble ? 224f : 72f;

        var stageGo = new GameObject("Stage", typeof(RectTransform));
        stageGo.transform.SetParent(canvasGo.transform, false);
        var stage = stageGo.GetComponent<RectTransform>();
        stage.anchorMin = Vector2.zero;
        stage.anchorMax = Vector2.one;
        stage.offsetMin = new Vector2(0f, bottom);
        stage.offsetMax = Vector2.zero;

        RectTransform panelRt = null;
        TMP_Text hintLabel = null;
        TMP_Text storyLabel = null;
        if (showBubble)
        {
            var panel = new GameObject("TextPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(RectMask2D));
            panel.transform.SetParent(canvasGo.transform, false);
            panelRt = panel.GetComponent<RectTransform>();
            panelRt.anchorMin = new Vector2(0.04f, 0f);
            panelRt.anchorMax = new Vector2(0.96f, 0f);
            panelRt.pivot = new Vector2(0.5f, 0f);
            panelRt.anchoredPosition = new Vector2(0f, 72f);
            panelRt.sizeDelta = new Vector2(0f, 152f);
            var panelImage = panel.GetComponent<Image>();
            panelImage.sprite = Spr("ui_text_bubble");
            panelImage.color = Color.white;
            panelImage.type = Image.Type.Simple;
            panelImage.preserveAspect = false;
            panelImage.raycastTarget = false;

            storyLabel = Body(panel.transform, story, 18f, new Color(0.13f, 0.16f, 0.32f), true);
            var storyRt = storyLabel.rectTransform;
            storyRt.anchorMin = Vector2.zero;
            storyRt.anchorMax = Vector2.one;
            storyRt.offsetMin = new Vector2(72f, 46f);
            storyRt.offsetMax = new Vector2(-72f, -30f);
            storyLabel.margin = new Vector4(6f, 4f, 6f, 4f);
        }

        if (!string.IsNullOrEmpty(hint))
        {
            hintLabel = Body(canvasGo.transform, hint, 30f, HintYellow);
            var hintRt = hintLabel.rectTransform;
            hintRt.anchorMin = hintRt.anchorMax = new Vector2(0.5f, 1f);
            hintRt.pivot = new Vector2(0.5f, 1f);
            hintRt.anchoredPosition = new Vector2(0f, -16f);
            hintRt.sizeDelta = new Vector2(640f, 50f);
            var hintShadow = hintLabel.gameObject.AddComponent<Shadow>();
            hintShadow.effectColor = new Color(0.08f, 0.1f, 0.22f, 0.9f);
            hintShadow.effectDistance = new Vector2(1.4f, -1.4f);
        }

        var navBar = new GameObject("NavBar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        navBar.transform.SetParent(canvasGo.transform, false);
        var navRt = navBar.GetComponent<RectTransform>();
        navRt.anchorMin = new Vector2(0f, 0f);
        navRt.anchorMax = new Vector2(1f, 0f);
        navRt.pivot = new Vector2(0.5f, 0f);
        navRt.anchoredPosition = Vector2.zero;
        navRt.sizeDelta = new Vector2(0f, 72f);
        var navImage = navBar.GetComponent<Image>();
        navImage.sprite = Spr("ui_nav_bar");
        navImage.color = Color.white;
        navImage.preserveAspect = false;
        navImage.raycastTarget = false;

        var backBtn = DockButton(navRt, "Back", Spr("ui_icon_back"));
        var homeBtn = DockButton(navRt, "Home", Spr("ui_icon_home"));
        var nextBtn = DockButton(navRt, "Next", Spr("ui_icon_next"));
        backBtn.interactable = backOn;
        bool hardOff = sceneName == StoryPages.Home || sceneName == StoryPages.Credits;
        nextBtn.interactable = false;

        var root = new GameObject("PageLogic");
        var nav = root.AddComponent<NavButtons>();
        nav.backScene = back;
        nav.nextScene = next;
        nav.backButton = backBtn;
        nav.homeButton = homeBtn;
        nav.nextButton = nextBtn;
        nav.nextBlocked = hardOff;
        nav.nextLocked = !hardOff;
        nav.hint = hintLabel;
        nav.lockedMessage = "Finish this page first.";
        if (!hardOff)
            nav.nextGate = NextGate(navRt);

        BuildSettings(canvasGo.transform, root);

        EditorSceneManager.MarkSceneDirty(scene);
        return new Page
        {
            root = root,
            canvas = canvas,
            stage = stage,
            panel = panelRt,
            next = nextBtn,
            story = storyLabel,
            hint = hintLabel,
            nav = nav,
            sceneName = sceneName
        };
    }

    static void End(Page page)
    {
        var scene = page.root.scene;
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, SceneDir + "/" + page.sceneName + ".unity");
    }

    static void BuildSettings(Transform canvas, GameObject root)
    {
        var open = IconButton(canvas, "SettingsButton", Spr("ui_icon_settings"), new Vector2(79.36f, 79.36f));
        var openRt = open.GetComponent<RectTransform>();
        openRt.anchorMin = new Vector2(1f, 1f);
        openRt.anchorMax = new Vector2(1f, 1f);
        openRt.pivot = new Vector2(1f, 1f);
        openRt.anchoredPosition = new Vector2(-12f, -12f);

        var musicBtn = IconButton(canvas, "MusicButton", Spr("ui_icon_music"), new Vector2(79.36f, 79.36f));
        var musicRt = musicBtn.GetComponent<RectTransform>();
        musicRt.anchorMin = new Vector2(1f, 1f);
        musicRt.anchorMax = new Vector2(1f, 1f);
        musicRt.pivot = new Vector2(1f, 1f);
        musicRt.anchoredPosition = new Vector2(-84f, -12f);

        var dimGo = new GameObject("SettingsRoot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        dimGo.transform.SetParent(canvas, false);
        var dimRt = dimGo.GetComponent<RectTransform>();
        dimRt.anchorMin = Vector2.zero;
        dimRt.anchorMax = Vector2.one;
        dimRt.offsetMin = Vector2.zero;
        dimRt.offsetMax = Vector2.zero;
        var dim = dimGo.GetComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.45f);

        var box = new GameObject("SettingsPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        box.transform.SetParent(dimGo.transform, false);
        var boxRt = box.GetComponent<RectTransform>();
        boxRt.anchorMin = boxRt.anchorMax = new Vector2(0.5f, 0.5f);
        boxRt.sizeDelta = new Vector2(520f, 280f);
        var boxImage = box.GetComponent<Image>();
        boxImage.sprite = Spr("ui_settings_panel");
        boxImage.color = Color.white;
        boxImage.type = Image.Type.Simple;
        boxImage.preserveAspect = false;

        var title = Body(box.transform, "Settings", 28f, Ink);
        var titleRt = title.rectTransform;
        titleRt.anchorMin = titleRt.anchorMax = new Vector2(0.5f, 1f);
        titleRt.pivot = new Vector2(0.5f, 1f);
        titleRt.anchoredPosition = new Vector2(0f, -16f);
        titleRt.sizeDelta = new Vector2(460f, 42f);
        title.fontSize = 28f;
        title.fontSizeMax = 28f;

        var music = MakeSlider(box.transform, "Music", new Vector2(0f, 40f));
        var text = MakeSlider(box.transform, "Text size", new Vector2(0f, -20f));
        var close = IconButton(box.transform, "Close", Spr("ui_icon_close"), new Vector2(54f, 54f));
        var closeRt = close.GetComponent<RectTransform>();
        closeRt.anchorMin = closeRt.anchorMax = new Vector2(0.5f, 0f);
        closeRt.pivot = new Vector2(0.5f, 0f);
        closeRt.anchoredPosition = new Vector2(0f, 18f);

        var menu = root.AddComponent<SettingsMenu>();
        menu.panel = dimGo;
        menu.openButton = open;
        menu.musicButton = musicBtn;
        menu.closeButton = close;
        menu.volumeSlider = music.slider;
        menu.textSlider = text.slider;
        menu.volumeLabel = music.value;
        menu.textLabel = text.value;
        dimGo.SetActive(false);
    }

    struct SliderRow
    {
        public Slider slider;
        public TMP_Text value;
    }

    static SliderRow MakeSlider(Transform parent, string caption, Vector2 pos)
    {
        var row = new GameObject(caption + "Row", typeof(RectTransform));
        row.transform.SetParent(parent, false);
        var rt = row.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(470f, 40f);
        rt.anchoredPosition = pos;

        var cap = Body(row.transform, caption, 22f, Ink);
        var capRt = cap.rectTransform;
        capRt.anchorMin = capRt.anchorMax = new Vector2(0f, 0.5f);
        capRt.pivot = new Vector2(0f, 0.5f);
        capRt.anchoredPosition = new Vector2(8f, 0f);
        capRt.sizeDelta = new Vector2(130f, 36f);
        cap.alignment = TextAlignmentOptions.MidlineLeft;

        var sliderGo = DefaultControls.CreateSlider(ui);
        sliderGo.name = caption + "Slider";
        sliderGo.transform.SetParent(row.transform, false);
        var srt = sliderGo.GetComponent<RectTransform>();
        srt.anchorMin = srt.anchorMax = new Vector2(0.5f, 0.5f);
        srt.anchoredPosition = new Vector2(20f, 0f);
        srt.sizeDelta = new Vector2(220f, 28f);

        var value = Body(row.transform, "70%", 22f, new Color(0.45f, 0.28f, 0.08f));
        var valueRt = value.rectTransform;
        valueRt.anchorMin = valueRt.anchorMax = new Vector2(1f, 0.5f);
        valueRt.pivot = new Vector2(1f, 0.5f);
        valueRt.anchoredPosition = new Vector2(-8f, 0f);
        valueRt.sizeDelta = new Vector2(70f, 36f);
        return new SliderRow { slider = sliderGo.GetComponent<Slider>(), value = value };
    }

    static void Title(Transform parent, string text)
    {
        var tmp = Body(parent, text, 34f, Gold);
        var rt = tmp.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -8f);
        rt.sizeDelta = new Vector2(900f, 52f);
        tmp.fontSize = 34f;
        tmp.fontSizeMax = 34f;
        var shadow = tmp.gameObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0.05f, 0.05f, 0.12f, 0.85f);
        shadow.effectDistance = new Vector2(1.5f, -1.5f);
    }

    static TMP_Text Body(Transform parent, string text, float size, Color color, bool followSettings = false)
    {
        var go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.font = font;
        tmp.text = text;
        tmp.fontSize = size;
        tmp.enableAutoSizing = followSettings;
        tmp.fontSizeMin = 14f;
        tmp.fontSizeMax = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        tmp.color = color;
        tmp.raycastTarget = false;
        if (followSettings)
        {
            var story = go.AddComponent<StoryText>();
            story.baseMaxSize = size;
        }
        return tmp;
    }

    static Image Stretch(Transform parent, string name, Sprite sprite)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = false;
        return image;
    }

    static Image Actor(Transform parent, string name, Sprite sprite, Vector2 size, Vector2 pos, bool raycast)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;
        var image = go.GetComponent<Image>();
        image.sprite = sprite;
        image.preserveAspect = true;
        image.raycastTarget = raycast;
        return image;
    }

    static Button SpriteButton(Transform parent, string name, Sprite sprite, Vector2 size, Vector2 pos)
    {
        var image = Actor(parent, name, sprite, size, pos, true);
        var button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.5f);
        colors.fadeDuration = 0.06f;
        button.colors = colors;
        return button;
    }

    static Button Miss(Transform stage)
    {
        var go = new GameObject("MissCatcher", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(stage, false);
        go.transform.SetAsFirstSibling();
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var image = go.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0f);
        var button = go.GetComponent<Button>();
        button.transition = Selectable.Transition.None;
        return button;
    }

    static Button NextGate(RectTransform bar)
    {
        var go = new GameObject("NextGate", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(bar, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.anchoredPosition = new Vector2(-18f, 4f);
        rt.sizeDelta = new Vector2(64f, 64f);
        var image = go.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0f);
        var button = go.GetComponent<Button>();
        button.transition = Selectable.Transition.None;
        button.targetGraphic = image;
        go.transform.SetAsLastSibling();
        return button;
    }

    static Button DockButton(RectTransform bar, string name, Sprite icon)
    {
        var button = IconButton(bar, name, icon, new Vector2(81.92f, 81.92f));
        var rt = button.GetComponent<RectTransform>();
        if (name == "Back")
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(18f, 4f);
        }
        else if (name == "Home")
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0f, 4f);
        }
        else
        {
            rt.anchorMin = rt.anchorMax = new Vector2(1f, 0.5f);
            rt.pivot = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(-18f, 4f);
        }
        return button;
    }

    static Button IconButton(Transform parent, string name, Sprite icon, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        var plate = go.GetComponent<Image>();
        plate.sprite = Spr("ui_button_plate");
        plate.preserveAspect = true;
        var button = go.GetComponent<Button>();
        button.targetGraphic = plate;
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.72f, 0.72f, 0.72f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.45f);
        colors.fadeDuration = 0.06f;
        button.colors = colors;
        var glyph = Actor(go.transform, "Icon", icon, size * 0.72f, Vector2.zero, false);
        glyph.preserveAspect = true;
        var tint = go.AddComponent<ButtonIconLock>();
        tint.icon = glyph;
        return button;
    }

    static Button PictureButton(Transform parent, string name, Sprite sprite, Vector2 size, bool bottomOfStage)
    {
        var button = SpriteButton(parent, name, sprite, size, Vector2.zero);
        var rt = button.GetComponent<RectTransform>();
        if (bottomOfStage)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 8f);
        }
        return button;
    }

    static BushHandle BushActor(Transform parent, string name, Sprite sprite, Vector2 size, Vector2 pos)
    {
        var image = Actor(parent, name, sprite, size, pos, true);
        return image.gameObject.AddComponent<BushHandle>();
    }

    static Button BarButton(Transform parent, string name, string label, Vector2 size, Color normal, Color textColor, int fontSize, bool bottomOfStage)
    {
        var go = DefaultControls.CreateButton(ui);
        go.name = name;
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        if (bottomOfStage)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 8f);
        }
        else
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }

        var legacy = go.GetComponentInChildren<Text>();
        var textGo = legacy.gameObject;
        Object.DestroyImmediate(legacy);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.font = font;
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 14f;
        tmp.fontSizeMax = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = textColor;
        tmp.raycastTarget = false;
        var textRt = tmp.rectTransform;
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(8f, 4f);
        textRt.offsetMax = new Vector2(-8f, -4f);

        var button = go.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = normal;
        colors.pressedColor = normal == Gold ? GoldPressed : InkPressed;
        colors.selectedColor = normal;
        colors.disabledColor = new Color(0.45f, 0.45f, 0.5f, 0.65f);
        colors.fadeDuration = 0.06f;
        button.colors = colors;
        return button;
    }

    static ColorBlock GoldColors()
    {
        var colors = ColorBlock.defaultColorBlock;
        colors.normalColor = Gold;
        colors.highlightedColor = Gold;
        colors.pressedColor = GoldPressed;
        colors.selectedColor = Gold;
        colors.disabledColor = new Color(0.45f, 0.45f, 0.5f, 0.65f);
        colors.fadeDuration = 0.06f;
        return colors;
    }

    static Sprite Spr(string name)
    {
        string folder = "Props";
        if (name.StartsWith("bg_")) folder = "Backgrounds";
        else if (name.StartsWith("ui_") || name == "title_logo") folder = "UI";
        else if (name.StartsWith("doudou") || name.StartsWith("lianlian") || name == "change" || name == "jade_rabbit")
            folder = "Characters";
        string path = ArtDir + "/" + folder + "/" + name + ".png";
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new FileNotFoundException("Missing picture", path);
        bool dirty = importer.textureType != TextureImporterType.Sprite
            || importer.spriteImportMode != SpriteImportMode.Single
            || importer.mipmapEnabled
            || !importer.alphaIsTransparency
            || importer.filterMode != FilterMode.Bilinear
            || importer.textureCompression != TextureImporterCompression.Uncompressed
            || importer.npotScale != TextureImporterNPOTScale.None
            || importer.maxTextureSize < 2048;
        if (dirty)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
        }
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
            throw new FileNotFoundException("Could not import sprite", path);
        return sprite;
    }

    static Sprite LoadBuiltin(string path)
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>(path);
    }
}

