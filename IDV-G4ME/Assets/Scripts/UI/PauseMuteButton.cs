using UnityEngine;
using TMPro; // Utilise TextMeshPro pour le libellé du bouton

public class PauseMuteButton : MonoBehaviour
{
    public TMP_Text label; // Texte du bouton dans le menu pause

    void OnEnable() { Refresh(); }

    public void OnClickToggle()
    {
        // Bascule l'état mute de la musique puis met à jour le libellé
        if (MusicManager.Instance != null)
            MusicManager.Instance.ToggleMute();
        Refresh();
    }

    void Refresh()
    {
        if (!label) return;
        bool muted = MusicManager.Instance != null && MusicManager.Instance.IsMuted;
        label.text = muted ? "Mute" : "Sound";
    }
}
