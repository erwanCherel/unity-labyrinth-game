using UnityEngine;

public class PauseSettingsToggle : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Panneau principal du menu pause (avec les boutons Resume, Settings, etc.)")]
    public GameObject mainPausePanel;
    
    [Tooltip("Panneau des paramètres (avec les sliders de sensibilité)")]
    public GameObject settingsPanel;

    void Start()
    {
        // S'assure que le panneau settings est caché au démarrage
        if (settingsPanel) settingsPanel.SetActive(false);
        // Et que le panneau principal est visible
        if (mainPausePanel) mainPausePanel.SetActive(true);
    }

    /// <summary>
    /// Appelé par le bouton "Settings" dans le menu pause
    /// </summary>
    public void OpenSettings()
    {
        if (mainPausePanel) mainPausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Appelé par le bouton "Retour" dans le panneau settings
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        if (mainPausePanel) mainPausePanel.SetActive(true);
    }
}
