using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftStationController : MonoBehaviour
{
    [SerializeField]
    private CraftStationInventory inventory;

    [SerializeField]
    private StoveController stove;

    [SerializeField]
    private IngredientData saltIngredient;

    [SerializeField]
    private List<RecipeData> recipes = new();

    [Header("Cooking")]
    [SerializeField] private float fryingDuration = 4f;

    [SerializeField]
    private float burnAfterReadySeconds = 4f;

    [Header("Animation")]
    [SerializeField] private Animator stirAnimator;
    [SerializeField] private string stirTrigger = "Stir";

    private readonly List<CraftAction> actions = new();

    private SeasoningState seasoning =
        SeasoningState.Unsalted;

    private CookingState cooking =
        CookingState.Raw;

    private RecipeData currentRecipe;
    private Coroutine fryingCoroutine;

    public CookingState Cooking => cooking;

    public void Stir()
    {
        if (inventory.Ingredients.Count == 0)
        {
            GameEvents.ShowStatus(
                "Нечего перемешивать"
            );

            return;
        }

        if (!actions.Contains(CraftAction.Stir))
        {
            actions.Add(CraftAction.Stir);
        }

        if (stirAnimator != null)
        {
            stirAnimator.SetTrigger(stirTrigger);
        }

        GameEvents.RaiseStirred();
        GameEvents.ShowStatus(
            "Ингредиенты перемешаны"
        );
    }

    public void ApplySalt()
    {
        if (!inventory.TryConsume(saltIngredient))
        {
            GameEvents.ShowStatus(
                "В хранилище нет соли"
            );

            return;
        }

        seasoning = SeasoningState.Salted;

        inventory.SaltAllIngredients();

        GameEvents.RaiseSaltApplied();
        GameEvents.ShowStatus("Блюдо посолено");
    }

    public void StartFrying()
    {
        if (inventory.Ingredients.Count == 0)
        {
            GameEvents.ShowStatus(
                "Сковородка пустая"
            );

            return;
        }

        if (!stove.IsReadyForCooking)
        {
            GameEvents.ShowStatus(
                "Сковородка ещё не нагрелась"
            );

            return;
        }

        if (fryingCoroutine != null)
        {
            return;
        }

        if (!actions.Contains(CraftAction.Fry))
        {
            actions.Add(CraftAction.Fry);
        }

        cooking = CookingState.Cooking;

        GameEvents.RaiseFryingStarted();
        GameEvents.ShowStatus("Жарим...");

        fryingCoroutine =
            StartCoroutine(FryingRoutine());
    }

    private IEnumerator FryingRoutine()
    {
        yield return new WaitForSeconds(
            fryingDuration
        );

        cooking = CookingState.Ready;

        currentRecipe = RecipeResolver.Resolve(
            inventory.Ingredients,
            actions,
            recipes
        );

        if (currentRecipe != null)
        {
            GameEvents.ShowStatus(
                $"Готово: {currentRecipe.DisplayName}"
            );
        }
        else
        {
            GameEvents.ShowStatus(
                "Получилось неизвестное блюдо"
            );
        }

        yield return new WaitForSeconds(
            burnAfterReadySeconds
        );

        cooking = CookingState.Burned;
        fryingCoroutine = null;

        GameEvents.RaiseFryingStopped();
        GameEvents.RaiseDishBurned();

        GameEvents.ShowStatus(
            "Блюдо сгорело!"
        );
    }

    public void Serve()
    {
        if (inventory.Ingredients.Count == 0)
        {
            GameEvents.ShowStatus(
                "Нет блюда для подачи"
            );

            return;
        }

        if (cooking == CookingState.Cooking)
        {
            if (fryingCoroutine != null)
            {
                StopCoroutine(fryingCoroutine);
                fryingCoroutine = null;
            }

            cooking = CookingState.Raw;

            GameEvents.RaiseFryingStopped();
        }
        else if (fryingCoroutine != null)
        {
            StopCoroutine(fryingCoroutine);
            fryingCoroutine = null;

            GameEvents.RaiseFryingStopped();
        }

        if (currentRecipe == null)
        {
            currentRecipe = RecipeResolver.Resolve(
                inventory.Ingredients,
                actions,
                recipes
            );
        }

        bool hasSpoiled =
            inventory.HasSpoiledIngredients();

        DishResult result = new(
            currentRecipe,
            cooking,
            seasoning,
            hasSpoiled
        );


        if (!result.IsSuccessful)
        {
            GameEvents.RaiseCraftFailed();
        }

        GameEvents.RaiseDishServed(result);

        ResetStation();
    }

    private void ResetStation()
    {
        inventory.Clear();
        actions.Clear();
        seasoning = SeasoningState.Unsalted;
        cooking = CookingState.Raw;
        currentRecipe = null;

        if (stove != null && stove.Pan != null)
            stove.Pan.ResetAfterCooking();
    }
}