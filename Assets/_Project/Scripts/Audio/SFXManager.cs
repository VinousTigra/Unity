using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource stirAudioSource;
    [SerializeField] private AudioSource fryAudioSource;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameEvents.StatusMessage += OnStatusMessage;
        GameEvents.Stirred += OnStirred;
        GameEvents.FryingStarted += OnFryingStarted;
        GameEvents.FryingStopped += OnFryingStopped;
    }

    private void OnDisable()
    {
        GameEvents.StatusMessage -= OnStatusMessage;
        GameEvents.Stirred -= OnStirred;
        GameEvents.FryingStarted -= OnFryingStarted;
        GameEvents.FryingStopped -= OnFryingStopped;
    }

    private void OnStirred()
    {
        if (stirAudioSource != null)
            stirAudioSource.Play();
    }

    private void OnFryingStarted()
    {
        if (fryAudioSource != null &&
            !fryAudioSource.isPlaying)
        {
            fryAudioSource.Play();
        }
    }

    private void OnFryingStopped()
    {
        if (fryAudioSource != null)
        {
            fryAudioSource.Stop();
        }
    }

    private void OnStatusMessage(string message)
    {
        if (message.StartsWith("Готово:"))
        {
            audioSource.Play();
        }
    }
}