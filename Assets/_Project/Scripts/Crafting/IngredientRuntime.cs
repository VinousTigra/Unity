using System;
using UnityEngine;

[Serializable]
public class IngredientRuntime
{
    [SerializeField] private IngredientData data;
    [SerializeField] private FreshnessState freshness;
    [SerializeField] private SeasoningState seasoning;

    public IngredientData Data => data;
    public FreshnessState Freshness => freshness;
    public SeasoningState Seasoning => seasoning;

    public IngredientRuntime(
        IngredientData data,
        FreshnessState freshness,
        SeasoningState seasoning)
    {
        this.data = data;
        this.freshness = freshness;
        this.seasoning = seasoning;
    }

    public void SetFreshness(FreshnessState newState)
    {
        freshness = newState;
    }

    public void SetSeasoning(SeasoningState newState)
    {
        seasoning = newState;
    }
}