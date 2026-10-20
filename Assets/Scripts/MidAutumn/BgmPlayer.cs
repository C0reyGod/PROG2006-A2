using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BgmPlayer : MonoBehaviour
{
    public static BgmPlayer Instance { get; private set; }

    [Tooltip("Drop a music clip here later. Volume still works while this is empty.")]
    public AudioClip music;

    AudioSource source;
    bool ducked;

    public void SetDuck(bool on)
    {
        ducked = on;
        ApplyVolume();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        source = GetComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        if (music == null)
            music = Resources.Load<AudioClip>("Audio/bgm_lullaby");
        ApplyVolume();
        TryPlay();
        GameSettings.Changed += ApplyVolume;
    }

    void Update()
    {
        if (music != null && source.clip != music)
            TryPlay();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            GameSettings.Changed -= ApplyVolume;
            Instance = null;
        }
    }

    public void ApplyVolume()
    {
        if (source != null)
        {
            float volume = GameSettings.MusicOn ? GameSettings.BgmVolume * 0.42f : 0f;
            if (ducked)
                volume *= 0.35f;
            source.volume = volume;
        }
    }

    void TryPlay()
    {
        if (music == null || source == null)
            return;
        source.clip = music;
        source.volume = GameSettings.MusicOn ? GameSettings.BgmVolume * 0.42f : 0f;
        if (!source.isPlaying)
            source.Play();
    }
}
