using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GiftDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GiftTask task;
    public RectTransform rabbit;
    RectTransform rect;
    Vector2 origin;
    bool dragging;
    bool moved;

    void Awake()
    {
        rect = transform as RectTransform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (task != null && task.finished)
            return;
        dragging = true;
        moved = false;
        if (rect != null)
            origin = rect.anchoredPosition;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging || rect == null)
            return;
        float scale = 1f;
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.scaleFactor > 0.01f)
            scale = canvas.scaleFactor;
        rect.anchoredPosition += eventData.delta / scale;
        if ((rect.anchoredPosition - origin).sqrMagnitude > 36f)
            moved = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!dragging)
            return;
        dragging = false;
        bool over = false;
        if (moved && rabbit != null)
            over = RectTransformUtility.RectangleContainsScreenPoint(rabbit, eventData.position, eventData.pressEventCamera);
        if (!moved || over)
        {
            if (task != null)
                task.CompleteGift();
        }
        else if (rect != null)
        {
            rect.anchoredPosition = origin;
            if (task != null)
                task.ClickMiss();
        }
    }
}
