using UnityEngine;

/// <summary>
/// Système de debug pour se téléporter rapidement vers différents points
/// Activé uniquement en mode développement
/// </summary>
public class DebugTeleporter : MonoBehaviour
{
    [Header("Points de téléportation")]
    [Tooltip("Point devant la porte du boss")]
    public Transform bossRoomEntrance;
    [Tooltip("Point de départ (spawn)")]
    public Transform startPoint;
    
    [Header("Configuration")]
    [Tooltip("Touche pour téléporter devant le boss")]
    public KeyCode teleportToBossKey = KeyCode.F1;
    [Tooltip("Touche pour revenir au spawn")]
    public KeyCode teleportToStartKey = KeyCode.F2;
    [Tooltip("Touche pour tuer instantanément le boss")]
    public KeyCode killBossKey = KeyCode.F3;
    
    [Header("Affichage")]
    [Tooltip("Affiche les instructions à l'écran")]
    public bool showDebugUI = true;
    
    private CharacterController playerController;
    private GameObject player;

    void Start()
    {
        // Trouve le joueur
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerController = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        // N'active le debug que dans l'éditeur ou en build de développement
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        if (Input.GetKeyDown(teleportToBossKey) && bossRoomEntrance != null)
        {
            TeleportPlayer(bossRoomEntrance.position, bossRoomEntrance.rotation);
            Debug.Log("[Debug] Téléportation : Entrée du boss");
        }
        
        if (Input.GetKeyDown(teleportToStartKey) && startPoint != null)
        {
            TeleportPlayer(startPoint.position, startPoint.rotation);
            Debug.Log("[Debug] Téléportation : Point de départ");
        }
        
        if (Input.GetKeyDown(killBossKey))
        {
            KillBoss();
        }
        
        #endif
    }

    void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        if (player == null) return;
        
        // Désactive temporairement le CharacterController pour la téléportation
        if (playerController != null)
        {
            playerController.enabled = false;
            player.transform.SetPositionAndRotation(position, rotation);
            playerController.enabled = true;
        }
        else
        {
            player.transform.SetPositionAndRotation(position, rotation);
        }
    }

    void KillBoss()
    {
        // Trouve le boss dans la scène
        BossEnemy boss = FindObjectOfType<BossEnemy>();
        
        if (boss != null)
        {
            // Force le boss à prendre 3 coups (ou le max) instantanément
            for (int i = 0; i < boss.maxHits; i++)
            {
                boss.TakeHit();
            }
            Debug.Log("[Debug] Boss tué instantanément !");
        }
        else
        {
            Debug.LogWarning("[Debug] Aucun boss trouvé dans la scène.");
        }
    }

    void OnGUI()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        
        if (!showDebugUI) return;
        
        // Affiche les instructions en haut à gauche
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        style.fontSize = 14;
        style.padding = new RectOffset(10, 10, 10, 10);
        
        string instructions = "=== DEBUG MODE ===\n";
        instructions += $"F1: Téléport Boss Room {(bossRoomEntrance ? "✓" : "✗")}\n";
        instructions += $"F2: Téléport Start {(startPoint ? "✓" : "✗")}\n";
        instructions += "F3: Kill Boss Instantly";
        
        GUI.Label(new Rect(10, 10, 300, 100), instructions, style);
        
        #endif
    }
}
