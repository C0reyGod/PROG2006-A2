using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BushHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    BushTask task;
    int index;
    RectTransform rect;
    Image image;
    Color restColor = Color.white;
    Vector3 restScale = Vector3.one;
    bool pressed;
    public bool cleared;

    public void Bind(BushTask owner, int bushIndex)
    {
        task = owner;
        index = bushIndex;
        rect = transform as RectTransform;
        image = GetComponent<Image>();
        if (image != null)
            restColor = image.color;
        restScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        if (cleared)
            return;
        transform.localScale = new Vector3(restScale.x * 0.88f, restScale.y * 0.88f, 1f);
        if (image != null)
            image.color = restColor * 0.72f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!pressed || cleared || rect == null)
            return;
        float scale = 1f;
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.scaleFactor > 0.01f)
            scale = canvas.scaleFactor;
        rect.anchoredPosition += eventData.delta / scale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!pressed || cleared)
            return;
        pressed = false;
        transform.localScale = restScale;
        if (image != null)
            image.color = restColor;
        if (task != null)
            task.ClearBush(index);
    }
}
