using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressRelay : MonoBehaviour, IPointerDownHandler
{
    UnityEngine.Events.UnityAction onPress;

    public static void Ensure(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null || action == null)
            return;
        var relay = button.GetComponent<PressRelay>();
        if (relay == null)
            relay = button.gameObject.AddComponent<PressRelay>();
        relay.onPress = action;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPress?.Invoke();
    }
}
