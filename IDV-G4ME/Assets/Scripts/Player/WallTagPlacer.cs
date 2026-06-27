using UnityEngine;

/// Permet au joueur de placer un petit marqueur sur les murs pour se repérer
public class WallTagPlacer : MonoBehaviour
{
    [Header("Input")] public int mouseButton = 1; // 1 = clic droit

    [Header("Placement")]
    public Transform rayOrigin;          // généralement la caméra
    public float maxDistance = 6f;
    public LayerMask hitLayers = ~0;

    [Header("Apparence")]
    public Color tagColor = Color.red;
    public float tagSize = 0.15f;        // taille de la croix
    public float tagThickness = 0.03f;   // épaisseur des branches
    public float tagOffset = 0.01f;      // pour éviter le z-fighting
    public int maxTags = 200;            // éviter l'infini

    int currentCount;

    void Awake()
    {
        // Auto-assign si non défini
        if (!rayOrigin)
        {
            var cam = GetComponentInChildren<Camera>() ?? Camera.main;
            if (cam) rayOrigin = cam.transform;
        }
    }

    void Update()
    {
        if (!rayOrigin) return;
        if (!Input.GetMouseButtonDown(mouseButton)) return;

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out var hit, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
        {
            PlaceTag(hit.point, hit.normal, hit.collider.transform);
        }
    }

    void PlaceTag(Vector3 point, Vector3 normal, Transform parent)
    {
        if (currentCount >= maxTags) return;

        // Crée un objet parent pour la croix en position mondiale
        var go = new GameObject("WallTag");
        go.layer = parent.gameObject.layer;
        go.transform.position = point + normal * tagOffset;
        go.transform.rotation = Quaternion.LookRotation(-normal);
        go.transform.SetParent(parent, true); // worldPositionStays = true

        // Trouve le shader approprié
        Shader shader = Shader.Find("Unlit/Color");
        if (shader == null) shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("HDRP/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        // Branche verticale de la croix (fine et haute)
        var vertical = GameObject.CreatePrimitive(PrimitiveType.Quad);
        vertical.name = "Vertical";
        vertical.transform.SetParent(go.transform, false);
        vertical.transform.localPosition = Vector3.zero;
        vertical.transform.localRotation = Quaternion.identity;
        vertical.transform.localScale = new Vector3(tagThickness, tagSize, 1f);
        var mrV = vertical.GetComponent<MeshRenderer>();
        var matV = new Material(shader);
        if (matV.HasProperty("_Color")) matV.color = tagColor;
        else if (matV.HasProperty("_BaseColor")) matV.SetColor("_BaseColor", tagColor);
        mrV.material = matV;
        var colV = vertical.GetComponent<Collider>();
        if (colV) GameObject.Destroy(colV);

        // Branche horizontale de la croix (large et fine)
        var horizontal = GameObject.CreatePrimitive(PrimitiveType.Quad);
        horizontal.name = "Horizontal";
        horizontal.transform.SetParent(go.transform, false);
        horizontal.transform.localPosition = Vector3.zero;
        horizontal.transform.localRotation = Quaternion.identity;
        horizontal.transform.localScale = new Vector3(tagSize, tagThickness, 1f);
        var mrH = horizontal.GetComponent<MeshRenderer>();
        var matH = new Material(shader);
        if (matH.HasProperty("_Color")) matH.color = tagColor;
        else if (matH.HasProperty("_BaseColor")) matH.SetColor("_BaseColor", tagColor);
        mrH.material = matH;
        var colH = horizontal.GetComponent<Collider>();
        if (colH) GameObject.Destroy(colH);

        currentCount++;
    }
}
