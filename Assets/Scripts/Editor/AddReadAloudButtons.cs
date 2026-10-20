using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class AddReadAloudButtons
{
    [MenuItem("MidAutumn/Add Read Aloud Buttons")]
    public static void AddAll()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Stop play mode before adding the read button.");
            return;
        }

        var icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/UI/read_icon.png");
        string[] scenes =
        {
            StoryPages.MoonGazing,
            StoryPages.ChasingLight,
            StoryPages.LostAndFound,
            StoryPages.MoonGoddess,
            StoryPages.Credits
        };

        foreach (var sceneName in scenes)
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MidAutumn/" + sceneName + ".unity");
            if (GameObject.Find("ReadAloudButton") != null)
            {
                Debug.Log(sceneName + " already has the read button.");
                continue;
            }

            var canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError(sceneName + " has no canvas.");
                continue;
            }

            var go = new GameObject("ReadAloudButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(ReadAloudButton));
            go.transform.SetParent(canvas.transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(12f, -12f);
            rt.sizeDelta = new Vector2(68f, 68f);
            var image = go.GetComponent<Image>();
            image.sprite = icon;
            image.preserveAspect = true;
            image.raycastTarget = true;
            var button = go.GetComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
            colors.selectedColor = Color.white;
            colors.fadeDuration = 0.06f;
            button.colors = colors;
            go.transform.SetAsLastSibling();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Added read button to " + sceneName);
        }
    }
}
