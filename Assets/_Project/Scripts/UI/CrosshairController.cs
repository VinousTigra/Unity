using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private GameObject crosshairVisual;

    private void Update()
    {
        if (crosshairVisual != null)
            crosshairVisual.SetActive(!GameModeController.IsUIOpen);
    }
}