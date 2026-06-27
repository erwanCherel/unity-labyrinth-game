using UnityEngine;
using System.Collections;

/// <summary>
/// Fait trembler la caméra pour donner un impact visuel
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Shake Settings")]
    [Tooltip("Intensité par défaut du shake")]
    public float defaultIntensity = 0.3f;
    [Tooltip("Durée par défaut du shake")]
    public float defaultDuration = 0.2f;

    private Vector3 originalPosition;
    private bool isShaking = false;

    void Awake()
    {
        // Singleton simple
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// Déclenche un shake de caméra
    /// </summary>
    public void Shake(float duration = -1f, float intensity = -1f)
    {
        if (isShaking) return; // Ne pas stacker les shakes
        
        float shakeDuration = duration > 0 ? duration : defaultDuration;
        float shakeIntensity = intensity > 0 ? intensity : defaultIntensity;
        
        StartCoroutine(ShakeCoroutine(shakeDuration, shakeIntensity));
    }

    IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        isShaking = true;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            // Génère une position aléatoire dans une sphère
            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            randomOffset.z = 0; // Pas de shake en profondeur
            
            transform.localPosition = originalPosition + randomOffset;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Retour à la position d'origine
        transform.localPosition = originalPosition;
        isShaking = false;
    }

    /// <summary>
    /// Shake rapide pour une attaque
    /// </summary>
    public void ShakeAttack()
    {
        Shake(0.15f, 0.2f);
    }

    /// <summary>
    /// Shake fort pour un gros impact
    /// </summary>
    public void ShakeHeavy()
    {
        Shake(0.3f, 0.4f);
    }
}
