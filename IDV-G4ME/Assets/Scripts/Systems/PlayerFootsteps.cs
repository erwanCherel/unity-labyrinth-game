using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Banques de sons")]
    public AudioClip[] stoneClips; // pavés / béton (tags: Stone, Concrete)
    public AudioClip[] woodClips;  // parquet / intérieur (tags: Wood, Floor)

    [Header("Détection sol")]
    public float rayDistance = 2f;
    public LayerMask groundMask = ~0; // par défaut: tout

    [Header("Déclenchement")]
    [Tooltip("Distance horizontale entre deux pas (mètres).")]
    public float stepDistance = 1.6f;
    [Tooltip("Vitesse minimale pour déclencher (m/s).")]
    public float minSpeed = 0.1f;

    [Header("Audio")]
    [Range(0f, 1f)] public float volume = 1f;
    public float maxDistance = 12f;
    public bool randomizePitch = true;

    AudioSource src;
    Vector3 lastPos;
    float accumulator;

    void Awake()
    {
        src = GetComponent<AudioSource>();

        // Réglages de base du son 3D
        src.playOnAwake = false;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.maxDistance = maxDistance;

        lastPos = transform.position;
    }

    void Update()
    {
        // Distance horizontale parcourue depuis la dernière frame
        Vector3 p = transform.position;
        float dxz = Vector3.Distance(new Vector3(p.x, 0, p.z), new Vector3(lastPos.x, 0, lastPos.z));
        lastPos = p;

        // Vitesse horizontale (sert de seuil pour éviter les bruits à l'arrêt)
        float speed = dxz / Mathf.Max(Time.deltaTime, 0.0001f);
        if (speed < minSpeed) return;

        // On vérifie le sol sous le joueur
        if (!Grounded(out RaycastHit hit)) { accumulator = 0f; return; }

        // Déclenche un pas au bout d'une certaine distance cumulée
        accumulator += dxz;
        if (accumulator >= stepDistance)
        {
            accumulator = 0f;
            var bank = SelectBankByTag(hit.collider.tag);
            if (bank == null || bank.Length == 0) return;
            PlayOneFrom(bank);
        }
    }

    // Renvoie vrai si un sol est détecté sous le joueur et fournit l'impact le plus proche
    bool Grounded(out RaycastHit nearest)
    {
        nearest = default;
        var origin = transform.position + Vector3.up * 0.2f;
        var hits = Physics.RaycastAll(origin, Vector3.down, rayDistance, groundMask, QueryTriggerInteraction.Ignore);
        if (hits.Length == 0) return false;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        nearest = hits[0];
        return true;
    }

    // Choix de la banque de sons en fonction du tag du sol
    AudioClip[] SelectBankByTag(string tag)
    {
        switch (tag)
        {
            case "Wood":
            case "Floor": return woodClips;
            case "Stone":
            case "Concrete": return stoneClips;
            default: return stoneClips; // fallback
        }
    }

    // Joue un clip aléatoire de la banque, avec pitch optionnellement varié
    void PlayOneFrom(AudioClip[] bank)
    {
        if (bank == null || bank.Length == 0) return;
        int i = Random.Range(0, bank.Length);
        src.pitch = randomizePitch ? Random.Range(0.95f, 1.05f) : 1f;
        src.PlayOneShot(bank[i], volume);
    }
}
