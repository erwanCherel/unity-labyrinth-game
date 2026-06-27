using UnityEngine;

/// <summary>
/// Script de détection pour le trigger de la LockedDoor.
/// À attacher sur un GameObject enfant avec un Collider en mode Trigger.
/// </summary>
public class LockedDoorTriggerRelay : MonoBehaviour
{
    [Tooltip("Référence à la LockedDoor parente")]
    public LockedDoor door;
    
    void Reset()
    {
        // Essaie de trouver automatiquement la LockedDoor parente
        if (door == null)
        {
            door = GetComponentInParent<LockedDoor>();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && door != null)
        {
            door.SetPlayerInside(true);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && door != null)
        {
            door.SetPlayerInside(false);
        }
    }
}
