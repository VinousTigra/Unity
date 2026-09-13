using UnityEngine;

public class PanController : InteractableBase
{
    [SerializeField] private Rigidbody body;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private StoveController stove;
    [SerializeField] private CraftStationController craftStation;

    private Transform initialParent;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private PanState state = PanState.OnTable;
    private bool ingredientsLoaded;

    public PanState State => state;
    public bool IsHeld => state == PanState.Held;
    public bool IsOnStove => state == PanState.OnStove;
    public bool IngredientsLoaded => ingredientsLoaded;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody>();
        }

        initialParent = transform.parent;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public override void Interact()
    {
        if (state == PanState.OnStove)
        {
            if (!ingredientsLoaded &&
                CraftStationInventory.Instance != null &&
                CraftStationInventory.Instance.Ingredients.Count > 0)
            {
                LoadIngredients();
                return;
            }

            // Если ингредиенты уже загружены,
            // взаимодействие со сковородкой снимает её с плиты.
            // Жарка запускается отдельной UI-кнопкой.
            PickUp();
            return;
        }

        if (state == PanState.Held)
        {
            ReturnToStart();
            return;
        }

        PickUp();
    }

    private void LoadIngredients()
    {
        ingredientsLoaded = true;

        if (craftStation != null)
        {
            craftStation.LockIngredients();
        }

        GameEvents.ShowStatus(
            "Ингредиенты добавлены в сковороду"
        );
    }

    private void PickUp()
    {
        if (state == PanState.OnStove &&
            stove != null)
        {
            stove.NotifyPanRemoved();
        }

        if (craftStation != null)
        {
            craftStation.CancelFrying();
        }

        StopAllPanParticles();

        state = PanState.Held;

        if (body != null)
        {
            body.isKinematic = true;
        }

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        GameEvents.ShowStatus("Сковородка взята");
    }

    public void PlaceOnStove(Transform anchor)
    {
        state = PanState.OnStove;

        if (body != null)
        {
            body.isKinematic = true;
        }

        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        GameEvents.ShowStatus(
            "Сковородка поставлена на плиту"
        );
    }

    private void ReturnToStart()
    {
        state = PanState.OnTable;

        transform.SetParent(initialParent);
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (body != null)
        {
            body.isKinematic = true;
        }

        GameEvents.ShowStatus(
            "Сковородка возвращена"
        );
    }

    public void ResetAfterCooking()
    {
        ingredientsLoaded = false;

        if (state == PanState.OnStove &&
            stove != null)
        {
            stove.NotifyPanRemoved();
        }

        StopAllPanParticles();

        ReturnToStart();
    }

    private void StopAllPanParticles()
    {
        ParticleSystem[] particles =
            GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particle in particles)
        {
            particle.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            particle.Clear(true);
        }
    }

    // Для будущего XR.

    public void NotifyXRGrabbed()
    {
        if (state == PanState.OnStove &&
            stove != null)
        {
            stove.NotifyPanRemoved();
        }

        if (craftStation != null)
        {
            craftStation.CancelFrying();
        }

        StopAllPanParticles();

        state = PanState.Held;
    }

    public void NotifyXRReleased()
    {
        if (state == PanState.Held)
        {
            state = PanState.OnTable;
        }
    }

    public void NotifyPlacedOnStoveXR()
    {
        state = PanState.OnStove;
    }
}