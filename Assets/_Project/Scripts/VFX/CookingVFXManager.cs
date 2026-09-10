using UnityEngine;

public class CookingVFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem fryingSteamVFX;
    [SerializeField] private ParticleSystem burnSmokeVFX;

    private void OnEnable()
    {
        GameEvents.FryingStarted += OnFryingStarted;
        GameEvents.FryingStopped += OnFryingStopped;
        GameEvents.DishBurned += OnDishBurned;
    }

    private void OnDisable()
    {
        GameEvents.FryingStarted -= OnFryingStarted;
        GameEvents.FryingStopped -= OnFryingStopped;
        GameEvents.DishBurned -= OnDishBurned;
    }

    private void OnDishBurned()
    {
        if (burnSmokeVFX != null)
            burnSmokeVFX.Play();
    }

    private void OnFryingStarted()
    {
        if (fryingSteamVFX != null)
            fryingSteamVFX.Play();
    }

    private void OnFryingStopped()
    {
        if (fryingSteamVFX != null)
            fryingSteamVFX.Stop();
    }
}