using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuBootstrap : MonoBehaviour
{
    IEnumerator Start()
    {
        // Remet le jeu dans un état "propre" (utile si on revient depuis une partie)
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Arrête le timer dans le menu
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.StopTimer();
        }

        // Lance un fade-in rapide si un ScreenFader global est présent
        if (ScreenFader.Instance)
            yield return ScreenFader.Instance.FadeIn(0.25f);

        // Supprime toute sélection UI précédente (évite le focus sur un ancien bouton)
        EventSystem.current?.SetSelectedGameObject(null);
    }
}
