using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Ennemi boss qui peut être frappé et vaincu après un certain nombre de coups
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class BossEnemy : MonoBehaviour
{
    public static System.Action OnBossDefeated; // évènement global
    [Header("Combat")]
    [Tooltip("Nombre de coups nécessaires pour vaincre le boss")]
    public int maxHits = 3;
    [Tooltip("Temps d'invincibilité après chaque coup (secondes)")]
    public float invincibilityTime = 0.5f;
    
    [Header("Comportement")]
    [Tooltip("Le boss poursuit le joueur")]
    public bool chasePlayer = false;
    [Tooltip("Vitesse de déplacement")]
    public float moveSpeed = 2f;
    
    [Header("Déplacement aléatoire")]
    [Tooltip("Centre de la salle (position du boss au départ)")]
    public Vector3 roomCenter = Vector3.zero;
    [Tooltip("Rayon de la zone de déplacement (le boss reste dans ce cercle)")]
    public float wanderRadius = 5f;
    [Tooltip("Temps avant de choisir une nouvelle destination")]
    public float newDestinationTime = 3f;
    
    private float destinationTimer = 0f;
    
    [Header("Fin de combat")]
    [Tooltip("Système de narration à déclencher quand le boss meurt")]
    public BossDefeatNarration narrationSystem;
    
    [Header("Effets visuels")]
    [Tooltip("Matériau à appliquer quand le boss est touché (flash rouge)")]
    public Material hitMaterial;
    [Tooltip("Durée du flash de dégât")]
    public float hitFlashDuration = 0.2f;
    
    [Header("Audio")]
    [Tooltip("Son joué quand le boss est touché")]
    public AudioClip hitSound;
    [Tooltip("Son joué quand le boss meurt")]
    public AudioClip deathSound;
    
    // État interne
    private int currentHits = 0;
    private bool isInvincible = false;
    private bool isDead = false;
    private NavMeshAgent agent;
    private Transform player;
    private Renderer[] renderers;
    private Material[] originalMaterials;
    private AudioSource audioSource;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        
        // Récupère tous les renderers pour l'effet de hit
        renderers = GetComponentsInChildren<Renderer>();
        
        // Sauvegarde les matériaux originaux
        if (renderers.Length > 0)
        {
            originalMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    originalMaterials[i] = renderers[i].material;
            }
        }
        
        // Trouve ou crée un AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.spatialBlend = 1f; // Son 3D
        audioSource.maxDistance = 20f;
    }

    void Start()
    {
        // Trouve le joueur
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        // Sauvegarde la position de départ comme centre de la salle
        if (roomCenter == Vector3.zero)
            roomCenter = transform.position;
        
        // Commence à errer dans la salle
        if (!chasePlayer)
        {
            ChooseNewDestination();
        }
    }

    void Update()
    {
        if (isDead) return;
        
        if (chasePlayer && player != null)
        {
            // Mode poursuite (désactivé par défaut)
            agent.SetDestination(player.position);
        }
        else
        {
            // Mode errance aléatoire
            WanderAround();
        }
    }

    void WanderAround()
    {
        destinationTimer -= Time.deltaTime;
        
        // Temps de choisir une nouvelle destination
        if (destinationTimer <= 0f)
        {
            ChooseNewDestination();
        }
    }

    void ChooseNewDestination()
    {
        // Choisit un point aléatoire dans un cercle autour du centre
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;
        Vector3 destination = roomCenter + new Vector3(randomPoint.x, 0f, randomPoint.y);
        
        agent.SetDestination(destination);
        destinationTimer = newDestinationTime;
    }

    /// <summary>
    /// Appelé quand le joueur attaque le boss (depuis PlayerFPSController par exemple)
    /// </summary>
    public void TakeHit()
    {
        if (isDead || isInvincible) return;
        
        currentHits++;
        
        // Effet visuel de hit
        if (hitMaterial != null)
            StartCoroutine(FlashHit());
        
        // Son de hit
        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);
        
        // Invincibilité temporaire
        StartCoroutine(InvincibilityFrames());
        
        // Vérifie si le boss est vaincu
        if (currentHits >= maxHits)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashHit()
    {
        // Change tous les matériaux en rouge
        if (hitMaterial != null && renderers != null)
        {
            foreach (var r in renderers)
            {
                if (r != null)
                    r.material = hitMaterial;
            }
        }
        
        yield return new WaitForSeconds(hitFlashDuration);
        
        // Restaure les matériaux originaux
        if (renderers != null && originalMaterials != null)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && originalMaterials[i] != null)
                    renderers[i].material = originalMaterials[i];
            }
        }
    }

    System.Collections.IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        // Son de mort
        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);
        
        // Arrête le NavMeshAgent
        if (agent != null)
            agent.isStopped = true;
        
        // Déclenche la narration
        if (narrationSystem != null)
        {
            narrationSystem.PlayNarration();
        }

        // Disparition immédiate du boss (au 3e coup)
        gameObject.SetActive(false);

        // Notifie le reste du jeu que le boss est vaincu
        OnBossDefeated?.Invoke();
    }
    

    void OnDrawGizmosSelected()
    {
        // Affiche la zone de déplacement dans l'éditeur
        Gizmos.color = Color.yellow;
        Vector3 center = roomCenter != Vector3.zero ? roomCenter : transform.position;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}
