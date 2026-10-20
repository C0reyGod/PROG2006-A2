using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MoonTask : MonoBehaviour
{
    public string pageId = StoryPages.MoonGazing;
    public Button moonButton;
    public Button missButton;
    public GameObject fullMoon;
    public GameObject chippedMoon;
    public GameObject lightPiece;
    public GameObject idlePose;
    public GameObject holdPose;
    public RectTransform lightRect;
    public Vector2 moonPos;
    public Vector2 handPos;
    public TMP_Text hint;
    public string hintStart = "Click the moon";
    public string hintDone = "A piece of the moon fell.";
    bool playing;
    RectTransform moonRect;
    float idle;

    void Start()
    {
        moonRect = moonButton == null ? null : moonButton.GetComponent<RectTransform>();
        if (moonButton != null)
        {
            moonButton.onClick.AddListener(ClickMoon);
            PressRelay.Ensure(moonButton, ClickMoon);
        }
        if (missButton != null)
            missButton.onClick.AddListener(ClickMiss);
        bool done = StoryProgress.IsDone(pageId);
        Apply(done, false);
        if (done)
            Unlock();
        SetHint(done ? hintDone : hintStart);
    }

    // While the moon is still waiting to be tapped it breathes and gives a
    // little nudge now and then, so children notice it.
    void Update()
    {
        if (moonRect == null || playing || StoryProgress.IsDone(pageId))
            return;
        idle += Time.deltaTime;
        float breathe = 1f + Mathf.Sin(idle * 2.1f) * 0.025f;
        float cycle = idle % 3.4f;
        float nudge = cycle < 0.55f ? Mathf.Sin(cycle / 0.55f * Mathf.PI) * 0.07f : 0f;
        float scale = breathe + nudge;
        moonRect.localScale = new Vector3(scale, scale, 1f);
    }

    void ResetMoonPose()
    {
        if (moonRect != null)
            moonRect.localScale = Vector3.one;
    }

    public void ClickMiss()
    {
        if (StoryProgress.IsDone(pageId))
            return;
        SetHint("Click it once more");
    }

    public void ClickMoon()
    {
        if (StoryProgress.IsDone(pageId) || playing)
            return;
        StoryProgress.Mark(pageId);
        StorySfx.PlayDing();
        playing = true;
        ResetMoonPose();
        Apply(true, true);
        SetHint(hintDone);
        Unlock();
    }

    void Apply(bool done, bool animate)
    {
        if (done) ResetMoonPose();
        if (fullMoon != null) fullMoon.SetActive(!done);
        if (chippedMoon != null) chippedMoon.SetActive(done);
        if (idlePose != null) idlePose.SetActive(!done);
        if (holdPose != null) holdPose.SetActive(done);
        if (lightPiece != null)
        {
            lightPiece.SetActive(done);
            if (done) lightPiece.transform.SetAsLastSibling();
        }
        if (!done || lightRect == null)
            return;
        if (animate)
            StartCoroutine(Fall());
        else
            lightRect.anchoredPosition = handPos;
    }

    IEnumerator Fall()
    {
        lightRect.anchoredPosition = moonPos;
        float t = 0f;
        while (t < 0.7f)
        {
            t += Time.deltaTime;
            float u = Mathf.SmoothStep(0f, 1f, t / 0.7f);
            lightRect.anchoredPosition = Vector2.Lerp(moonPos, handPos, u);
            yield return null;
        }
        lightRect.anchoredPosition = handPos;
        Unlock();
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
