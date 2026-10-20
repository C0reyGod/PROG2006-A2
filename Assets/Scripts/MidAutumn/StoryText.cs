using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class StoryText : MonoBehaviour
{
    public float baseMaxSize = 26f;
    TMP_Text label;

    void Awake()
    {
        label = GetComponent<TMP_Text>();
        label.enableAutoSizing = true;
        label.fontSizeMin = 12f;
        label.enableWordWrapping = true;
        label.overflowMode = TextOverflowModes.Ellipsis;
    }

    void OnEnable()
    {
        GameSettings.Changed += Apply;
        Apply();
    }

    void OnDisable()
    {
        GameSettings.Changed -= Apply;
    }

    void Apply()
    {
        if (label == null)
            label = GetComponent<TMP_Text>();
        label.fontSizeMax = Mathf.Max(14f, baseMaxSize * GameSettings.TextScale);
        label.fontSize = label.fontSizeMax;
    }
}
