using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private TMP_Text statusText;

    private void OnEnable()
    {
        GameEvents.StatusMessage += ShowMessage;
    }

    private void OnDisable()
    {
        GameEvents.StatusMessage -= ShowMessage;
    }

    private void ShowMessage(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}