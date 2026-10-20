using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BushTask : MonoBehaviour
{
    public string pageId = StoryPages.LostAndFound;
    public Button missButton;
    public BushHandle[] bushes;
    public GameObject[] showWhenDone;
    public GameObject[] hideWhenDone;
    public GameObject moonBefore;
    public GameObject moonAfter;
    public float moonFadeSeconds = 0.6f;
    public TMP_Text hint;
    public string hintStart = "Move the bushes";
    public string hintDone = "The warm path is open.";
    int left;

    void Start()
    {
        left = bushes == null ? 0 : bushes.Length;
        if (bushes != null)
        {
            for (int i = 0; i < bushes.Length; i++)
            {
                if (bushes[i] != null)
                    bushes[i].Bind(this, i);
            }
        }
        if (missButton != null)
            missButton.onClick.AddListener(ClickMiss);

        if (StoryProgress.IsDone(pageId))
            Finish(false);
        else
            SetHint(hintStart);
    }

    public void ClearBush(int index)
    {
        if (StoryProgress.IsDone(pageId) || bushes == null || index < 0 || index >= bushes.Length)
            return;
        var bush = bushes[index];
        if (bush == null || bush.cleared)
            return;
        bush.cleared = true;
        StorySfx.PlayPop();
        StartCoroutine(FadeBush(bush));
    }

    IEnumerator FadeBush(BushHandle bush)
    {
        var group = bush.GetComponent<CanvasGroup>();
        if (group == null)
            group = bush.gameObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
        float t = 0f;
        while (t < 0.55f)
        {
            t += Time.deltaTime;
            group.alpha = 1f - (t / 0.55f);
            yield return null;
        }
        bush.gameObject.SetActive(false);
        left--;
        if (left <= 0)
        {
            StoryProgress.Mark(pageId);
            Finish(true);
        }
    }

    public void ClickMiss()
    {
        if (StoryProgress.IsDone(pageId))
            return;
        SetHint("Click it once more");
    }

    void Finish(bool animate = true)
    {
        if (bushes != null)
        {
            foreach (var bush in bushes)
            {
                if (bush != null) bush.gameObject.SetActive(false);
            }
        }
        SetActive(showWhenDone, true);
        SetActive(hideWhenDone, false);
        if (moonBefore != null || moonAfter != null)
            StartCoroutine(SwapMoon(animate));
        Unlock();
        SetHint(hintDone);
    }

    IEnumerator SwapMoon(bool animate)
    {
        var from = moonBefore == null ? null : moonBefore.GetComponent<Graphic>();
        var to = moonAfter == null ? null : moonAfter.GetComponent<Graphic>();
        if (moonAfter != null)
            moonAfter.SetActive(true);
        if (!animate || moonFadeSeconds <= 0f)
        {
            SetAlpha(from, 0f);
            if (moonBefore != null) moonBefore.SetActive(false);
            SetAlpha(to, 1f);
            yield break;
        }
        SetAlpha(to, 0f);
        float t = 0f;
        while (t < moonFadeSeconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / moonFadeSeconds);
            SetAlpha(from, 1f - k);
            SetAlpha(to, k);
            yield return null;
        }
        SetAlpha(from, 0f);
        if (moonBefore != null) moonBefore.SetActive(false);
        SetAlpha(to, 1f);
    }

    static void SetAlpha(Graphic graphic, float alpha)
    {
        if (graphic == null) return;
        var color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }

    void Unlock()
    {
        var nav = GetComponent<NavButtons>();
        if (nav != null) nav.UnlockNext();
    }

    static void SetActive(GameObject[] items, bool value)
    {
        if (items == null) return;
        foreach (var item in items)
        {
            if (item != null) item.SetActive(value);
        }
    }

    void SetHint(string value)
    {
        if (hint != null) hint.text = value;
    }
}
