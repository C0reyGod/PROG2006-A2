using UnityEngine;

public class ReplayConfirm : MonoBehaviour
{
    public void Yes()
    {
        var nav = FindObjectOfType<NavButtons>();
        if (nav != null)
            nav.Replay();
        else
        {
            StoryProgress.ResetAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene(StoryPages.Home);
        }
    }

    public void No()
    {
        gameObject.SetActive(false);
    }
}
