using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField]
    private CraftStationInventory inventory;

    [SerializeField]
    private CraftStationController craftStation;

    [SerializeField]
    private TMP_Text inventoryText;

    [SerializeField]
    private TMP_Text statusText;

    [SerializeField]
    private TMP_Text resultText;

    private void OnEnable()
    {
        if (inventory != null)
        {
            inventory.Changed += RefreshInventory;
        }

        GameEvents.StatusMessage += ShowStatus;
        GameEvents.DishServed += ShowResult;

        RefreshInventory();
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.Changed -= RefreshInventory;
        }

        GameEvents.StatusMessage -= ShowStatus;
        GameEvents.DishServed -= ShowResult;
    }

    private void RefreshInventory()
    {
        if (inventoryText == null ||
            inventory == null)
        {
            return;
        }

        Dictionary<string, int> counts = new();

        foreach (IngredientRuntime ingredient
                 in inventory.Ingredients)
        {
            string name =
                ingredient.Data.DisplayName;

            if (!counts.TryAdd(name, 1))
            {
                counts[name]++;
            }
        }

        StringBuilder builder = new();

        foreach (KeyValuePair<string, int> item
                 in counts)
        {
            builder.AppendLine(
                $"{item.Key}: {item.Value}"
            );
        }

        inventoryText.text =
            builder.Length == 0
                ? "Ингредиентов нет"
                : builder.ToString();
    }

    private void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void ShowResult(DishResult result)
    {
        if (resultText == null) return;

        if (result.Recipe == null)
        {
            resultText.text = "ОШИБКА: Неверный рецепт!";
            return;
        }


        string dishName = result.Recipe.SaltedVariant && result.IsSalted
        ? $"Солёный {result.Recipe.DisplayName}"
        : result.Recipe.DisplayName;


        resultText.text = result.IsSuccessful
            ? $"{dishName}: отлично!"
            : $"{dishName}: неудача";
    }

    // Эти методы подключаются к Button.OnClick.

    public void OnStirClicked()
    {
        Debug.Log("STIR BUTTON CLICKED");

        craftStation.Stir();
    }

    public void OnSaltClicked()
    {
        craftStation.ApplySalt();
    }

    public void OnFryClicked()
    {
        craftStation.StartFrying();
    }

    public void OnServeClicked()
    {
        craftStation.Serve();
    }
}