using UnityEngine;

public class OpenTriggerRelay : MonoBehaviour
{
    public OpenDoor door;                  // Référence à la porte (je mets le OpenDoor parent ici)
    public GameObject interactPromptUI;    // UI du prompt d'interaction (ex: TextMeshPro)

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[EntryTrigger] enter by '{other.name}' tag='{other.tag}'");
        if (!other.CompareTag("Player")) return;

        // Le joueur est dans la zone : j'informe la porte et j'affiche le prompt
        door.SetPlayerInside(true);
        
        // N'affiche le prompt QUE si la porte n'est pas encore ouverte
        if (interactPromptUI && !door.IsOpened)
            interactPromptUI.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"[EntryTrigger] exit by '{other.name}' tag='{other.tag}'");
        if (!other.CompareTag("Player")) return;

        // Le joueur quitte la zone : j'informe la porte et je cache le prompt
        door.SetPlayerInside(false);
        if (interactPromptUI) interactPromptUI.SetActive(false);
    }

    void Update()
    {
        // Cache le prompt si la porte s'ouvre pendant que le joueur est dans la zone
        if (door != null && door.IsOpened && interactPromptUI && interactPromptUI.activeSelf)
            interactPromptUI.SetActive(false);
    }
}
