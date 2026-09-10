using UnityEngine;
using UnityEngine.InputSystem;

public class RecipeBookController : MonoBehaviour
{
    [SerializeField] private GameObject recipeBookPanel;

    private void Start()
    {
        if (recipeBookPanel != null)
            recipeBookPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            ToggleBook();
        }
    }

    private void ToggleBook()
    {
        if (recipeBookPanel == null)
            return;

        bool isOpen = recipeBookPanel.activeSelf;
        recipeBookPanel.SetActive(!isOpen);
    }
}