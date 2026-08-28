using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderUI : MonoBehaviour
{
    public enum VolumeType { SFX, Ambient }

    [SerializeField] private Slider slider;
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        float currentValue = volumeType == VolumeType.SFX
            ? AudioManager.Instance.GetSfxVolume()
            : AudioManager.Instance.GetAmbientVolume();

        slider.SetValueWithoutNotify(currentValue);

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        if (volumeType == VolumeType.SFX)
            AudioManager.Instance.SetSfxVolume(value);
        else
            AudioManager.Instance.SetAmbientVolume(value);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}