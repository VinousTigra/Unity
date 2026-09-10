using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Recipe",
    menuName = "Game/Recipe"
)]
public class RecipeData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;

    [SerializeField]
    private List<IngredientData> requiredIngredients = new();

    [SerializeField]
    private List<CraftAction> requiredActions = new();

    [SerializeField]
    private bool saltedVariant;

    [SerializeField]
    private GameObject resultPrefab;

    public string Id => id;
    public string DisplayName => displayName;

    public IReadOnlyList<IngredientData> RequiredIngredients
        => requiredIngredients;

    public IReadOnlyList<CraftAction> RequiredActions
        => requiredActions;

    public bool SaltedVariant => saltedVariant;

    public GameObject ResultPrefab => resultPrefab;
}