using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource audioSource;
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic1; // First gameplay music
    public AudioClip gameplayMusic2; // Second gameplay music
    private bool playFirstSongNext = true; // Flag to determine which song to play next
    public float fadeOutDuration = 1f;
    private bool isFadingOut = false;
    private string previousSceneName = "";
    public bool isHighScoreFromGameOver = false;
    private bool isFirstTimeGameplayMusic = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component not found on " + gameObject.name);
            }
            else
            {
                audioSource.clip = mainMenuMusic; // Set initial music clip
            }
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    void Start()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        // Check if the current scene is gameplay and if the music has finished playing
        if (SceneManager.GetActiveScene().name == "GamePlay" && !audioSource.isPlaying)
        {
            StartCoroutine(SwitchToGameplayMusic());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GamePlay")
        {
            // Ensure the music doesn't loop for gameplay
            audioSource.loop = false;

            // Switch to gameplay music
            if (!isFadingOut)
            {
                StartCoroutine(SwitchToGameplayMusic());
            }
        }
        else if (scene.name == "MainMenu")
        {
            // Ensure the music loops for the main menu
            audioSource.loop = true;

            if (previousSceneName == "GamePlay" || previousSceneName == "GameOver" || (previousSceneName == "HighScores" && isHighScoreFromGameOver))
            {
                StopAllCoroutines();
                StartCoroutine(FadeOutAndSwitchMusic(mainMenuMusic));
                isHighScoreFromGameOver = false;
            }
        }

        // Update the previous scene name
        previousSceneName = scene.name;
    }

    private IEnumerator SwitchToGameplayMusic()
    {
        if (isFirstTimeGameplayMusic)
        {
            // Fade out only the first time switching to gameplay music
            yield return StartCoroutine(FadeOutMusic());
            isFirstTimeGameplayMusic = false; // Set flag to false after first fade out
        }

        // Determine which song to play next
        AudioClip nextSong = playFirstSongNext ? gameplayMusic1 : gameplayMusic2;
        playFirstSongNext = !playFirstSongNext;

        audioSource.clip = nextSong;
        audioSource.Play();
    }

    private IEnumerator FadeOutMusic()
    {
        isFadingOut = true;
        float startVolume = audioSource.volume;

        // Gradually decrease the volume
        for (float t = 0; t < fadeOutDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeOutDuration);
            yield return null;
        }

        audioSource.volume = startVolume;
        isFadingOut = false;
    }

    private IEnumerator FadeOutAndSwitchMusic(AudioClip newClip)
    {
        yield return StartCoroutine(FadeOutMusic());
        audioSource.clip = newClip;
        audioSource.Play();
    }
}
