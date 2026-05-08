using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        if (debugText == null)
        {
            Debug.LogError("DebugText reference is not set in DebugPanel.");
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void UpdateDebugText(string _fpsText, string _memoryText)
    {
        debugText.text = $"{_fpsText}\t{_memoryText}";
    }
}
