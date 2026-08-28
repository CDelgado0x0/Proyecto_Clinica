using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private GameObject settingsManagerPrefab;
    [SerializeField] private GameObject brightnessOverlayPrefab;
    [SerializeField] private GameObject metricsManagerPrefab;

    private void Awake()
    {
        Instantiate(brightnessOverlayPrefab);
        Instantiate(settingsManagerPrefab);
        Instantiate(metricsManagerPrefab);
    }

    private void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
