using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayer;

    private InputAction interactAction;

    private void Awake()
    {
        PlayerInput playerInput =
            GetComponent<PlayerInput>();

        interactAction =
            playerInput.actions.FindAction(
                "Interact",
                true
            );
    }

    private void Update()
    {
        if (!interactAction.WasPressedThisFrame())
        {
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        TryInteract();
    }

    private void TryInteract()
    {
        Ray ray = new(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                interactionDistance,
                interactionLayer))
        {
            Debug.Log("INTERACT: ни во что не попал");
            return;
        }

        Debug.Log(
            "INTERACT HIT: " +
            hit.collider.name +
            " | object: " +
            hit.collider.gameObject.name
        );

        InteractableBase interactable =
            hit.collider.GetComponentInParent<InteractableBase>();

        if (interactable != null)
        {
            Debug.Log(
                "INTERACTABLE FOUND: " +
                interactable.GetType().Name
            );

            interactable.Interact();
        }
        else
        {
            Debug.Log(
                "INTERACTABLE NOT FOUND on " +
                hit.collider.name
            );
        }
    }
}