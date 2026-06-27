using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PauseMenu : MonoBehaviour
{
    // Version: 1.1
    [Header("Références UI")]
    [Tooltip("Canvas du menu pause (désactivé par défaut)")]
    public GameObject pauseUI;

    [Tooltip("Bouton sélectionné par défaut quand le menu s'ouvre")]
    public GameObject firstSelected;

    [Tooltip("Panneau de settings (pour le fermer quand on reprend le jeu)")]
    public GameObject settingsPanel;

    [Tooltip("Panneau de contrôles")]
    public GameObject panelControls;

    [Header("Gameplay")]
    [Tooltip("Composants à désactiver pendant la pause (ex: player controller)")]
    public MonoBehaviour[] disableWhilePaused;

    [Header("Touche (ancien Input)")]
    public KeyCode toggleKey = KeyCode.Escape;

#if ENABLE_INPUT_SYSTEM
    [Header("Nouvelle Input (optionnel)")]
    [Tooltip("Action d’entrée pour la pause (bind: <Keyboard>/escape, etc.)")]
    public InputActionReference pauseAction;
#endif

    private bool isPaused;

    void OnEnable()
    {
#if ENABLE_INPUT_SYSTEM
        if (pauseAction?.action != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePerformed;
        }
#endif
    }

    void OnDisable()
    {
#if ENABLE_INPUT_SYSTEM
        if (pauseAction?.action != null)
            pauseAction.action.performed -= OnPausePerformed;
#endif
    }

#if ENABLE_INPUT_SYSTEM
    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }
#endif

    void Start()
    {
        // Démarrage propre : jeu non en pause et UI cachée
        ForceUnpause();
        if (pauseUI) pauseUI.SetActive(false);
    }

    void Update()
    {
        // Ancien Input System : touche d’ouverture/fermeture du menu pause
        if (Input.GetKeyDown(toggleKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pauseUI) pauseUI.SetActive(true);

        // Curseur libre pour naviguer dans le menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Coupe les scripts de gameplay pendant la pause
        foreach (var c in disableWhilePaused)
            if (c) c.enabled = false;

        // Focus initial sur un bouton du menu
        if (firstSelected)
            EventSystem.current?.SetSelectedGameObject(firstSelected);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseUI) pauseUI.SetActive(false);

        // Ferme les panneaux s'ils sont ouverts
        if (settingsPanel) settingsPanel.SetActive(false);
        if (panelControls) panelControls.SetActive(false);

        // Reprend le contrôle FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Réactive les scripts de gameplay
        foreach (var c in disableWhilePaused)
            if (c) c.enabled = true;

        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void QuitToMenu()
    {
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        // Avant de changer de scène, on sort de la pause et on remet le temps normal
        Time.timeScale = 1f;
        isPaused = false;

        // Réinitialise le timer pour la prochaine partie
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.ResetTimer();
            GameTimer.Instance.StopTimer();
        }

        // Petit fondu si un ScreenFader global existe
        if (ScreenFader.Instance)
            yield return ScreenFader.Instance.FadeOut(0.3f);

        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        // Sécurité : ne jamais laisser le timeScale à 0 si l’objet est détruit
        if (isPaused) Time.timeScale = 1f;
    }

    public void ForceUnpause()
    {
        // Utilitaire pour remettre l'état par défaut (utile à l'ouverture de scène)
        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnClickSettings()
    {
        // Affiche le panneau de settings
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    public void OnClickCloseSettings()
    {
        // Ferme le panneau de settings
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void OnClickControls()
    {
        // Affiche le panneau de contrôles
        if (panelControls) panelControls.SetActive(true);
    }

    public void OnClickCloseControls()
    {
        // Ferme le panneau de contrôles
        if (panelControls) panelControls.SetActive(false);
    }
}
