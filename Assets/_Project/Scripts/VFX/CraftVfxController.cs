using UnityEngine;

public class CraftVfxController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem saltParticles;

    [SerializeField]
    private ParticleSystem fryingParticles;

    [SerializeField]
    private ParticleSystem burnedSmoke;

    private void OnEnable()
    {
        GameEvents.SaltApplied += PlaySalt;

        GameEvents.FryingStarted += StartFrying;
        GameEvents.FryingStopped += StopFrying;

        GameEvents.DishBurned += PlayBurned;
        GameEvents.CraftFailed += PlayBurned;
    }

    private void OnDisable()
    {
        GameEvents.SaltApplied -= PlaySalt;

        GameEvents.FryingStarted -= StartFrying;
        GameEvents.FryingStopped -= StopFrying;

        GameEvents.DishBurned -= PlayBurned;
        GameEvents.CraftFailed -= PlayBurned;
    }

    private void PlaySalt()
    {
        if (saltParticles != null)
        {
            saltParticles.Play();
        }
    }

    private void StartFrying()
    {
        if (fryingParticles != null)
        {
            fryingParticles.Play();
        }
    }

    private void StopFrying()
    {
        if (fryingParticles != null)
        {
            fryingParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }
    }

    private void PlayBurned()
    {
        if (burnedSmoke != null)
        {
            burnedSmoke.Play();
        }
    }
}