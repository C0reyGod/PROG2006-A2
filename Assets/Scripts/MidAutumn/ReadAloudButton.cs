using UnityEngine;
using UnityEngine.UI;

public class ReadAloudButton : MonoBehaviour
{
    AudioSource voice;
    AudioClip clip;
    bool speaking;

    void Awake()
    {
        voice = gameObject.AddComponent<AudioSource>();
        voice.playOnAwake = false;
        voice.loop = false;
        voice.spatialBlend = 0f;
        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(Toggle);
        clip = Resources.Load<AudioClip>("Audio/narration_" + gameObject.scene.name);
    }

    public void Toggle()
    {
        if (clip == null || voice == null)
            return;
        if (voice.isPlaying)
        {
            StopSpeaking();
            return;
        }

        voice.clip = clip;
        voice.volume = 1f;
        voice.Play();
        speaking = true;
        if (BgmPlayer.Instance != null)
            BgmPlayer.Instance.SetDuck(true);
    }

    void Update()
    {
        if (speaking && voice != null && !voice.isPlaying)
            StopSpeaking();
    }

    void OnDisable()
    {
        StopSpeaking();
    }

    void StopSpeaking()
    {
        if (voice != null && voice.isPlaying)
            voice.Stop();
        if (!speaking)
            return;
        speaking = false;
        if (BgmPlayer.Instance != null)
            BgmPlayer.Instance.SetDuck(false);
    }
}
