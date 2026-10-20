using UnityEngine;
using UnityEngine.UI;

public class CloudDrift : MonoBehaviour
{
    public float speed = 10f;
    public bool leader = true;

    RectTransform rect;
    RectTransform wrap;

    void Awake()
    {
        rect = transform as RectTransform;
        var image = GetComponent<Image>();
        if (image != null)
            image.raycastTarget = false;
    }

    void Start()
    {
        if (!leader || rect == null)
            return;

        var copy = Instantiate(gameObject, transform.parent);
        copy.name = gameObject.name + "_wrap";
        var drift = copy.GetComponent<CloudDrift>();
        if (drift != null)
            drift.leader = false;
        wrap = copy.transform as RectTransform;
        copy.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
    }

    void Update()
    {
        if (!leader || rect == null)
            return;

        float width = rect.rect.width;
        if (width < 1f)
            return;

        var pos = rect.anchoredPosition;
        pos.x -= speed * Time.deltaTime;
        if (pos.x <= -width)
            pos.x += width;
        rect.anchoredPosition = pos;
        if (wrap != null)
            wrap.anchoredPosition = pos + new Vector2(width, 0f);
    }
}
