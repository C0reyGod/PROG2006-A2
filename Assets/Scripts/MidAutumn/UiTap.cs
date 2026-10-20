using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiTap : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        StorySfx.PlayTap();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Install()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Attach(SceneManager.GetActiveScene());
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Attach(scene);
    }

    // Every real button gets the soft press sound. Overlay catchers that use
    // no visual transition (tap-anywhere areas) are skipped on purpose.
    static void Attach(Scene scene)
    {
        if (!scene.IsValid())
            return;
        foreach (var button in Object.FindObjectsOfType<Button>(true))
        {
            if (button == null || button.transition == Selectable.Transition.None)
                continue;
            if (button.GetComponent<UiTap>() == null)
                button.gameObject.AddComponent<UiTap>();
        }
    }
}
