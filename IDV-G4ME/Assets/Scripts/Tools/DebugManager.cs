using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [Header("Debug Mode Settings")]
    [Tooltip("Activé au démarrage")]
    public bool enableDebugAtStart = false;

    public static bool IsDebugEnabled { get; private set; }

    private static DebugManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Charger l'état sauvegardé du debug
        IsDebugEnabled = PlayerPrefs.GetInt("DebugMode", enableDebugAtStart ? 1 : 0) == 1;
    }

    void Start()
    {
        // Si aucune valeur n'a été sauvegardée, utiliser la valeur par défaut
        if (!PlayerPrefs.HasKey("DebugMode"))
        {
            IsDebugEnabled = enableDebugAtStart;
            SaveDebugState();
        }
    }

    void Update()
    {
        // Raccourci clavier pour toggle debug en build
        // Combinaison : Alt + D
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftAlt))
        {
            ToggleDebug();
        }
    }

    public static void ToggleDebug()
    {
        IsDebugEnabled = !IsDebugEnabled;
        if (instance != null)
        {
            instance.SaveDebugState();
        }

        string status = IsDebugEnabled ? "ACTIVÉ" : "DÉSACTIVÉ";
        Debug.Log($"[DEBUG MODE] {status} - IsDebugEnabled: {IsDebugEnabled}");
    }

    public static void SetDebugMode(bool enabled)
    {
        IsDebugEnabled = enabled;
        if (instance != null)
        {
            instance.SaveDebugState();
        }

        Debug.Log($"[DEBUG MODE] Mis à {(enabled ? "ACTIVÉ" : "DÉSACTIVÉ")} - IsDebugEnabled: {IsDebugEnabled}");
    }

    private void SaveDebugState()
    {
        PlayerPrefs.SetInt("DebugMode", IsDebugEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ResetDebugState()
    {
        PlayerPrefs.DeleteKey("DebugMode");
        IsDebugEnabled = false;
        Debug.Log("[DEBUG MODE] État réinitialisé");
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
