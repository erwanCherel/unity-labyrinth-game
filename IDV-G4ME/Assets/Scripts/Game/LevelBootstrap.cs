using System.Collections;
using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [Tooltip("Durée du fondu d'entrée au chargement de la scène (si un ScreenFader existe).")]
    public float fadeInDuration = 0.6f;

    void Start()
    {
        // Je remets le timeScale au cas où on viendrait d'un menu pause
        Time.timeScale = 1f;

        // Crée un GameTimer s'il n'existe pas déjà
        if (GameTimer.Instance == null)
        {
            var timerObj = new GameObject("_GameTimer");
            timerObj.AddComponent<GameTimer>();
        }

        // Démarre le timer s'il n'est pas déjà en cours (pour le premier niveau)
        // Mais ne le réinitialise PAS pour garder le temps total
        if (GameTimer.Instance != null && !GameTimer.Instance.Running)
        {
            GameTimer.Instance.StartTimer();
        }

        // Lance un fade-in si un fader persistant est disponible
        if (ScreenFader.Instance)
            StartCoroutine(ScreenFader.Instance.FadeIn(fadeInDuration));
    }
}
