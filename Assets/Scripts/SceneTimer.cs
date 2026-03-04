using UnityEngine;

public class SceneTimer : MonoBehaviour
{
    public float sceneDuration = 10f;   // tiempo editable
    public int nextSceneIndex = 0;      // escena a cargar después

    void Start()
    {
        Invoke(nameof(LoadNextScene), sceneDuration);
    }

    void LoadNextScene()
    {
        SceneLoader loader = FindFirstObjectByType<SceneLoader>();
        loader.LoadScene(nextSceneIndex);
    }
}