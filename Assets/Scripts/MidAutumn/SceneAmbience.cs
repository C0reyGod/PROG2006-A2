using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAmbience : MonoBehaviour
{
    static SceneAmbience instance;
    AudioSource source;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<SceneAmbience>() != null)
            return;
        var go = new GameObject("SceneAmbience");
        go.AddComponent<SceneAmbience>();
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        SceneManager.sceneLoaded += OnScene;
    }

    void Start()
    {
        Play(SceneManager.GetActiveScene().name);
    }

    void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnScene;
    }

    void OnScene(Scene scene, LoadSceneMode mode)
    {
        Play(scene.name);
    }

    void Play(string sceneName)
    {
        string key = "Audio/amb_home";
        if (sceneName == StoryPages.MoonGazing)
            key = "Audio/amb_village";
        else if (sceneName == StoryPages.ChasingLight)
            key = "Audio/amb_forest";
        else if (sceneName == StoryPages.LostAndFound)
            key = "Audio/amb_dark";
        else if (sceneName == StoryPages.MoonGoddess)
            key = "Audio/amb_chime";
        else if (sceneName == StoryPages.Credits)
            key = "Audio/amb_chime";
        var clip = Resources.Load<AudioClip>(key);
        if (clip == null)
            clip = Resources.Load<AudioClip>("Audio/amb_night");
        if (clip == null || source == null)
            return;
        if (source.clip == clip && source.isPlaying)
            return;
        source.clip = clip;
        source.volume = 0.2f;
        source.Play();
    }
}
