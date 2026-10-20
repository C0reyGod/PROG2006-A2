using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavButtons : MonoBehaviour
{
    public string backScene;
    public string nextScene;
    public Button backButton;
    public Button homeButton;
    public Button nextButton;
    public Button nextGate;
    public Button startButton;
    public Button replayButton;
    public bool nextLocked = true;
    public bool nextBlocked;
    public TMP_Text hint;
    public string lockedMessage = "Finish this page first.";

    void Start()
    {
        if (backButton != null) backButton.onClick.AddListener(GoBack);
        if (homeButton != null) homeButton.onClick.AddListener(GoHome);
        if (nextButton != null) nextButton.onClick.AddListener(GoNext);
        if (nextGate != null)
        {
            nextGate.onClick.AddListener(OnLockedClick);
            PressRelay.Ensure(nextGate, OnLockedClick);
        }
        if (startButton != null) startButton.onClick.AddListener(GoStart);
        if (replayButton != null) replayButton.onClick.AddListener(AskReplay);
        ApplyNextLock();
    }

    public void GoBack()
    {
        Load(backScene);
    }

    public void GoHome()
    {
        Load(StoryPages.Home);
    }

    public void GoNext()
    {
        if (nextBlocked || nextLocked)
            return;
        Load(nextScene);
    }

    public void OnLockedClick()
    {
        if (nextBlocked || !nextLocked)
            return;
        if (hint != null)
            hint.text = string.IsNullOrEmpty(lockedMessage) ? "Finish this page first." : lockedMessage;
    }

    public void UnlockNext()
    {
        nextLocked = false;
        ApplyNextLock();
    }

    void ApplyNextLock()
    {
        if (nextButton != null)
            nextButton.interactable = !nextBlocked && !nextLocked;
        if (nextGate != null)
            nextGate.gameObject.SetActive(!nextBlocked && nextLocked);
    }

    public void GoStart()
    {
        Load(StoryPages.MoonGazing);
    }

    public void AskReplay()
    {
        var confirms = Object.FindObjectsOfType<ReplayConfirm>(true);
        if (confirms != null && confirms.Length > 0)
        {
            var panel = confirms[0].gameObject;
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();
            return;
        }
        Replay();
    }

    public void Replay()
    {
        StoryProgress.ResetAll();
        Load(StoryPages.Home);
    }

    static void Load(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
            SceneManager.LoadScene(sceneName);
    }
}
