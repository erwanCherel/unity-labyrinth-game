using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugUI : MonoBehaviour
{
    [Header("Debug UI Elements")]
    public Button debugToggleButton;
    public TextMeshProUGUI debugStatusText;
    public GameObject debugPanel;

    [Header("Debug Info Display")]
    public TextMeshProUGUI debugInfoText;
    private bool showDebugInfo = false;

    void Start()
    {
        if (debugToggleButton != null)
            debugToggleButton.onClick.AddListener(OnDebugToggleClicked);

        UpdateDebugUI();
    }

    void Update()
    {
        // Affiche/masque le panneau debug avec Tab
        if (Input.GetKeyDown(KeyCode.Tab) && DebugManager.IsDebugEnabled)
        {
            showDebugInfo = !showDebugInfo;
            if (debugPanel != null)
                debugPanel.SetActive(showDebugInfo);
        }

        if (showDebugInfo && debugInfoText != null)
            UpdateDebugInfo();
    }

    public void OnDebugToggleClicked()
    {
        DebugManager.ToggleDebug();
        UpdateDebugUI();
    }

    private void UpdateDebugUI()
    {
        if (debugStatusText != null)
        {
            debugStatusText.text = DebugManager.IsDebugEnabled ? "DEBUG: ON" : "DEBUG: OFF";
            debugStatusText.color = DebugManager.IsDebugEnabled ? Color.green : Color.red;
        }

        if (debugPanel != null)
            debugPanel.SetActive(false);
    }

    private void UpdateDebugInfo()
    {
        if (debugInfoText == null) return;

        string info = "=== DEBUG INFO ===\n";
        info += $"FPS: {Mathf.Round(1f / Time.deltaTime)}\n";
        info += $"Time.timeScale: {Time.timeScale}\n";
        info += $"Time.deltaTime: {Time.deltaTime:F4}s\n";
        info += $"Scenes loaded: {UnityEngine.SceneManagement.SceneManager.sceneCount}\n";
        info += $"\n[Alt+D] Toggle Debug\n[Tab] Show/Hide Info";

        debugInfoText.text = info;
    }
}
