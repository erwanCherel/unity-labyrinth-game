using UnityEngine;

/// <summary>
/// Ajoute automatiquement des blocs au-dessus d'une porte pour combler les espaces
/// </summary>
[ExecuteAlways]
public class DoorFrame : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Largeur de l'ouverture du mur")]
    public float wallOpeningWidth = 2f;
    [Tooltip("Hauteur de l'ouverture du mur")]
    public float wallOpeningHeight = 3f;
    [Tooltip("Hauteur réelle de la porte")]
    public float doorHeight = 2.5f;
    [Tooltip("Épaisseur du mur")]
    public float wallThickness = 0.3f;
    [Tooltip("Matériau pour les blocs de remplissage")]
    public Material fillMaterial;

    [ContextMenu("Create Frame Fillers")]
    public void CreateFrameFillers()
    {
        // Supprime les anciens fillers
        ClearFillers();

        if (doorHeight >= wallOpeningHeight)
        {
            Debug.Log("La porte remplit déjà toute l'ouverture.");
            return;
        }

        float gapHeight = wallOpeningHeight - doorHeight;
        
        // Crée un bloc horizontal au-dessus de la porte (linteau)
        GameObject lintel = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lintel.name = "Filler_Lintel";
        lintel.transform.SetParent(transform);
        lintel.transform.localPosition = new Vector3(0f, doorHeight + gapHeight / 2f, 0f);
        lintel.transform.localScale = new Vector3(wallOpeningWidth, gapHeight, wallThickness);
        
        if (fillMaterial)
            lintel.GetComponent<MeshRenderer>().sharedMaterial = fillMaterial;
        
        lintel.isStatic = true;
        
        Debug.Log($"Cadre de porte créé : linteau de {gapHeight}m de haut");
    }

    [ContextMenu("Clear Frame Fillers")]
    public void ClearFillers()
    {
        // Supprime tous les enfants nommés "Filler_"
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (child.name.StartsWith("Filler_"))
            {
                #if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(child.gameObject);
                else
                    Destroy(child.gameObject);
                #else
                Destroy(child.gameObject);
                #endif
            }
        }
    }
}
