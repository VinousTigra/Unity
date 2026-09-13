using UnityEngine;
using UnityEngine.InputSystem;

public class GameModeController : MonoBehaviour
{
    public static bool IsUIOpen { get; private set; }

    private void Start()
    {
        SetUIOpen(false);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SetUIOpen(!IsUIOpen);
        }
    }

    public static void SetUIOpen(bool open)
    {
        IsUIOpen = open;

        if (open)
        {
            Cursor.lockState =
                CursorLockMode.None;

            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState =
                CursorLockMode.Locked;

            Cursor.visible = false;
        }
    }
}