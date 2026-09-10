using UnityEngine;

public class DishPresenter : MonoBehaviour
{
    [SerializeField]
    private Transform servingPoint;

    [SerializeField]
    private GameObject defaultDishPrefab;

    [SerializeField]
    private GameObject failedDishPrefab;

    private GameObject currentDish;

    private void OnEnable()
    {
        GameEvents.DishServed += OnDishServed;
    }

    private void OnDisable()
    {
        GameEvents.DishServed -= OnDishServed;
    }

    private void OnDishServed(DishResult result)
    {
        if (currentDish != null)
        {
            Destroy(currentDish);
        }

        GameObject prefab = ChoosePrefab(result);

        if (prefab == null)
        {
            return;
        }

        currentDish = Instantiate(
            prefab,
            servingPoint.position,
            servingPoint.rotation
        );
    }

    private GameObject ChoosePrefab(DishResult result)
    {
        if (!result.IsSuccessful)
        {
            return failedDishPrefab != null
                ? failedDishPrefab
                : defaultDishPrefab;
        }

        if (result.Recipe != null &&
            result.Recipe.ResultPrefab != null)
        {
            return result.Recipe.ResultPrefab;
        }

        return defaultDishPrefab;
    }
}