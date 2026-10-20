using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class AddNavLabels
{
    [MenuItem("MidAutumn/Add Nav Labels")]
    public static void AddAll()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogError("Stop play mode before adding nav labels.");
            return;
        }

        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        string[] scenes =
        {
            StoryPages.Home,
            StoryPages.MoonGazing,
            StoryPages.ChasingLight,
            StoryPages.LostAndFound,
            StoryPages.MoonGoddess,
            StoryPages.Credits
        };
        string[] names = { "Back", "Home", "Next" };

        foreach (var sceneName in scenes)
        {
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/MidAutumn/" + sceneName + ".unity");
            bool changed = false;
            foreach (var buttonName in names)
            {
                var button = GameObject.Find(buttonName);
                if (button == null)
                {
                    Debug.LogWarning(sceneName + " is missing " + buttonName);
                    continue;
                }
                if (button.transform.Find("Label") != null)
                    continue;

                var go = new GameObject("Label", typeof(RectTransform));
                go.transform.SetParent(button.transform, false);
                var rt = go.GetComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0f, -30f);
                rt.sizeDelta = new Vector2(120f, 22f);

                var tmp = go.AddComponent<TextMeshProUGUI>();
                tmp.font = font;
                tmp.text = buttonName;
                tmp.fontSize = 15f;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.color = new Color(0.36f, 0.18f, 0.05f, 1f);
                tmp.enableWordWrapping = false;
                tmp.overflowMode = TextOverflowModes.Overflow;
                tmp.raycastTarget = false;
                tmp.outlineWidth = 0.18f;
                tmp.outlineColor = new Color(1f, 0.96f, 0.86f, 1f);
                changed = true;
            }

            if (!changed)
            {
                Debug.Log(sceneName + " already has nav labels.");
                continue;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Added nav labels to " + sceneName);
        }
    }
}
