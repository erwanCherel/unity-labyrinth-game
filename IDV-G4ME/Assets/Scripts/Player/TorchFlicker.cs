using UnityEngine;

[RequireComponent(typeof(Light))]
public class TorchFlicker : MonoBehaviour
{
    [Header("Intensité")]
    [Tooltip("Intensité de base de la lumière")]
    public float baseIntensity = 2.5f;
    [Tooltip("Variation d'intensité (flamme qui bouge)")]
    public float flickerAmount = 0.3f;
    [Tooltip("Vitesse du scintillement")]
    public float flickerSpeed = 8f;

    [Header("Couleur")]
    [Tooltip("Couleur de base de la flamme")]
    public Color baseColor = new Color(1f, 0.6f, 0.2f); // Orange chaud
    [Tooltip("Variation de couleur")]
    public float colorVariation = 0.1f;

    [Header("Portée")]
    [Tooltip("Portée de base de la lumière")]
    public float baseRange = 8f;
    [Tooltip("Variation de portée")]
    public float rangeVariation = 0.5f;

    private Light torchLight;
    private float timeOffset;

    void Awake()
    {
        torchLight = GetComponent<Light>();
        
        // Offset aléatoire pour que chaque torche soit unique
        timeOffset = Random.Range(0f, 100f);
        
        // Configure le type de lumière
        torchLight.type = LightType.Point;
        torchLight.shadows = LightShadows.Soft; // Ombres douces optionnelles
    }

    void Update()
    {
        // Crée un effet de scintillement avec plusieurs ondes de bruit
        float time = Time.time * flickerSpeed + timeOffset;
        
        // Combine plusieurs fréquences pour un effet plus naturel
        float flicker1 = Mathf.PerlinNoise(time, 0f);
        float flicker2 = Mathf.PerlinNoise(time * 1.7f, 50f) * 0.5f;
        float flickerValue = (flicker1 + flicker2) - 0.75f;
        
        // Applique l'intensité
        torchLight.intensity = baseIntensity + (flickerValue * flickerAmount);
        
        // Variation subtile de couleur (plus rouge = flamme faible, plus jaune = flamme forte)
        float colorShift = flickerValue * colorVariation;
        torchLight.color = new Color(
            baseColor.r + colorShift,
            baseColor.g,
            baseColor.b - colorShift * 0.5f
        );
        
        // Variation de portée
        torchLight.range = baseRange + (flickerValue * rangeVariation);
    }

    // Pour tester les valeurs dans l'éditeur
    void OnValidate()
    {
        if (torchLight == null) torchLight = GetComponent<Light>();
        torchLight.intensity = baseIntensity;
        torchLight.color = baseColor;
        torchLight.range = baseRange;
    }
}
