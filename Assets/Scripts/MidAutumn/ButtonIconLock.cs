using UnityEngine;
using UnityEngine.UI;

public class ButtonIconLock : MonoBehaviour
{
    public Image icon;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (icon == null)
        {
            var child = transform.Find("Icon");
            if (child != null) icon = child.GetComponent<Image>();
        }
    }

    void LateUpdate()
    {
        if (button == null || icon == null)
            return;
        var tint = button.interactable
            ? Color.white
            : new Color(0.7f, 0.7f, 0.7f, 0.35f);
        if (icon.color != tint)
            icon.color = tint;
    }
}
