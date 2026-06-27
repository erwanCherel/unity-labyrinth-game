using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntranceForestSpawner : MonoBehaviour
{
    [Header("Prefab d'arbre (PF_Tree)")]
    public GameObject treePrefab;          // prefab du Project (pas une instance de scène)

    [Header("Porte d'entrée")]
    public Transform gate;                 // parent de l’EntryDoor
    [Tooltip("+1 = dans le sens de la porte ; -1 = côté opposé (extérieur si la porte regarde l'intérieur)")]
    public int forwardSign = -1;

    [Header("Rangées d'arbres")]
    public int rows = 4;                   // nombre de rangées
    public float spacing = 3f;             // écart entre rangées ET entre troncs
    public float patchWidth = 60f;         // largeur du massif (gauche-droite)
    public float forwardOffset = 1f;       // retrait avant la 1re rangée

    [Header("Allée dégagée (toutes sauf la dernière rangée)")]
    public float pathWidth = 6f;
    public bool makePath = true;
    public Material pathMaterial;
    public float pathYOffset = 0.01f;

    [Header("Sol / placement")]
    public float groundY = 0f;             // Y du sol extérieur
    [Range(0, 2)] public float jitter = 0.5f;
    public bool alignByBounds = true;      // aligne la base visuelle au sol si le pivot n’est pas au pied

    [Header("Mur invisible (optionnel)")]
    public bool makeBlocker = false;
    public float blockerHeight = 3f;
    public float blockerThickness = 2f;

    [ContextMenu("Spawn / Regenerate")]
    [System.Obsolete]
    public void Spawn()
    {
        if (!treePrefab) { Debug.LogError("Assigne treePrefab (PF_Tree.prefab)"); return; }
        if (!gate) { Debug.LogError("Assigne gate (EntryDoor parent)"); return; }
        rows = Mathf.Max(1, rows);
        if (forwardSign != 1 && forwardSign != -1) forwardSign = -1;

        // Nettoyage de toute génération précédente
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // Projette forward/right sur XZ pour garder des rangées régulières
        Vector3 f = gate.forward; f.y = 0f; if (f.sqrMagnitude < 1e-4f) f = Vector3.forward; f = f.normalized * forwardSign;
        Vector3 r = gate.right; r.y = 0f; if (r.sqrMagnitude < 1e-4f) r = Vector3.right; r = r.normalized;

        float halfW = patchWidth * 0.5f;

        // Placement rangée par rangée
        for (int row = 0; row < rows; row++)
        {
            float forwardDist = forwardOffset + row * spacing;

            for (float x = -halfW; x <= halfW + 0.01f; x += spacing)
            {
                // Laisse une allée libre au centre (sauf dernière rangée)
                if (row < rows - 1 && Mathf.Abs(x) < pathWidth * 0.5f) continue;

                float jx = Random.Range(-jitter, jitter);
                float jz = Random.Range(-jitter, jitter);
                Vector3 pos = gate.position + r * (x + jx) + f * (forwardDist + jz);

                PlaceTree(pos, Quaternion.Euler(0, Random.Range(0f, 360f), 0), Random.Range(0.95f, 1.08f));
            }
        }

        // Chemin en dalles (jusqu’avant la rangée pleine)
        if (makePath && pathMaterial && rows >= 2)
        {
            float pathDepth = (rows - 1) * spacing;
            Vector3 center = gate.position + f * (forwardOffset + pathDepth * 0.5f);

            var q = GameObject.CreatePrimitive(PrimitiveType.Quad);
            q.name = "StonePath";
            q.transform.SetParent(transform, false);
            q.transform.position = new Vector3(center.x, groundY + pathYOffset, center.z);
            q.transform.rotation = Quaternion.LookRotation(f, Vector3.up) * Quaternion.Euler(90, 0, 0);
            q.transform.localScale = new Vector3(pathWidth, pathDepth, 1f);

            var mr = q.GetComponent<MeshRenderer>(); if (mr) mr.sharedMaterial = pathMaterial;
            var col = q.GetComponent<MeshCollider>(); if (col) DestroyImmediate(col);
        }

        // Mur invisible (optionnel) après la dernière rangée
        if (makeBlocker)
        {
            float endDist = forwardOffset + (rows - 1) * spacing + blockerThickness * 0.5f;
            Vector3 center = gate.position + f * endDist;

            var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = "Blocker_End";
            b.transform.SetParent(transform, false);
            b.transform.position = new Vector3(center.x, groundY + blockerHeight * 0.5f, center.z);
            b.transform.rotation = Quaternion.LookRotation(f, Vector3.up);
            b.transform.localScale = new Vector3(patchWidth, blockerHeight, blockerThickness);

            var mrB = b.GetComponent<MeshRenderer>(); if (mrB) mrB.enabled = false;
            var bc = b.GetComponent<BoxCollider>(); if (bc) bc.isTrigger = false;
        }

        Debug.Log("Entrance forest generated.");
    }

    [System.Obsolete]
    void PlaceTree(Vector3 pos, Quaternion rot, float scl)
    {
        GameObject go;
#if UNITY_EDITOR
        // Instancie à partir du prefab Project (préserve le lien prefab)
        go = PrefabUtility.IsPartOfPrefabAsset(treePrefab)
            ? (GameObject)PrefabUtility.InstantiatePrefab(treePrefab, transform)
            : Instantiate(treePrefab, transform);
#else
        go = Instantiate(treePrefab, transform);
#endif

        go.transform.SetPositionAndRotation(new Vector3(pos.x, groundY, pos.z), rot);
        go.transform.localScale = Vector3.one * scl;

        if (alignByBounds)
        {
            // Aligne le bas visuel au niveau du sol si le pivot n’est pas au pied
            var rend = go.GetComponentInChildren<Renderer>();
            if (rend)
            {
                float bottom = rend.bounds.min.y;
                float dy = bottom - groundY;
                if (Mathf.Abs(dy) > 0.001f)
                    go.transform.position += Vector3.down * dy;
            }
        }

#if UNITY_EDITOR
        // Marque statique pour batching/navmesh
        GameObjectUtility.SetStaticEditorFlags(
            go, StaticEditorFlags.BatchingStatic | StaticEditorFlags.NavigationStatic);
#endif
    }

    void OnDrawGizmosSelected()
    {
        if (!gate) return;

        Vector3 f = gate.forward; f.y = 0f; if (f.sqrMagnitude < 1e-4f) f = Vector3.forward; f = f.normalized * forwardSign;
        Vector3 r = gate.right; r.y = 0f; if (r.sqrMagnitude < 1e-4f) r = Vector3.right; r = r.normalized;

        float halfW = patchWidth * 0.5f;
        float depth = (rows - 1) * spacing + spacing;

        // Emprise complète (vert)
        Gizmos.color = new Color(0, 1, 0, 0.25f);
        Vector3 A = gate.position + r * -halfW + f * forwardOffset;
        Vector3 B = gate.position + r * halfW + f * forwardOffset;
        Vector3 C = gate.position + r * halfW + f * (forwardOffset + depth);
        Vector3 D = gate.position + r * -halfW + f * (forwardOffset + depth);
        Draw(A, B, C, D);

        // Allée centrale (jaune) jusqu’à l’avant-dernière rangée
        if (rows >= 2)
        {
            Gizmos.color = new Color(1, 1, 0, 0.4f);
            float pathDepth = (rows - 1) * spacing;
            Vector3 A2 = gate.position + r * -(pathWidth * 0.5f) + f * forwardOffset;
            Vector3 B2 = gate.position + r * (pathWidth * 0.5f) + f * forwardOffset;
            Vector3 C2 = gate.position + r * (pathWidth * 0.5f) + f * (forwardOffset + pathDepth);
            Vector3 D2 = gate.position + r * -(pathWidth * 0.5f) + f * (forwardOffset + pathDepth);
            Draw(A2, B2, C2, D2);
        }

        void Draw(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        { Gizmos.DrawLine(p1, p2); Gizmos.DrawLine(p2, p3); Gizmos.DrawLine(p3, p4); Gizmos.DrawLine(p4, p1); }
    }
}
