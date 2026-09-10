using UnityEngine;

[CreateAssetMenu(
    fileName = "Ingredient",
    menuName = "Game/Ingredient"
)]
public class IngredientData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;

    public string Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
}