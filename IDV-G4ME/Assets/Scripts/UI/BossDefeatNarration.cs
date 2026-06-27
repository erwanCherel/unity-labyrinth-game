using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Affiche une séquence de textes narratifs après avoir vaincu le boss
/// </summary>
public class BossDefeatNarration : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Panneau noir plein écran")]
    public GameObject blackScreenUI;
    [Tooltip("Texte pour afficher la narration")]
    public TMP_Text narrationText;
    
    [Header("Narration")]
    [TextArea(3, 10)]
    [Tooltip("Lignes de texte à afficher une par une")]
    public string[] narrationLines = new string[]
    {
        "You have defeated the terrible Kothar...",
        "But will you be able to find your way back?",
        "I hope so, because otherwise...",
        "Kothar's Tomb... will become YOUR tomb!"
    };
    
    [Header("Timing")]
    [Tooltip("Durée d'affichage de chaque ligne (secondes)")]
    public float lineDuration = 3f;
    [Tooltip("Durée du fade noir au début (secondes)")]
    public float fadeInDuration = 1f;
    [Tooltip("Durée du fade noir à la fin (secondes)")]
    public float fadeOutDuration = 1f;
    [Tooltip("Pause après la dernière ligne avant de reprendre")]
    public float endPause = 2f;
    
    [Header("Audio (optionnel)")]
    public AudioSource narrationAudio;
    public AudioClip narrationSound;
    [Tooltip("Optionnel: un clip par ligne. Si vide ou manquant, utilise 'narrationSound'.")]
    public AudioClip[] narrationLineClips;

    [Header("Musique")]
    [Tooltip("Musique à jouer pendant la narration (remplace temporairement la musique d'ambiance)")]
    public AudioClip narrationMusic;

    [Header("UI à cacher")]
    [Tooltip("Timer UI à cacher pendant la narration")]
    public GameObject timerUI;

    void Start()
    {
        // Cache tout au début
        if (blackScreenUI) blackScreenUI.SetActive(false);
        if (narrationText) narrationText.text = "";
    }

    /// <summary>
    /// Lance la séquence de narration
    /// Appelée par le BossEnemy quand il meurt
    /// </summary>
    public void PlayNarration()
    {
        StartCoroutine(NarrationSequence());
    }

    IEnumerator NarrationSequence()
    {
        // Fige le jeu et libère le curseur
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Cache le timer pendant la narration
        if (timerUI) timerUI.SetActive(false);

        // Change la musique pour la narration
        if (narrationMusic && MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayLevelClip(narrationMusic);
        }

        // Active l'écran noir
        if (blackScreenUI)
        {
            blackScreenUI.SetActive(true);
        }
        
        if (narrationText)
        {
            narrationText.gameObject.SetActive(true);
            narrationText.text = "";
            
            // Force les paramètres
            narrationText.color = Color.white;
            narrationText.fontSize = 50;
            narrationText.alignment = TMPro.TextAlignmentOptions.Center;
            
            // S'assure que le RectTransform est correctement configuré
            RectTransform rectTransform = narrationText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
            }
            
            // Met le texte EN DERNIER dans la hiérarchie pour qu'il soit au-dessus
            narrationText.transform.SetAsLastSibling();
        }
        
        // S'assure que l'écran noir est en PREMIER (derrière le texte)
        if (blackScreenUI)
        {
            blackScreenUI.transform.SetAsFirstSibling();
        }

        // Fade in progressif
        yield return new WaitForSecondsRealtime(fadeInDuration);

        // Affiche chaque ligne de narration
        for (int i = 0; i < narrationLines.Length; i++)
        {
            string line = narrationLines[i];
            
            if (narrationText)
            {
                narrationText.text = line;
                narrationText.color = new Color(1f, 1f, 1f, 1f);
                narrationText.fontSize = 50;
                
                // Force le rafraîchissement du canvas
                Canvas.ForceUpdateCanvases();
            }
            
            // Son de narration (optionnel) – un clip par ligne si fourni, sinon clip unique
            if (narrationAudio)
            {
                AudioClip clipToPlay = narrationSound;
                if (narrationLineClips != null && i < narrationLineClips.Length && narrationLineClips[i] != null)
                    clipToPlay = narrationLineClips[i];
                if (clipToPlay)
                    narrationAudio.PlayOneShot(clipToPlay);
            }
            
            yield return new WaitForSecondsRealtime(lineDuration);
            
            // Efface le texte avant la ligne suivante
            if (narrationText) narrationText.text = "";
            yield return new WaitForSecondsRealtime(0.5f);
        }

        // Pause finale
        yield return new WaitForSecondsRealtime(endPause);

        // Fade out
        yield return new WaitForSecondsRealtime(fadeOutDuration);

        // Cache l'écran noir et reprend le jeu
        if (blackScreenUI) blackScreenUI.SetActive(false);
        
        // Réaffiche le timer
        if (timerUI) timerUI.SetActive(true);
        
        // Reprend la musique normale du niveau
        if (narrationMusic && MusicManager.Instance != null)
        {
            MusicManager.Instance.ResumeNormalLevelMusic();
        }
        
        // Rétablit le contrôle du joueur
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Version avec effet de typing (machine à écrire)
    /// </summary>
    public void PlayNarrationWithTyping()
    {
        StartCoroutine(NarrationSequenceTyping());
    }

    IEnumerator NarrationSequenceTyping()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Cache le timer pendant la narration
        if (timerUI) timerUI.SetActive(false);

        // Change la musique pour la narration
        if (narrationMusic && MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayLevelClip(narrationMusic);
        }

        if (blackScreenUI) blackScreenUI.SetActive(true);
        if (narrationText) narrationText.text = "";

        yield return new WaitForSecondsRealtime(fadeInDuration);

        // Affiche chaque ligne avec effet typing
        for (int i = 0; i < narrationLines.Length; i++)
        {
            string line = narrationLines[i];
            if (narrationText) narrationText.text = "";
            
            // Son par ligne (au début de la ligne)
            if (narrationAudio)
            {
                AudioClip clipToPlay = narrationSound;
                if (narrationLineClips != null && i < narrationLineClips.Length && narrationLineClips[i] != null)
                    clipToPlay = narrationLineClips[i];
                if (clipToPlay)
                    narrationAudio.PlayOneShot(clipToPlay);
            }

            // Effet machine à écrire
            foreach (char c in line)
            {
                if (narrationText) narrationText.text += c;
                
                // Petit son pour chaque lettre
                if (narrationAudio && narrationSound)
                {
                    narrationAudio.pitch = Random.Range(0.95f, 1.05f);
                    narrationAudio.PlayOneShot(narrationSound, 0.3f);
                }
                
                yield return new WaitForSecondsRealtime(0.05f);
            }
            
            // Pause pour lire la ligne
            yield return new WaitForSecondsRealtime(lineDuration);
            
            // Efface
            if (narrationText) narrationText.text = "";
            yield return new WaitForSecondsRealtime(0.5f);
        }

        yield return new WaitForSecondsRealtime(endPause);
        yield return new WaitForSecondsRealtime(fadeOutDuration);

        if (blackScreenUI) blackScreenUI.SetActive(false);
        
        // Réaffiche le timer
        if (timerUI) timerUI.SetActive(true);
        
        // Reprend la musique normale du niveau
        if (narrationMusic && MusicManager.Instance != null)
        {
            MusicManager.Instance.ResumeNormalLevelMusic();
        }
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
