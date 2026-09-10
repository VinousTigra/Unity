using System.Collections.Generic;

public static class RecipeResolver
{
    public static RecipeData Resolve(
        IReadOnlyList<IngredientRuntime> ingredients,
        IReadOnlyList<CraftAction> actions,
        IReadOnlyList<RecipeData> recipes)
    {
        foreach (RecipeData recipe in recipes)
        {
            if (Matches(recipe, ingredients, actions))
            {
                return recipe;
            }
        }

        return null;
    }

    private static bool Matches(
        RecipeData recipe,
        IReadOnlyList<IngredientRuntime> ingredients,
        IReadOnlyList<CraftAction> actions)
    {
        if (recipe.RequiredIngredients.Count != ingredients.Count)
        {
            return false;
        }

        Dictionary<IngredientData, int> requiredCounts = new();

        foreach (IngredientData ingredient in recipe.RequiredIngredients)
        {
            if (!requiredCounts.TryAdd(ingredient, 1))
            {
                requiredCounts[ingredient]++;
            }
        }

        Dictionary<IngredientData, int> actualCounts = new();

        foreach (IngredientRuntime ingredient in ingredients)
        {
            IngredientData data = ingredient.Data;

            if (!actualCounts.TryAdd(data, 1))
            {
                actualCounts[data]++;
            }
        }

        foreach (KeyValuePair<IngredientData, int> required in requiredCounts)
        {
            if (!actualCounts.TryGetValue(
                    required.Key,
                    out int actualCount))
            {
                return false;
            }

            if (actualCount != required.Value)
            {
                return false;
            }
        }

        foreach (CraftAction requiredAction in recipe.RequiredActions)
        {
            if (!ContainsAction(actions, requiredAction))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ContainsAction(
        IReadOnlyList<CraftAction> actions,
        CraftAction target)
    {
        foreach (CraftAction action in actions)
        {
            if (action == target)
            {
                return true;
            }
        }

        return false;
    }
}