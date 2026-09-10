using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftStationInventory : MonoBehaviour
{
    public static CraftStationInventory Instance { get; private set; }

    [SerializeField]
    private List<IngredientRuntime> ingredients = new();

    public IReadOnlyList<IngredientRuntime> Ingredients => ingredients;

    public event Action Changed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
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

    public void AddIngredient(
        IngredientData data,
        FreshnessState freshness,
        SeasoningState seasoning)
    {
        ingredients.Add(
            new IngredientRuntime(data, freshness, seasoning)
        );

        Debug.Log(
            $"Добавлен ингредиент: {data.DisplayName}"
        );

        Changed?.Invoke();
    }

    public bool TryConsume(IngredientData data)
    {
        for (int i = 0; i < ingredients.Count; i++)
        {
            if (ingredients[i].Data != data)
            {
                continue;
            }

            ingredients.RemoveAt(i);
            Changed?.Invoke();

            return true;
        }

        return false;
    }

    public bool HasSpoiledIngredients()
    {
        foreach (IngredientRuntime ingredient in ingredients)
        {
            if (ingredient.Freshness == FreshnessState.Spoiled)
            {
                return true;
            }
        }

        return false;
    }

    public void SaltAllIngredients()
    {
        foreach (IngredientRuntime ingredient in ingredients)
        {
            ingredient.SetSeasoning(SeasoningState.Salted);
        }

        Changed?.Invoke();
    }

    public void Clear()
    {
        ingredients.Clear();
        Changed?.Invoke();
    }
}