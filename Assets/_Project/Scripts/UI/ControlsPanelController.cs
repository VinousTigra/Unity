using UnityEngine;

public class ControlsPanelController : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;

    public void CloseControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }
}