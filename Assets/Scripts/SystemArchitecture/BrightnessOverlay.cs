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

    #if UNITY_EDITOR //Esto sirve para crearlo en caso de que haciendo pruebas no se lance la aplicación desde el bootstrap, sino desde una escena del juego. No hace falta que el script esté asociado a ningun gameobject de la escena.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance != null) return; // ya existe, no hace nada

            GameObject obj = new GameObject("BrightnessManager_Auto");
            obj.AddComponent<BrightnessOverlay>();
        }
    #endif

    public void SetBrightness(float value)
    {
        if (overlayImage == null) return;
        float alpha = Mathf.Lerp(0.8f, 0f, value);
        overlayImage.color = new Color(0f, 0f, 0f, alpha);
    }
}
