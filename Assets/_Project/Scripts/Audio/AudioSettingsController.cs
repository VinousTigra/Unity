using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    [SerializeField]
    private string musicParameter = "MusicVolume";

    [SerializeField]
    private string sfxParameter = "SFXVolume";

    public void SetMusicVolume(float value)
    {
        SetVolume(musicParameter, value);
    }

    public void SetSfxVolume(float value)
    {
        SetVolume(sfxParameter, value);
    }

    private void SetVolume(
        string parameter,
        float linearValue)
    {
        float value =
            Mathf.Clamp(linearValue, 0.0001f, 1f);

        float decibels =
            Mathf.Log10(value) * 20f;

        mixer.SetFloat(parameter, decibels);
    }
}