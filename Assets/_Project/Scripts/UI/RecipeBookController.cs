using UnityEngine;
using UnityEngine.InputSystem;

public class RecipeBookController : MonoBehaviour
{
    [SerializeField]
    private GameObject recipeBookPanel;

    private void Start()
    {
        if (recipeBookPanel != null)
        {
            recipeBookPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (recipeBookPanel == null)
        {
            return;
        }

        if (recipeBookPanel.activeSelf &&
            !GameModeController.IsUIOpen)
        {
            recipeBookPanel.SetActive(false);
        }

        if (Keyboard.current != null &&
            Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleBook();
        }
    }

    private void ToggleBook()
    {
        bool shouldOpen =
            !recipeBookPanel.activeSelf;

        recipeBookPanel.SetActive(
            shouldOpen
        );

        GameModeController.SetUIOpen(
            shouldOpen
        );
    }

    public void CloseBook()
    {
        if (recipeBookPanel != null)
        {
            recipeBookPanel.SetActive(false);
        }

        GameModeController.SetUIOpen(false);
    }
}