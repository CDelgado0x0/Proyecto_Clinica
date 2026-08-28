using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour, IPointerUpHandler
{
    public enum VolumeType { SFX, Ambient }

    [SerializeField] private Slider slider;
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        if (SettingsManager.Instance == null || slider == null) return;

        float currentValue = volumeType == VolumeType.SFX
            ? SettingsManager.Instance.CurrentSettings.sfxVolume
            : SettingsManager.Instance.CurrentSettings.ambientVolume;

        slider.SetValueWithoutNotify(currentValue);
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    // Se llama en cada frame mientras se arrastra: solo aplica el volumen
    // en memoria (mixer), sin tocar el disco.
    private void OnSliderChanged(float value)
    {
        if (SettingsManager.Instance == null) return;

        if (volumeType == VolumeType.SFX)
            SettingsManager.Instance.SetSfxVolume(value);
        else
            SettingsManager.Instance.SetAmbientVolume(value);
    }

    // Se llama una única vez al soltar el dedo/ratón: aquí sí persistimos a disco.
    public void OnPointerUp(PointerEventData eventData)
    {
        SettingsManager.Instance?.SaveSettings();
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}