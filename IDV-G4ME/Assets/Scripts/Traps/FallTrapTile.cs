using UnityEngine;
using System.Collections;

public class FallTrapTile : MonoBehaviour
{
    public Collider trigger;           // zone déclencheur (souvent le BoxCollider du tile en trigger)
    public Collider solid;             // collider solide sur lequel on marche
    public Transform tileMesh;         // visuel du tile qui chute (souvent cet objet)
    public float delay = 0.15f;        // délai avant la chute
    public float dropDistance = 3f;    // distance de chute
    public float dropTime = 0.35f;     // durée de l’animation de chute
    public float resetAfter = -1f;     // <0 = pas de reset, sinon délai avant de remonter

    bool used;

    void Reset()
    {
        // Remplit automatiquement les références de base si rien n’est assigné
        trigger = GetComponent<Collider>();
        solid = GetComponent<Collider>();
        tileMesh = transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        // Le joueur marche sur la tuile : on lance la séquence de chute
        StartCoroutine(Drop());
    }

    IEnumerator Drop()
    {
        used = true;
        yield return new WaitForSeconds(delay);

        // On coupe le collider solide pour laisser tomber le joueur
        if (solid) solid.enabled = false;

        // Animation de descente linéaire
        Vector3 start = tileMesh.position;
        Vector3 end = start + Vector3.down * dropDistance;
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, dropTime);
            tileMesh.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        // Remise en place optionnelle
        if (resetAfter > 0f)
        {
            yield return new WaitForSeconds(resetAfter);

            // Remonte la tuile et réactive le collider
            tileMesh.position = start;
            if (solid) solid.enabled = true;
            used = false;
        }
    }
}
