using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressPop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Vector3 restScale = Vector3.one;
    bool down;

    void Awake()
    {
        restScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        down = true;
        transform.localScale = restScale * 0.86f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        down = false;
        transform.localScale = restScale;
    }

    void OnDisable()
    {
        if (!down)
            return;
        down = false;
        transform.localScale = restScale;
    }
}
