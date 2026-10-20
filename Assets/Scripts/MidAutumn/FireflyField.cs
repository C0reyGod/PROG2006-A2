using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireflyField : MonoBehaviour
{
    public int count = 10;
    bool built;

    // The forest pages get drifting fireflies as soon as they load.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawn(SceneManager.GetActiveScene().name);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Spawn(scene.name);
    }

    static void Spawn(string sceneName)
    {
        if (sceneName != StoryPages.ChasingLight && sceneName != StoryPages.LostAndFound)
            return;
        var stage = GameObject.Find("Stage");
        if (stage == null)
            return;
        var parent = stage.transform;
        if (parent.Find("Fireflies") != null)
            return;
        var go = new GameObject("Fireflies", typeof(RectTransform), typeof(FireflyField));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.transform.SetAsFirstSibling();
    }

    void Start()
    {
        if (built)
            return;
        built = true;
        var dot = Resources.Load<Sprite>("UI/firefly");
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject("Firefly", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(14f, 14f);
            rt.anchoredPosition = new Vector2(Random.Range(-420f, 420f), Random.Range(-80f, 160f));
            var image = go.GetComponent<Image>();
            image.sprite = dot;
            image.raycastTarget = false;
            image.color = new Color(1f, 0.92f, 0.45f, Random.Range(0.35f, 0.9f));
            var fly = go.AddComponent<FireflyMove>();
            fly.speed = Random.Range(8f, 18f);
            fly.phase = Random.Range(0f, 6f);
        }
    }
}

public class FireflyMove : MonoBehaviour
{
    public float speed = 12f;
    public float phase;
    RectTransform rect;
    Vector2 origin;

    void Awake()
    {
        rect = transform as RectTransform;
        origin = rect.anchoredPosition;
    }

    void Update()
    {
        float t = Time.time + phase;
        rect.anchoredPosition = origin + new Vector2(Mathf.Sin(t * 0.35f) * 36f, Mathf.Cos(t * 0.5f) * 18f);
        var image = GetComponent<Image>();
        if (image != null)
        {
            var color = image.color;
            color.a = 0.25f + Mathf.Abs(Mathf.Sin(t * 1.7f)) * 0.7f;
            image.color = color;
        }
        origin += new Vector2(speed * Time.deltaTime * 0.15f, 0f);
        if (origin.x > 460f)
            origin.x = -460f;
    }
}
