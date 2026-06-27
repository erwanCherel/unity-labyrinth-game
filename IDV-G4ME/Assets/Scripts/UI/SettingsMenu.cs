using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    public Slider sensitivityXSlider;
    public Slider sensitivityYSlider;
    public TMP_Text sensitivityXLabel;
    public TMP_Text sensitivityYLabel;

    [Header("Settings Range")]
    public float minSensitivity = 1f;
    public float maxSensitivity = 5f;

    // Clés pour sauvegarder les préférences
    const string PREF_SENS_X = "mouse_sensitivity_x";
    const string PREF_SENS_Y = "mouse_sensitivity_y";

    void OnEnable()
    {
        // Initialise les sliders avec les valeurs sauvegardées (ou par défaut)
        float savedX = PlayerPrefs.GetFloat(PREF_SENS_X, 1.5f);
        float savedY = PlayerPrefs.GetFloat(PREF_SENS_Y, 1.5f);

        if (sensitivityXSlider)
        {
            sensitivityXSlider.minValue = minSensitivity;
            sensitivityXSlider.maxValue = maxSensitivity;
            sensitivityXSlider.value = savedX;
            sensitivityXSlider.onValueChanged.AddListener(OnSensitivityXChanged);
        }

        if (sensitivityYSlider)
        {
            sensitivityYSlider.minValue = minSensitivity;
            sensitivityYSlider.maxValue = maxSensitivity;
            sensitivityYSlider.value = savedY;
            sensitivityYSlider.onValueChanged.AddListener(OnSensitivityYChanged);
        }

        // Affiche les valeurs initiales
        UpdateLabels();
    }

    void OnDisable()
    {
        // Nettoie les listeners
        if (sensitivityXSlider) sensitivityXSlider.onValueChanged.RemoveListener(OnSensitivityXChanged);
        if (sensitivityYSlider) sensitivityYSlider.onValueChanged.RemoveListener(OnSensitivityYChanged);
    }

    void OnSensitivityXChanged(float value)
    {
        // Sauvegarde la nouvelle valeur
        PlayerPrefs.SetFloat(PREF_SENS_X, value);
        PlayerPrefs.Save();

        // Applique au joueur si présent dans la scène
        ApplyToPlayer();

        // Met à jour le label
        UpdateLabels();
    }

    void OnSensitivityYChanged(float value)
    {
        // Sauvegarde la nouvelle valeur
        PlayerPrefs.SetFloat(PREF_SENS_Y, value);
        PlayerPrefs.Save();

        // Applique au joueur si présent dans la scène
        ApplyToPlayer();

        // Met à jour le label
        UpdateLabels();
    }

    void UpdateLabels()
    {
        if (sensitivityXLabel && sensitivityXSlider)
            sensitivityXLabel.text = $"Mouse X: {sensitivityXSlider.value:F1}";

        if (sensitivityYLabel && sensitivityYSlider)
            sensitivityYLabel.text = $"Mouse Y: {sensitivityYSlider.value:F1}";
    }

    void ApplyToPlayer()
    {
        var player = FindObjectOfType<PlayerFPSController>();
        if (player)
        {
            player.mouseSensitivityX = PlayerPrefs.GetFloat(PREF_SENS_X, 1.5f);
            player.mouseSensitivityY = PlayerPrefs.GetFloat(PREF_SENS_Y, 1.5f);
        }
    }
}
