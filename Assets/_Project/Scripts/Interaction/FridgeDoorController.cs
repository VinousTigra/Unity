using System.Collections;
using UnityEngine;

public class FridgeDoorController : InteractableBase
{
    [SerializeField] private Animator animator;

    [SerializeField]
    private string openParameter = "IsOpen";

    [SerializeField]
    private float spoilAfterOpenSeconds = 200f;

    [SerializeField]
    private WorldIngredient[] refrigeratedIngredients;

    private bool isOpen;
    private Coroutine spoilCoroutine;

    public bool IsOpen => isOpen;

    public override void Interact()
    {
        isOpen = !isOpen;

        if (animator != null)
        {
            animator.SetBool(openParameter, isOpen);
        }

        GameEvents.RaiseFridgeStateChanged(isOpen);

        GameEvents.ShowStatus(
            isOpen
                ? "Холодильник открыт"
                : "Холодильник закрыт"
        );

        if (isOpen)
        {
            StartSpoilTimer();
        }
        else
        {
            StopSpoilTimer();
        }
    }

    private void StartSpoilTimer()
    {
        StopSpoilTimer();

        spoilCoroutine =
            StartCoroutine(SpoilRoutine());
    }

    private void StopSpoilTimer()
    {
        if (spoilCoroutine == null)
        {
            return;
        }

        StopCoroutine(spoilCoroutine);
        spoilCoroutine = null;
    }

    private IEnumerator SpoilRoutine()
    {
        yield return new WaitForSeconds(
            spoilAfterOpenSeconds
        );

        foreach (WorldIngredient ingredient
                 in refrigeratedIngredients)
        {
            if (ingredient != null)
            {
                ingredient.MarkSpoiled();
            }
        }

        GameEvents.ShowStatus(
            "Продукты в открытом холодильнике испортились!"
        );

        spoilCoroutine = null;
    }

    private void OnDisable()
    {
        StopSpoilTimer();
    }
}