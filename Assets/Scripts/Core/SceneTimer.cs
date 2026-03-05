using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTimer : MonoBehaviour
{
    [Header("Scene Timing")]
    public float sceneDuration = 10f;
    public int nextSceneIndex = 0;

    private bool hasLoadedNextScene = false;

    void Start()
    {
        StartCoroutine(SceneCountdown());
    }

    IEnumerator SceneCountdown()
    {
        yield return new WaitForSeconds(sceneDuration);
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        if (hasLoadedNextScene) return;

        hasLoadedNextScene = true;

        // Si estamos en simulación completa
        if (SimulationManager.fullSimulation)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Si es una escena individual, volver al menú
            SimulationManager.fullSimulation = false;
            SceneManager.LoadScene(1);
        }
    }
}