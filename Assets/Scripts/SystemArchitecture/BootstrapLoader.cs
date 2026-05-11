using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private GameObject settingsManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;
    [SerializeField] private GameObject brightnessOverlayPrefab;
    [SerializeField] private GameObject metricsManagerPrefab;
    [SerializeField] private GameObject gameManagerPrefab;

    private void Awake()
    {
        Instantiate(brightnessOverlayPrefab);
        Instantiate(settingsManagerPrefab);
        Instantiate(audioManagerPrefab);
        Instantiate(metricsManagerPrefab);
        Instantiate(gameManagerPrefab);
    }

    private void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
