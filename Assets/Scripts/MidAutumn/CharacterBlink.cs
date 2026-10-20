using UnityEngine;
using UnityEngine.UI;

public class CharacterBlink : MonoBehaviour
{
    void Awake()
    {
        var image = GetComponent<Image>();
        if (image != null)
            image.raycastTarget = false;
    }
}
