using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("Références")]
    public TMP_Text label;          // texte TMP affiché dans le Canvas
    public bool showMilliseconds = true;

    void Reset() { label = GetComponent<TMP_Text>(); }

    void Update()
    {
        if (!label || GameTimer.Instance == null) return;
        label.text = FormatTime(GameTimer.Instance.Elapsed, showMilliseconds);
    }

    // Format d'affichage : mm:ss(.cc)
    public static string FormatTime(float t, bool ms)
    {
        if (t < 0f) t = 0f;
        int minutes = (int)(t / 60f);
        int seconds = (int)(t % 60f);
        if (!ms) return $"{minutes:00}:{seconds:00}";
        int centi = (int)((t - Mathf.Floor(t)) * 100f); // centièmes pour un affichage plus précis
        return $"{minutes:00}:{seconds:00}.{centi:00}";
    }
}
