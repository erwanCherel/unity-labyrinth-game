using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    public GameObject menuButton; // Bouton "Menu principal"

    void Start()
    {
        // Cache le bouton au démarrage (il sera affiché à la fin)
        if (menuButton) menuButton.SetActive(false);
    }

    // Appelé quand l'UI de fin doit proposer le retour au menu
    public void ShowMenuButton()
    {
        if (menuButton) menuButton.SetActive(true);
    }

    // Action du bouton : revenir au menu principal
    public void BackToMenu()
    {
        Time.timeScale = 1f; // sécurité au cas où le jeu serait en pause
        SceneManager.LoadScene("MainMenu");
    }
}
