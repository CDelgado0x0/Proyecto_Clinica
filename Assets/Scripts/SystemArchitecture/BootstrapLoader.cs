using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private GameObject settingsManagerPrefab;
    [SerializeField] private GameObject brightnessOverlayPrefab;
    [SerializeField] private GameObject metricsManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;

    private void Awake()
    {
        Instantiate(audioManagerPrefab);
        Instantiate(brightnessOverlayPrefab);
        Instantiate(settingsManagerPrefab);
        Instantiate(metricsManagerPrefab);
    }

    private void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
