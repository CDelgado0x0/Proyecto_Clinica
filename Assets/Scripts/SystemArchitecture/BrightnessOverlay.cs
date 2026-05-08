using UnityEngine;
using UnityEngine.UI;

public class BrightnessOverlay : MonoBehaviour
{
    public static BrightnessOverlay Instance { get; private set; }

    [SerializeField] private Image overlayImage;

    private void Awake()
    {
        if (Instance != null) { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SetBrightness(float value)
    {
        float alpha = Mathf.Lerp(0.8f, 0f, value);
        overlayImage.color = new Color(0f, 0f, 0f, alpha);
    }
}
