using System.Collections;
using UnityEngine;

public class WorldIngredient : InteractableBase
{
    [SerializeField] private IngredientData data;

    [Header("Runtime State")]
    [SerializeField]
    private FreshnessState freshness = FreshnessState.Fresh;

    [SerializeField]
    private SeasoningState seasoning = SeasoningState.Unsalted;

    [Header("Respawn")]
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private Collider interactionCollider;
    [SerializeField] private float respawnDelay = 5f;

    private bool isAvailable = true;

    public IngredientData Data => data;
    public FreshnessState Freshness => freshness;
    public bool IsAvailable => isAvailable;

    private void Awake()
    {
        if (interactionCollider == null)
        {
            interactionCollider = GetComponent<Collider>();
        }

        if (visualRoot == null)
        {
            Transform visual = transform.Find("Visual");

            if (visual != null)
            {
                visualRoot = visual.gameObject;
            }
        }
    }

    public override void Interact()
    {
        if (!isAvailable)
        {
            return;
        }

        CraftStationInventory inventory =
            CraftStationInventory.Instance;

        if (inventory == null)
        {
            Debug.LogError(
                "CraftStationInventory отсутствует в сцене."
            );

            return;
        }

        inventory.AddIngredient(
            data,
            freshness,
            seasoning
        );

        GameEvents.ShowStatus(
            $"Взято: {data.DisplayName}"
        );

        StartCoroutine(RespawnRoutine());
    }

    public void MarkSpoiled()
    {
        freshness = FreshnessState.Spoiled;
    }

    private IEnumerator RespawnRoutine()
    {
        isAvailable = false;

        if (visualRoot != null)
        {
            visualRoot.SetActive(false);
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = false;
        }

        yield return new WaitForSeconds(respawnDelay);

        if (visualRoot != null)
        {
            visualRoot.SetActive(true);
        }

        if (interactionCollider != null)
        {
            interactionCollider.enabled = true;
        }

        isAvailable = true;
    }
}