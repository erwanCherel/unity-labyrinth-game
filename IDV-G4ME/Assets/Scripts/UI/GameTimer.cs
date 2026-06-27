using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    public bool Running { get; private set; } = true; // indique si le timer tourne
    public float Elapsed { get; private set; } = 0f;  // temps écoulé depuis le début

    void Awake()
    {
        // Singleton simple : garde une seule instance à travers les scènes
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Incrémente le temps uniquement si le timer est actif
        if (Running) Elapsed += Time.deltaTime;
    }

    public void StartTimer() => Running = true;   // relance le timer
    public void StopTimer() => Running = false;   // met le timer en pause
    public void ResetTimer() { Elapsed = 0f; }    // remet le compteur à zéro
}
