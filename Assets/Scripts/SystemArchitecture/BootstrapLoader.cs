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
        Instantiate(brightnessOverlayPrefab);
        Instantiate(settingsManagerPrefab);
        Instantiate(metricsManagerPrefab);
        Instantiate(audioManagerPrefab);
    }

    private void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
