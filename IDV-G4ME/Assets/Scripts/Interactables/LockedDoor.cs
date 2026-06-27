using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

/// <summary>
/// Porte de sortie verrouillée jusqu'à la défaite du boss.
/// Affiche un écran de victoire quand le joueur l'ouvre après avoir vaincu le boss.
/// </summary>
public class LockedDoor : MonoBehaviour
{
    [Header("UI - Messages")]
    [Tooltip("UI du prompt d'interaction (ex: 'Appuyez sur E')")]
    public GameObject interactPromptUI;
    
    [Tooltip("Panel UI pour le message de porte verrouillée")]
    public GameObject lockedMessagePanel;
    
    [Tooltip("Texte du message de porte verrouillée")]
    public TextMeshProUGUI lockedMessageText;
    
    [Tooltip("Message affiché quand la porte est verrouillée")]
    public string lockedMessage = "The door is closed";
    
    [Tooltip("Durée d'affichage du message verrouillé (secondes)")]
    public float lockedMessageDuration = 2f;
    
    [Header("UI - Écran de victoire")]
    [Tooltip("Panel noir plein écran pour la victoire")]
    public GameObject victoryBlackScreen;
    
    [Tooltip("Texte de victoire")]
    public TextMeshProUGUI victoryText;
    
    [Tooltip("Message de victoire")]
    [TextArea(3, 5)]
    public string victoryMessage = "Félicitations !\nVous êtes sorti de la tombe de Kothar";
    
    [Tooltip("Durée du fondu vers le noir (secondes)")]
    public float fadeInDuration = 1f;
    
    [Tooltip("Durée d'affichage de l'écran de victoire (secondes)")]
    public float victoryDisplayDuration = 4f;
    
    [Header("Transition")]
    [Tooltip("Nom de la scène du menu principal à charger après la victoire")]
    public string mainMenuSceneName = "MainMenu";
    
    [Header("Timer UI")]
    [Tooltip("GameObject du timer à cacher pendant la victoire")]
    public GameObject timerUI;
    
    [Header("Audio (optionnel)")]
    [Tooltip("Son joué quand on essaie d'ouvrir la porte verrouillée")]
    public AudioClip lockedSound;
    
    [Tooltip("Musique de victoire (remplace la musique d'ambiance)")]
    public AudioClip victoryMusic;
    
    // État interne
    private bool isUnlocked = false;
    private bool playerInside = false;
    private bool victoryTriggered = false;
    private AudioSource audioSource;
    
    void Awake()
    {
        // Récupère ou crée un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (lockedSound != null || victoryMusic != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0.5f; // Semi-3D
        }
        
        // S'assure que les panels UI sont désactivés au départ
        if (lockedMessagePanel) lockedMessagePanel.SetActive(false);
        if (victoryBlackScreen) victoryBlackScreen.SetActive(false);
    }
    
    void OnEnable()
    {
        // S'abonne à l'événement de défaite du boss
        BossEnemy.OnBossDefeated += UnlockDoor;
    }
    
    void OnDisable()
    {
        // Se désabonne de l'événement
        BossEnemy.OnBossDefeated -= UnlockDoor;
    }
    
    void Update()
    {
        // Si le joueur est dans la zone et appuie sur E
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }
    
    /// <summary>
    /// Appelé par le trigger enfant quand le joueur entre dans la zone
    /// </summary>
    public void SetPlayerInside(bool inside)
    {
        playerInside = inside;
        
        // Affiche/cache le prompt d'interaction
        if (interactPromptUI)
        {
            interactPromptUI.SetActive(inside && !victoryTriggered);
        }
    }
    
    /// <summary>
    /// Déverrouille la porte (appelé automatiquement quand le boss meurt)
    /// </summary>
    void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("[LockedDoor] Boss defeated! Door is now unlocked.");
    }
    
    /// <summary>
    /// Tente d'ouvrir la porte
    /// </summary>
    void TryOpenDoor()
    {
        if (victoryTriggered) return; // Évite les déclenchements multiples
        
        if (!isUnlocked)
        {
            // Porte verrouillée : affiche le message
            StartCoroutine(ShowLockedMessage());
        }
        else
        {
            // Porte déverrouillée : déclenche la victoire !
            StartCoroutine(ShowVictorySequence());
        }
    }
    
    /// <summary>
    /// Affiche temporairement le message "The door is closed"
    /// </summary>
    IEnumerator ShowLockedMessage()
    {
        // Joue le son de porte verrouillée
        if (lockedSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lockedSound);
        }
        
        // Configure et affiche le message
        if (lockedMessagePanel && lockedMessageText)
        {
            lockedMessageText.text = lockedMessage;
            lockedMessagePanel.SetActive(true);
            
            yield return new WaitForSeconds(lockedMessageDuration);
            
            lockedMessagePanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Affiche la séquence de victoire complète
    /// </summary>
    IEnumerator ShowVictorySequence()
    {
        victoryTriggered = true;
        
        // Cache le prompt d'interaction
        if (interactPromptUI) interactPromptUI.SetActive(false);
        
        // Arrête le timer
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StopTimer();
            float time = GameTimer.Instance.Elapsed;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            Debug.Log($"[LockedDoor] Timer stopped at: {minutes:00}:{seconds:00}");
        }
        
        // Cache le timer si présent
        if (timerUI) timerUI.SetActive(false);
        
        // Met le jeu en pause
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Prépare l'écran de victoire
        if (victoryBlackScreen && victoryText)
        {
            victoryText.text = victoryMessage;
            
            // Commence avec un écran noir transparent
            CanvasGroup canvasGroup = victoryBlackScreen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = victoryBlackScreen.AddComponent<CanvasGroup>();
            
            canvasGroup.alpha = 0f;
            victoryBlackScreen.SetActive(true);
            
            // Fondu vers le noir
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }
        
        // Joue la musique de victoire si présente
        if (victoryMusic != null)
        {
            MusicManager musicManager = FindObjectOfType<MusicManager>();
            if (musicManager != null)
            {
                // Utilise le MusicManager existant pour jouer la musique de victoire
                musicManager.PlayVictoryMusic(victoryMusic);
            }
            else if (audioSource != null)
            {
                // Fallback : joue directement avec l'AudioSource local
                audioSource.clip = victoryMusic;
                audioSource.loop = false;
                audioSource.Play();
            }
        }
        
        // Affiche le message de victoire pendant la durée spécifiée
        yield return new WaitForSecondsRealtime(victoryDisplayDuration);
        
        // Restaure l'état normal du jeu avant de changer de scène
        Time.timeScale = 1f;
        
        Debug.Log("[LockedDoor] Victory sequence complete! Returning to main menu...");
        
        // Charge le menu principal
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
