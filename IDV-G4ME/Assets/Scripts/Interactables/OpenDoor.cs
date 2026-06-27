using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OpenDoor : MonoBehaviour
{
    // ---------------- UI ----------------
    [Header("UI")]
    [Tooltip("UI du message d'interaction à masquer quand la porte s'ouvre.")]
    public GameObject interactPromptUI;

    // -------------- Références ----------
    [Header("Références")]
    [Tooltip("Mesh de la porte (Transform). Glisser l’objet parent si besoin.")]
    public Transform doorMesh;
    [Tooltip("Collider qui bloque la porte (souvent un BoxCollider). Désactivé au début de l'ouverture.")]
    public Collider doorBlocker;

    // -------- Paramètres d'ouverture ----
    [Header("Paramètres d'ouverture")]
    [Tooltip("Direction locale du mouvement d’ouverture (ex: Up, Right, etc.).")]
    public Vector3 openDirection = Vector3.up;
    [Tooltip("Distance d’ouverture en mètres.")]
    public float openHeight = 3.5f;
    [Tooltip("Durée totale de l’ouverture (en secondes).")]
    public float openDuration = 0.9f;
    [Tooltip("Courbe d’animation de l’ouverture (optionnelle).")]
    public AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // -------- Transition de niveau ------
    [Header("Transition de niveau")]
    [Tooltip("Nom de la scène à charger après ouverture (laisser vide pour juste ouvrir la porte).")]
    public string nextSceneName = "";
    [Tooltip("Durée du fondu visuel (en secondes).")]
    public float fadeDuration = 0.6f;
    [Tooltip("Petite pause avant de lancer le fade après l’ouverture.")]
    public float postOpenHold = 0.15f;

    // --------- Options d’interaction ----
    [Header("Interaction")]
    [Tooltip("Désactive le trigger une fois la porte ouverte (évite que le prompt réapparaisse).")]
    public bool disableTriggerAfterOpen = true;

    // --------------- État ----------------
    bool opened;
    bool playerInside;
    Collider cachedTrigger;    // Trigger enfant, si présent

    // Accès rapide pour d’autres scripts (utile pour les triggers)
    public bool IsOpened => opened;

    // Appelé par le trigger pour signaler la présence du joueur
    public void SetPlayerInside(bool inside)
    {
        playerInside = inside;
        // Si la porte est déjà ouverte, on s’assure que l’UI reste cachée
        if (opened && interactPromptUI) interactPromptUI.SetActive(false);
    }

    void Reset()
    {
        // Remplit automatiquement les références si elles sont manquantes
        if (!doorMesh) doorMesh = transform;
        if (!doorBlocker) doorBlocker = GetComponent<Collider>();
    }

    void Awake()
    {
        // Récupère le premier collider enfant configuré en trigger
        foreach (var col in GetComponentsInChildren<Collider>(true))
        {
            if (col != null && col.isTrigger) { cachedTrigger = col; break; }
        }
    }

    void Update()
    {
        // Interaction directe : le joueur appuie sur E
        if (!opened && playerInside && Input.GetKeyDown(KeyCode.E))
            StartCoroutine(OpenAndOpen());
    }

    /// <summary>Permet d’ouvrir la porte via un autre script (ex: interaction externe).</summary>
    public void Interact()
    {
        if (!opened) StartCoroutine(OpenAndOpen());
    }

    public IEnumerator OpenAndOpen()
    {
        if (opened) yield break;
        opened = true;

        // 1) Cache le prompt d'interaction immédiatement
        if (interactPromptUI) interactPromptUI.SetActive(false);

        // 2) Désactive le collider bloquant
        if (doorBlocker) doorBlocker.enabled = false;

        // 3) Calcule la position cible en prenant en compte la direction locale
        Vector3 start = doorMesh.position;
        Vector3 dirW = doorMesh.TransformDirection(openDirection).normalized;
        if (dirW.sqrMagnitude < 0.0001f) dirW = Vector3.up; // sécurité au cas où
        Vector3 target = start + dirW * openHeight;

        // 4) Animation de l'ouverture (temps réel, non lié au timeScale)
        float t = 0f;
        float dur = Mathf.Max(0.01f, openDuration);
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / dur;
            float k = openCurve != null ? openCurve.Evaluate(t) : t;
            doorMesh.position = Vector3.LerpUnclamped(start, target, k);
            yield return null;
        }
        doorMesh.position = target;

        // 5) Désactive le trigger si nécessaire
        if (disableTriggerAfterOpen && cachedTrigger)
            cachedTrigger.gameObject.SetActive(false);

        // 6) Petite pause avant la transition
        yield return new WaitForSecondsRealtime(postOpenHold);

        // 7) Transition vers une autre scène si spécifiée
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            if (ScreenFader.Instance)
                yield return ScreenFader.Instance.FadeOut(fadeDuration);

            var op = SceneManager.LoadSceneAsync(nextSceneName);
            while (!op.isDone) yield return null;

            if (ScreenFader.Instance)
                yield return ScreenFader.Instance.FadeIn(fadeDuration);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // Visualisation de la direction d’ouverture dans la scène
        if (!doorMesh) return;
        Gizmos.color = Color.cyan;
        Vector3 p = doorMesh.position;
        Vector3 d = doorMesh.TransformDirection(openDirection.normalized) * Mathf.Max(0.5f, openHeight * 0.3f);
        Gizmos.DrawLine(p, p + d);
        Gizmos.DrawSphere(p + d, 0.05f);
    }
#endif
}
