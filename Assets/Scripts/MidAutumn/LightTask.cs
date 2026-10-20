using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LightTask : MonoBehaviour
{
    public string pageId = StoryPages.ChasingLight;
    public Button lightButton;
    public Button missButton;
    public RectTransform lightRect;
    public Sprite bushSprite;
    public Vector2[] spots;
    // Scenery already placed in the scene (the trees) that doubles as a spot.
    public RectTransform[] extraSpots;
    public TMP_Text hint;
    public string hintStart = "Where is it hiding?";
    public string hintDone = "Found it!";
    public string hintWrong = "No light here. Look for the glowing spot.";
    public float glowPulseSeconds = 2.8f;

    readonly List<RectTransform> spotList = new List<RectTransform>();
    readonly List<RectTransform> bushes = new List<RectTransform>();
    Image glowImage;
    float glowTime;
    static Sprite glowSprite;
    int answer;
    bool done;

    void Start()
    {
        if (missButton != null)
            missButton.onClick.AddListener(ClickMiss);

        if (StoryProgress.IsDone(pageId))
        {
            if (lightRect != null)
                lightRect.gameObject.SetActive(false);
            if (lightButton != null)
                lightButton.interactable = false;
            SetHint(hintDone);
            Unlock();
            return;
        }

        if (lightButton != null)
            lightButton.interactable = false;
        BuildSpots();
        SetHint(hintStart);
    }

    void BuildSpots()
    {
        if (lightRect == null)
            return;
        var parent = lightRect.parent;
        lightRect.gameObject.SetActive(false);

        spotList.Clear();
        bushes.Clear();

        // The halo gets its own layer, kept behind the characters and the
        // bushes so the light only spills out around the leaves.
        var glowRoot = new GameObject("SpotGlow", typeof(RectTransform));
        glowRoot.transform.SetParent(parent, false);
        var catcher = parent.Find("MissCatcher");
        glowRoot.transform.SetSiblingIndex(catcher != null ? catcher.GetSiblingIndex() + 1 : 0);

        // Hand-placed scenery (the trees) takes part in the hunt as well.
        if (extraSpots != null)
            foreach (var extra in extraSpots)
                if (extra != null && extra != lightRect)
                    spotList.Add(extra);

        if (bushSprite != null && spots != null && spots.Length > 0)
        {
            var root = new GameObject("HidingSpots", typeof(RectTransform));
            root.transform.SetParent(parent, false);
            root.transform.SetAsLastSibling();

            for (int i = 0; i < spots.Length; i++)
            {
                var go = new GameObject("Spot" + i, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                go.transform.SetParent(root.transform, false);
                var rt = go.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(210f, 140f);
                rt.anchoredPosition = spots[i];
                var image = go.GetComponent<Image>();
                image.sprite = bushSprite;
                image.preserveAspect = true;
                bushes.Add(rt);
                spotList.Add(rt);
            }
        }

        if (spotList.Count == 0)
            return;

        for (int i = 0; i < spotList.Count; i++)
            MakeClickable(spotList[i], i);

        answer = Random.Range(0, spotList.Count);
        BuildGlow(glowRoot.transform);
    }

    // Turns a prop into a spot: same button feel and wobble as a bush.
    void MakeClickable(RectTransform spot, int index)
    {
        var image = spot.GetComponent<Graphic>();
        if (image != null)
            image.raycastTarget = true;
        var button = spot.GetComponent<Button>();
        if (button == null)
            button = spot.gameObject.AddComponent<Button>();
        if (image != null)
            button.targetGraphic = image;
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.98f, 0.92f, 1f);
        colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
        colors.selectedColor = Color.white;
        colors.fadeDuration = 0.06f;
        button.colors = colors;
        button.onClick.AddListener(() => Choose(index));
        if (spot.GetComponent<SpotShake>() == null)
            spot.gameObject.AddComponent<SpotShake>();
    }

    // A soft warm halo sits *behind* the right spot, so a little light spills
    // out around its leaves. It only hints, it does not give the answer away.
    void BuildGlow(Transform parent)
    {
        var target = spotList[answer];
        var go = new GameObject("Halo", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        // Match the prop's on-screen size so the rim reads on a tree as well as a bush.
        Vector2 size = target.sizeDelta;
        Vector3 scale = target.localScale;
        rt.sizeDelta = new Vector2(Mathf.Max(268f, size.x * Mathf.Abs(scale.x) * 1.28f),
                                   Mathf.Max(180f, size.y * Mathf.Abs(scale.y) * 1.28f));
        rt.anchoredPosition = target.anchoredPosition;
        glowImage = go.GetComponent<Image>();
        glowImage.sprite = SoftGlow();
        glowImage.raycastTarget = false;
        glowImage.color = new Color(1f, 0.82f, 0.38f, 0.16f);
        glowTime = 0f;
    }

    // The halo breathes slowly between a faint and a slightly stronger glow.
    void Update()
    {
        if (glowImage == null)
            return;
        glowTime += Time.deltaTime;
        float k = 0.5f + 0.5f * Mathf.Sin(glowTime / glowPulseSeconds * Mathf.PI * 2f);
        var color = glowImage.color;
        color.a = Mathf.Lerp(0.16f, 0.46f, k);
        glowImage.color = color;
    }

    void Choose(int index)
    {
        if (done || index < 0 || index >= spotList.Count)
            return;
        var spot = spotList[index];
        if (index == answer)
        {
            done = true;
            StoryProgress.Mark(pageId);
            StorySfx.PlayDing();
            StartCoroutine(Reveal(spot));
            return;
        }
        StorySfx.PlayRustle();
        if (spot != null)
        {
            var shake = spot.GetComponent<SpotShake>();
            if (shake != null)
                shake.Play();
        }
        SetHint(hintWrong);
    }

    IEnumerator Reveal(RectTransform spot)
    {
        StartCoroutine(FadeGlow());
        // Only the added bushes fade away; scenery like the trees stays put.
        foreach (var bush in bushes)
        {
            if (bush == spot)
                continue;
            var group = bush.gameObject.AddComponent<CanvasGroup>();
            StartCoroutine(FadeOut(group));
        }

        var rest = spot != null ? spot.anchoredPosition : Vector2.zero;
        // Tall props (the trees) get the light above their canopy, not on the trunk.
        float settle = 34f;
        if (spot != null)
        {
            float half = spot.sizeDelta.y * Mathf.Abs(spot.localScale.y) * 0.5f;
            if (half > 90f)
                settle = half + 12f;
        }

        lightRect.gameObject.SetActive(true);
        lightRect.SetAsLastSibling();
        lightRect.localScale = Vector3.zero;
        lightRect.anchoredPosition = rest + new Vector2(0f, settle - 22f);
        float t = 0f;
        const float dur = 0.45f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            float scale = Mathf.Sin(u * Mathf.PI * 0.5f);
            float lift = Mathf.Sin(u * Mathf.PI) * 28f;
            lightRect.localScale = new Vector3(scale, scale, 1f);
            lightRect.anchoredPosition = rest + new Vector2(0f, settle - 22f + lift);
            yield return null;
        }
        lightRect.localScale = Vector3.one;
        lightRect.anchoredPosition = rest + new Vector2(0f, settle);
        SetHint(hintDone);
        Unlock();
    }

    static IEnumerator FadeOut(CanvasGroup group)
    {
        float t = 0f;
        while (t < 0.35f)
        {
            t += Time.deltaTime;
            group.alpha = 1f - Mathf.Clamp01(t / 0.35f);
            yield return null;
        }
        group.gameObject.SetActive(false);
    }

    IEnumerator FadeGlow()
    {
        if (glowImage == null)
            yield break;
        var image = glowImage;
        glowImage = null;
        float start = image.color.a;
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            var color = image.color;
            color.a = Mathf.Lerp(start, 0f, Mathf.Clamp01(t / 0.4f));
            image.color = color;
            yield return null;
        }
        image.gameObject.SetActive(false);
    }

    // Builds a soft-edged disc once, so the halo needs no extra art. The middle
    // stays solid (the bush hides it) and only the rim fades out.
    static Sprite SoftGlow()
    {
        if (glowSprite != null)
            return glowSprite;
        const int size = 128;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        var pixels = new Color[size * size];
        float half = size * 0.5f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - half) / half;
                float dy = (y + 0.5f - half) / half;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(0.62f, 1f, d));
                pixels[y * size + x] = new Color(1f, 1f, 1f, a);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        glowSprite = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
        return glowSprite;
    }

    public void ClickMiss()
    {
        if (done || StoryProgress.IsDone(pageId))
            return;
        SetHint(hintStart);
    }

    void Unlock()
    {
        var nav = GetComponent<NavButtons>();
        if (nav != null) nav.UnlockNext();
    }

    void SetHint(string value)
    {
        if (hint != null) hint.text = value;
    }
}

// Nudges a hiding spot sideways when it turns out to be empty.
public class SpotShake : MonoBehaviour
{
    RectTransform rect;
    Vector2 rest;
    float time;

    void Awake()
    {
        rect = transform as RectTransform;
        if (rect != null)
            rest = rect.anchoredPosition;
    }

    public void Play()
    {
        if (rect == null)
            return;
        rest = rect.anchoredPosition;
        time = 0.4f;
    }

    void Update()
    {
        if (rect == null || time <= 0f)
            return;
        time -= Time.deltaTime;
        if (time <= 0f)
        {
            rect.anchoredPosition = rest;
            return;
        }
        float amp = 9f * (time / 0.4f);
        rect.anchoredPosition = rest + new Vector2(Mathf.Sin(Time.time * 46f) * amp, 0f);
    }
}
