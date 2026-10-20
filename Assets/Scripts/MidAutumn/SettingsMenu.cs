using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject panel;
    public Button openButton;
    public Button musicButton;
    public Button closeButton;
    public Slider volumeSlider;
    public Slider textSlider;
    public TMP_Text volumeLabel;
    public TMP_Text textLabel;

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.SetValueWithoutNotify(GameSettings.BgmVolume);
            volumeSlider.onValueChanged.AddListener(OnVolume);
        }

        if (textSlider != null)
        {
            textSlider.minValue = 0.75f;
            textSlider.maxValue = 1.65f;
            textSlider.SetValueWithoutNotify(GameSettings.TextScale);
            textSlider.onValueChanged.AddListener(OnText);
        }

        if (openButton != null) openButton.onClick.AddListener(Open);
        if (musicButton != null) musicButton.onClick.AddListener(ToggleMusic);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        RefreshLabels();
        if (panel != null) panel.SetActive(false);
    }

    public void ToggleMusic()
    {
        GameSettings.ToggleMusic();
    }

    public void Open()
    {
        if (panel != null) panel.SetActive(true);
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(GameSettings.BgmVolume);
        if (textSlider != null) textSlider.SetValueWithoutNotify(GameSettings.TextScale);
        RefreshLabels();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
    }

    void OnVolume(float value)
    {
        GameSettings.SetBgmVolume(value);
        RefreshLabels();
    }

    void OnText(float value)
    {
        GameSettings.SetTextScale(value);
        RefreshLabels();
    }

    void RefreshLabels()
    {
        if (volumeLabel != null)
            volumeLabel.text = Mathf.RoundToInt(GameSettings.BgmVolume * 100f) + "%";
        if (textLabel != null)
            textLabel.text = Mathf.RoundToInt(GameSettings.TextScale * 100f) + "%";
    }
}
