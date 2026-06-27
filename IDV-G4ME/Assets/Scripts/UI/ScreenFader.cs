using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance { get; private set; }

    [Tooltip("Durée par défaut d’un fondu")]
    public float defaultDuration = 0.6f;

    CanvasGroup group;

    void Awake()
    {
        // Singleton simple + persistant entre scènes
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        group = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeOut(float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        group.blocksRaycasts = true; // bloque les clics pendant le fondu
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            group.alpha = Mathf.SmoothStep(0f, 1f, t);
            yield return null;
        }
        group.alpha = 1f;
    }

    public IEnumerator FadeIn(float duration = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            group.alpha = Mathf.SmoothStep(1f, 0f, t);
            yield return null;
        }
        group.alpha = 0f;
        group.blocksRaycasts = false; // relâche les clics une fois transparent
    }
}
