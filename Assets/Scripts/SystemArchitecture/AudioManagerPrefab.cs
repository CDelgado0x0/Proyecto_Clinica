using UnityEngine;
using UnityEngine.Audio;

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
    [SerializeField] private float defaultAmbientVolume = 0.75f;

    private const string SFX_KEY = "SFXVolume";
    private const string AMBIENT_KEY = "AmbientVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAndApplyVolumes();
    }

    private void LoadAndApplyVolumes()
    {
        float sfxVolume = PlayerPrefs.GetFloat(SFX_KEY, defaultSfxVolume);
        float ambientVolume = PlayerPrefs.GetFloat(AMBIENT_KEY, defaultAmbientVolume);

        ApplySfxVolume(sfxVolume);
        ApplyAmbientVolume(ambientVolume);
    }

    // ---------- SFX ----------
    public void SetSfxVolume(float linearValue)
    {
        ApplySfxVolume(linearValue);
        PlayerPrefs.SetFloat(SFX_KEY, linearValue);
        PlayerPrefs.Save();
    }

    private void ApplySfxVolume(float linearValue)
    {
        float dB = LinearToDecibel(linearValue);
        sfxMixer.SetFloat(sfxParam, dB);
    }

    public float GetSfxVolume()
    {
        return PlayerPrefs.GetFloat(SFX_KEY, defaultSfxVolume);
    }

    // ---------- Ambiente ----------
    public void SetAmbientVolume(float linearValue)
    {
        ApplyAmbientVolume(linearValue);
        PlayerPrefs.SetFloat(AMBIENT_KEY, linearValue);
        PlayerPrefs.Save();
    }

    private void ApplyAmbientVolume(float linearValue)
    {
        float dB = LinearToDecibel(linearValue);
        ambientMixer.SetFloat(ambientParam, dB);
    }

    public float GetAmbientVolume()
    {
        return PlayerPrefs.GetFloat(AMBIENT_KEY, defaultAmbientVolume);
    }

    // ---------- Utilidad ----------
    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}