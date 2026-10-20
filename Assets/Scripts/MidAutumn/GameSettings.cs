using System;
using UnityEngine;

public static class GameSettings
{
    public const float DefaultVolume = 0.4f;
    public const float DefaultTextScale = 1f;
    const string VolumeKey = "ma_bgm_volume";
    const string TextKey = "ma_text_scale";

    const string MusicKey = "ma_music_on";

    public static float BgmVolume { get; private set; }
    public static float TextScale { get; private set; }
    public static bool MusicOn { get; private set; }
    public static event Action Changed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Boot()
    {
        BgmVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, DefaultVolume));
        TextScale = Mathf.Clamp(PlayerPrefs.GetFloat(TextKey, DefaultTextScale), 0.75f, 1.65f);
        MusicOn = PlayerPrefs.GetInt(MusicKey, 1) == 1;
    }

    public static void ToggleMusic()
    {
        MusicOn = !MusicOn;
        PlayerPrefs.SetInt(MusicKey, MusicOn ? 1 : 0);
        Changed?.Invoke();
    }

    public static void SetBgmVolume(float value)
    {
        BgmVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(VolumeKey, BgmVolume);
        Changed?.Invoke();
    }

    public static void SetTextScale(float value)
    {
        TextScale = Mathf.Clamp(value, 0.75f, 1.65f);
        PlayerPrefs.SetFloat(TextKey, TextScale);
        Changed?.Invoke();
    }
}
