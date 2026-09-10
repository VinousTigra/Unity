using System.Text;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text inventoryText;

    private CraftStationInventory inventory;

    private void Start()
    {
        inventory = CraftStationInventory.Instance;

        if (inventory == null)
        {
            Debug.LogError(
                "InventoryUI: CraftStationInventory не найден."
            );

            return;
        }

        inventory.Changed += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.Changed -= Refresh;
        }
    }

    private void Refresh()
    {
        if (inventoryText == null)
        {
            return;
        }

        if (inventory.Ingredients.Count == 0)
        {
            inventoryText.text = "ѕусто";
            return;
        }

        StringBuilder text = new();

        foreach (IngredientRuntime ingredient
                 in inventory.Ingredients)
        {
            if (ingredient.Data != null)
            {
                text.AppendLine(
                    "Х " + ingredient.Data.DisplayName
                );
            }
        }

        inventoryText.text = text.ToString();
    }
}