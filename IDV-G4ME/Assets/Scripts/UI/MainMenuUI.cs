using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour
{
    [Header("Références UI")]
    public GameObject panelInfo; // panneau d'informations ou de crédits

    [Header("Scènes")]
    public string firstLevelScene = "Level1"; // première scène de jeu à charger

    public void OnClickPlay()
    {
        // Réinitialise le timer pour une nouvelle partie
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.ResetTimer();
            GameTimer.Instance.StartTimer();
        }

        // Charge la première scène de jeu
        // (optionnel : tu peux ajouter un fade-out via ScreenFader.Instance.FadeOut)
        SceneManager.LoadScene(firstLevelScene);
    }

    public void OnClickInfo()
    {
        // Affiche le panneau d'informations
        if (panelInfo) panelInfo.SetActive(true);
    }

    public void OnClickCloseInfo()
    {
        // Ferme le panneau d'informations
        if (panelInfo) panelInfo.SetActive(false);
    }

    public void OnClickMute()
    {
        // Active/désactive la musique
        if (MusicManager.Instance) MusicManager.Instance.ToggleMute();

        // Tu peux aussi changer dynamiquement le texte du bouton (ex: "Mute" ↔ "Unmute")
    }

    public void OnClickQuit()
    {
        // Quitte le jeu (fonctionne dans l'éditeur ET dans la build)
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
