using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftStationController : MonoBehaviour
{
    public static CraftStationController Instance { get; private set; }

    [SerializeField]
    private CraftStationInventory inventory;

    [SerializeField]
    private StoveController stove;

    [SerializeField]
    private IngredientData saltIngredient;

    [SerializeField]
    private List<RecipeData> recipes = new();

    [Header("Cooking")]
    [SerializeField]
    private float fryingDuration = 4f;

    [SerializeField]
    private float burnAfterReadySeconds = 4f;

    [Header("Animation")]
    [SerializeField]
    private Animator stirAnimator;

    [SerializeField]
    private string stirTrigger = "Stir";

    private readonly List<CraftAction> actions = new();

    private SeasoningState seasoning =
        SeasoningState.Unsalted;

    private CookingState cooking =
        CookingState.Raw;

    private RecipeData currentRecipe;
    private Coroutine fryingCoroutine;

    private bool ingredientsLocked;

    public CookingState Cooking => cooking;
    public bool IngredientsLocked => ingredientsLocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "В сцене найдено несколько CraftStationController."
            );

            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool CanCollectIngredient(IngredientData ingredient)
    {
        if (ingredient == null)
        {
            return false;
        }

        if (!ingredientsLocked)
        {
            return true;
        }

        // Соль разрешаем взять даже после загрузки
        // основных ингредиентов в сковороду.
        return ingredient == saltIngredient;
    }

    public void LockIngredients()
    {
        ingredientsLocked = true;
    }

    public void Stir()
    {
        if (inventory == null ||
            inventory.Ingredients.Count == 0)
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
        if (inventory == null ||
            !inventory.TryConsume(saltIngredient))
        {
            GameEvents.ShowStatus(
                "В хранилище нет соли"
            );

            return;
        }

        seasoning = SeasoningState.Salted;

        inventory.SaltAllIngredients();

        GameEvents.RaiseSaltApplied();

        GameEvents.ShowStatus(
            "Блюдо посолено"
        );
    }

    public void StartFrying()
    {
        if (inventory == null ||
            inventory.Ingredients.Count == 0)
        {
            GameEvents.ShowStatus(
                "Сковородка пустая"
            );

            return;
        }

        if (stove == null ||
            !stove.IsReadyForCooking)
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

        LockIngredients();

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
        if (inventory == null ||
            inventory.Ingredients.Count == 0)
        {
            GameEvents.ShowStatus(
                "Нет блюда для подачи"
            );

            return;
        }

        if (fryingCoroutine != null)
        {
            StopCoroutine(fryingCoroutine);
            fryingCoroutine = null;

            GameEvents.RaiseFryingStopped();

            if (cooking == CookingState.Cooking)
            {
                // Подали слишком рано.
                cooking = CookingState.Raw;
            }
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

    public void CancelFrying()
    {
        if (fryingCoroutine == null)
        {
            return;
        }

        StopCoroutine(fryingCoroutine);
        fryingCoroutine = null;

        // Если сняли блюдо до готовности,
        // оно снова считается сырым.
        if (cooking == CookingState.Cooking)
        {
            cooking = CookingState.Raw;
        }

        // Если оно уже Ready, мы просто сняли его с огня
        // и остановили дальнейшее подгорание.

        GameEvents.RaiseFryingStopped();
    }

    public void RestartCooking()
    {
        if (fryingCoroutine != null)
        {
            StopCoroutine(fryingCoroutine);
            fryingCoroutine = null;

            GameEvents.RaiseFryingStopped();
        }

        ResetStation();

        GameEvents.ShowStatus(
            "Готовка сброшена"
        );
    }

    private void ResetStation()
    {
        if (inventory != null)
        {
            inventory.Clear();
        }

        actions.Clear();

        seasoning = SeasoningState.Unsalted;
        cooking = CookingState.Raw;
        currentRecipe = null;
        ingredientsLocked = false;

        if (stove != null &&
            stove.Pan != null)
        {
            stove.Pan.ResetAfterCooking();
        }
    }
}