using System.Collections;
using UnityEngine;

public class StoveController : InteractableBase
{
    [SerializeField] private PanController pan;
    public PanController Pan => pan;
    [SerializeField] private Transform panAnchor;

    [SerializeField]
    private float heatingDuration = 4f;

    private StoveState state = StoveState.Off;
    private bool panOnStove;

    private Coroutine heatingCoroutine;

    public StoveState State => state;

    public bool IsReadyForCooking =>
        panOnStove &&
        state == StoveState.Ready;

    public override void Interact()
    {
        Debug.Log(
            "STOVE INTERACT | PanOnStove = " +
            panOnStove +
            " | Pan = " +
            (pan != null ? pan.name : "NULL") +
            " | PanState = " +
            (pan != null ? pan.State.ToString() : "NULL") +
            " | StoveState = " +
            state
        );

        if (pan != null && pan.IsHeld)
        {
            Debug.Log("STOVE: Сковородка в руках — ставим на плиту.");

            pan.PlaceOnStove(panAnchor);
            panOnStove = true;

            Debug.Log("STOVE: Сковородка успешно поставлена.");

            return;
        }

        if (!panOnStove)
        {
            Debug.Log("STOVE: НЕТ СКОВОРОДКИ НА ПЛИТЕ.");

            GameEvents.ShowStatus(
                "Сначала поставьте сковородку"
            );

            return;
        }

        if (state == StoveState.Off)
        {
            Debug.Log("STOVE: Начинаем нагрев.");

            StartHeating();
        }
        else
        {
            Debug.Log("STOVE: Выключаем плиту.");

            TurnOff();
        }
    }

    private void StartHeating()
    {
        state = StoveState.Heating;

        Debug.Log(
            "STOVE: HEATING STARTED | Duration = " +
            heatingDuration
        );

        GameEvents.RaiseStoveStateChanged(state);

        GameEvents.ShowStatus(
            "Сковородка нагревается..."
        );

        heatingCoroutine =
            StartCoroutine(HeatingRoutine());
    }

    private IEnumerator HeatingRoutine()
    {
        Debug.Log("STOVE: Ждём " + heatingDuration + " секунд.");

        yield return new WaitForSeconds(
            heatingDuration
        );

        heatingCoroutine = null;

        Debug.Log(
            "STOVE: Таймер закончился | PanOnStove = " +
            panOnStove
        );

        if (!panOnStove)
        {
            TurnOff();
            yield break;
        }

        state = StoveState.Ready;

        Debug.Log("STOVE: СКОВОРОДКА ГОТОВА.");

        GameEvents.RaiseStoveStateChanged(state);
        GameEvents.RaiseStoveReady();

        GameEvents.ShowStatus(
            "Сковородка нагрелась"
        );
    }

    private void TurnOff()
    {
        if (heatingCoroutine != null)
        {
            StopCoroutine(heatingCoroutine);
            heatingCoroutine = null;
        }

        state = StoveState.Off;

        Debug.Log("STOVE: ПЛИТА ВЫКЛЮЧЕНА.");

        GameEvents.RaiseStoveStateChanged(state);
        GameEvents.ShowStatus("Плита выключена");
    }

    public void NotifyPanRemoved()
    {
        Debug.Log("STOVE: Сковородка снята.");

        panOnStove = false;
        TurnOff();
    }

    public void NotifyPanPlacedXR()
    {
        Debug.Log("STOVE: Сковородка поставлена через XR.");

        panOnStove = true;

        if (pan != null)
        {
            pan.NotifyPlacedOnStoveXR();
        }
    }

    public void NotifyPanRemovedXR()
    {
        Debug.Log("STOVE: Сковородка снята через XR.");

        NotifyPanRemoved();
    }
}