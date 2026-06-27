using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerFPSController : MonoBehaviour
{
    // --- Références ---
    [Header("Références")]
    [Tooltip("Pivot/caméra pour le pitch (glisser la Main Camera).")]
    public Transform cameraPivot;
    [Tooltip("Origine de l'attaque (un Empty devant la caméra).")]
    public Transform attackOrigin;
    [Tooltip("UI du prompt d'interaction (UI_InteractPrompt).")]
    public GameObject interactPromptUI;
    [Tooltip("Torche (Light enfant), optionnelle.")]
    public Light torch;

    // --- Look souris ---
    [Header("Caméra / Look")]
    public float mouseSensitivityX = 1.5f;
    public float mouseSensitivityY = 1.5f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    // --- Mouvement ---
    [Header("Déplacements")]
    public float moveSpeed = 3.8f;
    public float runMultiplier = 1.6f;
    public float jumpHeight = 1.1f;
    public float gravity = -9.81f;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    // --- Attaque corps-à-corps (placeholder) ---
    [Header("Attaque")]
    public KeyCode attackKey = KeyCode.Mouse0;
    public float attackRange = 1.8f;
    public float attackRadius = 0.25f;
    public LayerMask attackLayers = ~0;

    // --- Divers ---
    [Header("Divers")]
    public KeyCode torchKey = KeyCode.F;

    // internes
    CharacterController cc;
    Vector3 velocity;
    bool isGrounded;
    float camPitch;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Charge les paramètres de sensibilité sauvegardés
        LoadSensitivitySettings();
    }

    void Start()
    {
    }

    void LoadSensitivitySettings()
    {
        // Charge la sensibilité depuis PlayerPrefs (même clés que SettingsMenu)
        mouseSensitivityX = PlayerPrefs.GetFloat("mouse_sensitivity_x", mouseSensitivityX);
        mouseSensitivityY = PlayerPrefs.GetFloat("mouse_sensitivity_y", mouseSensitivityY);
    }

    void Update()
    {
        HandleLook();
        HandleMoveAndJump();
        HandleAttack();
        HandleTorch();
        ApplyGravity();
    }

    // --- Caméra ---
    void HandleLook()
    {
        if (!cameraPivot) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * 100f * Time.deltaTime;

        // Yaw sur le corps
        transform.Rotate(Vector3.up * mouseX);

        // Pitch limité sur le pivot caméra
        camPitch = Mathf.Clamp(camPitch - mouseY, minPitch, maxPitch);
        cameraPivot.localEulerAngles = new Vector3(camPitch, 0f, 0f);
    }

    // --- Mouvement + Saut ---
    void HandleMoveAndJump()
    {
        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < -2f) velocity.y = -2f;

        float ix = Input.GetAxisRaw("Horizontal");
        float iz = Input.GetAxisRaw("Vertical");

        Vector3 input = (transform.right * ix + transform.forward * iz).normalized;
        float speed = moveSpeed * (Input.GetKey(runKey) ? runMultiplier : 1f);

        if (input.sqrMagnitude > 0f)
            cc.Move(input * speed * Time.deltaTime);

        if (isGrounded && Input.GetKeyDown(jumpKey))
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // --- Gravité ---
    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }

    // --- Attaque mêlée ---
    void HandleAttack()
    {
        if (!attackOrigin) return;
        if (!Input.GetKeyDown(attackKey)) return;

        // Effet visuel d'attaque (screen shake)
        if (CameraShake.Instance != null)
            CameraShake.Instance.ShakeAttack();

        // Utilise OverlapSphere à la fin de la portée d'attaque
        Vector3 attackPoint = attackOrigin.position + attackOrigin.forward * attackRange;
        Collider[] hits = Physics.OverlapSphere(attackPoint, attackRadius, attackLayers, QueryTriggerInteraction.Ignore);
        
        if (hits.Length > 0)
        {
            BossEnemy bossHit = null;
            
            foreach (Collider hit in hits)
            {
                // Cherche le BossEnemy sur l'objet touché ET dans toute sa hiérarchie parent
                BossEnemy boss = hit.GetComponentInParent<BossEnemy>();
                
                // Si pas trouvé, cherche aussi sur le Transform.root
                if (boss == null)
                    boss = hit.transform.root.GetComponentInChildren<BossEnemy>();
                
                if (boss != null)
                {
                    bossHit = boss;
                    break;
                }
            }
            
            if (bossHit != null)
            {
                bossHit.TakeHit();
                // Shake plus fort quand on touche le boss
                if (CameraShake.Instance != null)
                    CameraShake.Instance.ShakeHeavy();
            }
        }
    }

    // --- Torche ---
    void HandleTorch()
    {
        if (!torch) return;
        if (Input.GetKeyDown(torchKey))
            torch.enabled = !torch.enabled;
    }

    void OnDrawGizmosSelected()
    {
        if (!attackOrigin) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin.position + attackOrigin.forward * attackRange, attackRadius);
    }
}
