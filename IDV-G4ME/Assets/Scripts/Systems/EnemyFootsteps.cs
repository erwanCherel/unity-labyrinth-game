using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class EnemyFootsteps : MonoBehaviour
{
    public AudioClip[] clips;
    public float stepDistance = 1.7f;  // distance parcourue entre deux pas
    public float volume = 1f;

    NavMeshAgent agent;
    AudioSource src;
    Vector3 lastPos;
    float accumulator;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        src = GetComponent<AudioSource>();

        // Réglages de base pour un son 3D propre
        src.playOnAwake = false;
        src.spatialBlend = 1f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.maxDistance = 18f;

        lastPos = transform.position;
    }

    void Update()
    {
        // Distance horizontale parcourue depuis la dernière frame
        Vector3 p = transform.position;
        float dt = Vector3.Distance(new Vector3(p.x, 0, p.z), new Vector3(lastPos.x, 0, lastPos.z));
        lastPos = p;

        // Pas si l’agent reste immobile
        if (agent == null || agent.velocity.sqrMagnitude < 0.01f) return;

        accumulator += dt;
        if (accumulator >= stepDistance)
        {
            accumulator = 0f;
            PlayStep();
        }
    }

    void PlayStep()
    {
        if (clips == null || clips.Length == 0) return;
        int i = Random.Range(0, clips.Length);
        src.PlayOneShot(clips[i], volume);
    }
}
