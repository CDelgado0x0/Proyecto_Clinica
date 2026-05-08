using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class SettingsData
{
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;
    public float dialogFontSize = 24f;
    public float brightness = 1f;
    public int sceneDuration = 0;
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public SettingsData CurrentSettings { get; private set; }
    private string SavePath => Application.persistentDataPath + "/settings.json";
    public string MetricsPath => Application.persistentDataPath + "/metrics.json";

    private readonly float[] sceneDurations = { 120f, 300f, 600f }; //Guardamos las posibles duraciones de la escena

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        CurrentSettings = LoadSettings();
        ApplySettings();
    }

    private SettingsData LoadSettings()  //Si existe un JSON lo lee y devuelve sus valores, de lo contrario crea uno y devuelve valores default
    {
        if (!File.Exists(SavePath))
            return new SettingsData();
        try
        {
            return JsonUtility.FromJson<SettingsData>(File.ReadAllText(SavePath));
        }
        catch
        {
            Debug.LogWarning("Settings corruptos, cargando valores por defecto.");
            return new SettingsData();
        }
    }

    public void SaveSettings()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(CurrentSettings, prettyPrint: true));
    }

    public void ApplySettings()
    {
        SetAmbientVolume(CurrentSettings.ambientVolume);
        SetSfxVolume(CurrentSettings.sfxVolume);
        SetBrightness(CurrentSettings.brightness);
    }

    public void SetAmbientVolume(float value)
    {
        CurrentSettings.ambientVolume = value;
        // Notificar al audioManager!!
    }

    public void SetSfxVolume(float value)
    {
        CurrentSettings.sfxVolume = value;
    }

    public void SetDialogFontSize(float value)
    {
        CurrentSettings.dialogFontSize = value;
    }

    public void SetBrightness(float value)
    {
        CurrentSettings.brightness = value;
        BrightnessOverlay.Instance.SetBrightness(value);
    }

    public void SetSceneDuration(int index)
    {
        CurrentSettings.sceneDuration = index;
    }

    public float GetSceneDuration()
    {
        return sceneDurations[CurrentSettings.sceneDuration];
    }
}
