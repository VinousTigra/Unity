using UnityEngine;

public class GameAudioController : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;

    [SerializeField]
    private AudioSource fryingLoopSource;

    [Header("Clips")]
    [SerializeField] private AudioClip fridgeOpenClip;
    [SerializeField] private AudioClip fridgeCloseClip;

    [SerializeField] private AudioClip stoveReadyClip;

    [SerializeField] private AudioClip saltClip;

    [SerializeField] private AudioClip successClip;
    [SerializeField] private AudioClip failureClip;

    private void OnEnable()
    {
        GameEvents.FridgeStateChanged += OnFridgeStateChanged;
        GameEvents.StoveReady += OnStoveReady;
        GameEvents.SaltApplied += OnSaltApplied;

        GameEvents.FryingStarted += OnFryingStarted;
        GameEvents.FryingStopped += OnFryingStopped;

        GameEvents.DishServed += OnDishServed;
    }

    private void OnDisable()
    {
        GameEvents.FridgeStateChanged -= OnFridgeStateChanged;
        GameEvents.StoveReady -= OnStoveReady;
        GameEvents.SaltApplied -= OnSaltApplied;

        GameEvents.FryingStarted -= OnFryingStarted;
        GameEvents.FryingStopped -= OnFryingStopped;

        GameEvents.DishServed -= OnDishServed;
    }

    private void OnFridgeStateChanged(bool isOpen)
    {
        PlayOneShot(
            isOpen
                ? fridgeOpenClip
                : fridgeCloseClip
        );
    }

    private void OnStoveReady()
    {
        PlayOneShot(stoveReadyClip);
    }

    private void OnSaltApplied()
    {
        PlayOneShot(saltClip);
    }

    private void OnFryingStarted()
    {
        if (fryingLoopSource != null &&
            !fryingLoopSource.isPlaying)
        {
            fryingLoopSource.Play();
        }
    }

    private void OnFryingStopped()
    {
        if (fryingLoopSource != null)
        {
            fryingLoopSource.Stop();
        }
    }

    private void OnDishServed(DishResult result)
    {
        PlayOneShot(
            result.IsSuccessful
                ? successClip
                : failureClip
        );
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}