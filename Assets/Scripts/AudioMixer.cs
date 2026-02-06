using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider slider;
    [SerializeField] private string exposedParam;

    [Header("Default Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultValue = 0.75f;

    private const float MIN_VOLUME = 0.0001f;

    void Start()
    {
        float savedValue = PlayerPrefs.GetFloat(exposedParam, defaultValue);
        slider.value = savedValue;

        slider.onValueChanged.AddListener(SetVolume);
        SetVolume(slider.value);
    }

    public void SetVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Max(value, MIN_VOLUME)) * 20f;
        mixer.SetFloat(exposedParam, volume);

        PlayerPrefs.SetFloat(exposedParam, value);
    }

}
