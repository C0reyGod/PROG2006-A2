using UnityEngine;
using UnityEngine.UI;

public class LockBadge : MonoBehaviour
{
    public GameObject badge;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        if (badge == null)
        {
            var child = transform.Find("LockBadge");
            if (child != null)
                badge = child.gameObject;
        }
    }

    void LateUpdate()
    {
        if (badge == null || button == null)
            return;
        bool show = !button.interactable;
        if (badge.activeSelf != show)
            badge.SetActive(show);
    }
}
