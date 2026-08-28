using UnityEngine;
using UnityEngine.Audio;

// Se ejecuta ANTES que SettingsManager (que usa el orden por defecto, 0)
// para garantizar que AudioManager.Instance ya exista cuando
// SettingsManager.Awake() llame a ApplySettings().
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixers")]
    [SerializeField] private AudioMixer sfxMixer;
    [SerializeField] private AudioMixer ambientMixer;

    [Header("Exposed Parameters")]
    [SerializeField] private string sfxParam = "SFXVolume";
    [SerializeField] private string ambientParam = "AmbientVolume";

    [Header("Valores por defecto (0 a 1)")]
    [SerializeField] private float defaultSfxVolume = 0.75f;
    [SerializeField] private float defaultAmbientVolume = 0f;
    [SerializeField] private float clinicalAmbientVolume = 0f;

    // AudioManager ya NO persiste nada. SettingsManager (settings.json)
    // es la única fuente de verdad; aquí solo cacheamos en memoria
    // el último valor aplicado, para poder devolverlo con los getters.
    private float currentSfxVolume;
    private float currentAmbientVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Valor provisional hasta que SettingsManager (si existe) aplique
        // los valores reales cargados desde settings.json.
        currentSfxVolume = defaultSfxVolume;
        currentAmbientVolume = defaultAmbientVolume;
        //ApplySfxVolume(currentSfxVolume);
        //ApplyAmbientVolume(currentAmbientVolume);
    }

    // ---------- SFX ----------
    public void SetSfxVolume(float linearValue)
    {
        currentSfxVolume = linearValue;
        ApplySfxVolume(linearValue);
    }

    private void ApplySfxVolume(float linearValue)
    {
        float dB = LinearToDecibel(linearValue);
        sfxMixer.SetFloat(sfxParam, dB);
    }

    public float GetSfxVolume()
    {
        return currentSfxVolume;
    }

    // ---------- Ambiente ----------
    public void SetAmbientVolume(float linearValue)
    {
        currentAmbientVolume = linearValue;
        ApplyAmbientVolume(linearValue);
    }

    private void ApplyAmbientVolume(float linearValue)
    {
        float dB = LinearToDecibel(linearValue);
        ambientMixer.SetFloat(ambientParam, dB);
        Debug.Log($"Ambient Volume set to {linearValue} (dB: {dB})");
    }

    public float GetAmbientVolume()
    {
        return currentAmbientVolume;
    }


    //Cambiar los valores dentro de la clinica

    public void SetClinicalAmbientVolume()
    {
        float dB = LinearToDecibel(clinicalAmbientVolume);
        ambientMixer.SetFloat(ambientParam, dB);
    }

    public void SetNormalAmbientVolume()
    {
        float dB = LinearToDecibel(currentAmbientVolume);
        ambientMixer.SetFloat(ambientParam, dB);
    }


    // ---------- Utilidad ----------
    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}