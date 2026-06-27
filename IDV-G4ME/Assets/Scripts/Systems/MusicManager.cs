using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu"; // nom exact de la scène menu

    [Header("Clips")]
    public AudioClip menuClip;            // musique du menu
    public AudioClip[] levelClips;        // playlist pour les niveaux

    [Header("Volumes cibles")]
    [Range(0f, 1f)] public float menuVolume = 0.6f;
    [Range(0f, 1f)] public float levelVolume = 0.12f; // fond très discret en jeu

    [Header("Lecture levels")]
    public bool shuffleLevels = true;
    public float crossfadeSeconds = 1.5f;

    AudioSource a, b;     // deux sources pour gérer le crossfade
    int currentLevelIndex = -1;
    bool muted;
    float targetVolume = 0.6f; // mis à jour selon le contexte (menu/level)

    void Awake()
    {
        // Singleton simple et persistance entre scènes
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Prépare deux sources pour le crossfade
        a = gameObject.AddComponent<AudioSource>();
        b = gameObject.AddComponent<AudioSource>();
        foreach (var s in new[] { a, b })
        {
            s.loop = false;
            s.playOnAwake = false;
            s.spatialBlend = 0f;
            s.volume = 0f;
        }

        // Récupère l'état "mute" sauvegardé
        muted = PlayerPrefs.GetInt("music_muted", 0) == 1;

        // Écoute le chargement de scènes pour adapter la musique
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Choisit le profil audio en fonction de la scène
        if (scene.name == mainMenuSceneName)
        {
            targetVolume = menuVolume;
            if (menuClip != null)
                PlayClip(menuClip, immediateIfSilence: true);
        }
        else
        {
            targetVolume = levelVolume;
            PlayNextLevelTrack(immediateIfSilence: true);
        }
    }

    void Update()
    {
        // Enchaîne la piste suivante quand on est en niveau et que rien ne joue
        if (!IsPlaying(a) && !IsPlaying(b) && !IsInMenu())
            PlayNextLevelTrack();
    }

    bool IsInMenu() => SceneManager.GetActiveScene().name == mainMenuSceneName;
    bool IsPlaying(AudioSource s) => s.isPlaying || s.volume > 0.001f;

    // -------- API publique (pour les UI) --------

    public void ToggleMute()
    {
        SetMute(!muted);
    }

    public void SetMute(bool value)
    {
        muted = value;
        PlayerPrefs.SetInt("music_muted", muted ? 1 : 0);

        // Détermine le volume cible à appliquer
        float target = muted ? 0f : targetVolume;

        // Met à jour directement les volumes des deux sources
        if (a) a.volume = target;
        if (b) b.volume = target;

        // Si on vient de réactiver la musique et qu'aucune piste ne tourne, relance la lecture courante
        if (!muted && !IsPlaying(a) && !IsPlaying(b))
        {
            if (IsInMenu() && menuClip)
                PlayClip(menuClip, immediateIfSilence: true);
            else if (levelClips != null && levelClips.Length > 0)
                PlayNextLevelTrack(immediateIfSilence: true);
        }
    }

    public bool IsMuted => muted;

    // Force la musique du menu (appel manuel si besoin)
    public void ForceMenuMusic()
    {
        targetVolume = menuVolume;
        if (menuClip) PlayClip(menuClip);
    }

    // Force une musique précise pour un niveau
    public void PlayLevelClip(AudioClip clip)
    {
        targetVolume = levelVolume;
        if (clip) PlayClip(clip);
    }

    // Reprend la playlist normale des niveaux
    public void ResumeNormalLevelMusic()
    {
        targetVolume = levelVolume;
        PlayNextLevelTrack(immediateIfSilence: false);
    }

    // Joue une musique de victoire (utilisé par LockedDoor)
    public void PlayVictoryMusic(AudioClip victoryClip)
    {
        if (victoryClip) PlayClip(victoryClip);
    }

    // -------- Lecture / Crossfade --------

    void PlayNextLevelTrack(bool immediateIfSilence = false)
    {
        if (levelClips == null || levelClips.Length == 0) return;

        int next = (shuffleLevels && levelClips.Length > 1)
            ? GetRandomDifferentIndex()
            : (currentLevelIndex + 1) % levelClips.Length;

        currentLevelIndex = next;
        PlayClip(levelClips[currentLevelIndex], immediateIfSilence);
    }

    int GetRandomDifferentIndex()
    {
        int n;
        do { n = Random.Range(0, levelClips.Length); } while (n == currentLevelIndex);
        return n;
    }

    void PlayClip(AudioClip clip, bool immediateIfSilence = false)
    {
        if (!clip) return;

        // Si rien ne joue (ex: première fois), on peut démarrer directement sans fondu
        if (immediateIfSilence && !IsPlaying(a) && !IsPlaying(b))
        {
            var active = a;
            active.clip = clip;
            active.time = 0f;
            active.volume = muted ? 0f : targetVolume;
            active.Play();
            return;
        }

        StartCoroutine(CrossfadeTo(clip, muted ? 0f : targetVolume, crossfadeSeconds));
    }

    IEnumerator CrossfadeTo(AudioClip next, float toVol, float dur)
    {
        AudioSource from = a.isPlaying ? a : b;
        AudioSource to = a.isPlaying ? b : a;

        to.clip = next;
        to.time = 0f;
        to.volume = 0f;
        to.Play();

        float t = 0f;
        dur = Mathf.Max(0.02f, dur);

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = t / dur;
            to.volume = Mathf.Lerp(0f, toVol, k);
            from.volume = Mathf.Lerp(from.volume, 0f, k);
            yield return null;
        }

        from.Stop();
        to.volume = toVol;
    }
}
