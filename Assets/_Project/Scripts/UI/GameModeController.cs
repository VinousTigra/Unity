using UnityEngine;
using UnityEngine.InputSystem;

public class GameModeController : MonoBehaviour
{
    public static bool IsUIOpen { get; private set; }

    private void Start()
    {
        SetFPSMode();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (IsUIOpen)
                SetFPSMode();
            else
                SetUIMode();
        }
    }

    private void SetFPSMode()
    {
        IsUIOpen = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetUIMode()
    {
        IsUIOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}