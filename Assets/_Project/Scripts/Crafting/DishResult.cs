using System;

[Serializable]
public class DishResult
{
    public RecipeData Recipe { get; }
    public CookingState Cooking { get; }
    public SeasoningState Seasoning { get; }
    public bool HasSpoiledIngredients { get; }
    public bool IsSuccessful { get; }

    public bool IsSalted =>
        Seasoning == SeasoningState.Salted;

    public DishResult(
        RecipeData recipe,
        CookingState cooking,
        SeasoningState seasoning,
        bool hasSpoiledIngredients)
    {
        Recipe = recipe;
        Cooking = cooking;
        Seasoning = seasoning;
        HasSpoiledIngredients = hasSpoiledIngredients;

        IsSuccessful =
            recipe != null &&
            cooking == CookingState.Ready &&
            !hasSpoiledIngredients;
    }
}