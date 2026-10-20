using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GiftTask : MonoBehaviour
{
    public string pageId = StoryPages.MoonGoddess;
    public Button missButton;
    public GameObject mooncakeInHand;
    public GameObject mooncakeGiven;
    public GameObject chippedMoon;
    public GameObject fullMoon;
    public RectTransform rabbitBody;
    public RectTransform returningLight;
    public RectTransform moonPoint;
    public TMP_Text hint;
    public string hintStart = "Give the mooncake to the rabbit";
    public string hintDone = "The moon is whole again.";
    public bool finished;

    void Start()
    {
        if (missButton != null)
            missButton.onClick.AddListener(ClickMiss);
        bool done = StoryProgress.IsDone(pageId);
        finished = done;
        Show(done);
        if (done)
        {
            if (returningLight != null)
                returningLight.gameObject.SetActive(false);
            Unlock();
        }
        SetHint(done ? hintDone : hintStart);
    }

    public void CompleteGift()
    {
        if (finished)
            return;
        finished = true;
        StoryProgress.Mark(pageId);
        StorySfx.PlayDing();
        if (mooncakeInHand != null) mooncakeInHand.SetActive(false);
        if (mooncakeGiven != null) mooncakeGiven.SetActive(true);
        StartCoroutine(Hop());
        StartCoroutine(Mend());
        SetHint(hintDone);
    }

    public void ClickMiss()
    {
        if (finished)
            return;
        SetHint("Click it once more");
    }

    void Show(bool done)
    {
        if (mooncakeInHand != null) mooncakeInHand.SetActive(!done);
        if (mooncakeGiven != null) mooncakeGiven.SetActive(done);
        if (chippedMoon != null) chippedMoon.SetActive(!done);
        if (fullMoon != null) fullMoon.SetActive(done);
    }

    IEnumerator Hop()
    {
        if (rabbitBody == null)
            yield break;
        var start = rabbitBody.anchoredPosition;
        float t = 0f;
        while (t < 0.32f)
        {
            t += Time.deltaTime;
            float y = Mathf.Sin(t / 0.32f * Mathf.PI) * 34f;
            rabbitBody.anchoredPosition = start + new Vector2(0f, y);
            yield return null;
        }
        rabbitBody.anchoredPosition = start;
    }

    IEnumerator Mend()
    {
        if (returningLight != null && moonPoint != null)
        {
            returningLight.gameObject.SetActive(true);
            returningLight.SetAsLastSibling();
            var start = returningLight.anchoredPosition;
            var end = moonPoint.anchoredPosition;
            float t = 0f;
            while (t < 0.75f)
            {
                t += Time.deltaTime;
                returningLight.anchoredPosition = Vector2.Lerp(start, end, t / 0.75f);
                yield return null;
            }
            returningLight.gameObject.SetActive(false);
        }
        if (chippedMoon != null) chippedMoon.SetActive(false);
        if (fullMoon != null) fullMoon.SetActive(true);
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
