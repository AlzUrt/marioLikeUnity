using UnityEngine;
using UnityEngine.SceneManagement;
using System;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; } = 1;
    public int stage { get; private set; } = 1;
    public int coins { get; private set; } = 0;
    public int deathCount { get; private set; } = 0;

    // Variables pour le timer
    public float currentTime { get; private set; } = 0f;
    private bool isTimerRunning = false;

    // Reference to our UIManager
    private UIManager uiManager;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        NewGame();
    }

    private void Update()
    {
        // Mettre à jour le timer s'il est en cours d'exécution
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;

            // Mettre à jour l'UI toutes les secondes
            if (Mathf.FloorToInt(currentTime) != Mathf.FloorToInt(currentTime - Time.deltaTime))
            {
                if (uiManager != null)
                {
                    uiManager.UpdateTimerText();
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find UIManager in the newly loaded scene
        uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateCoinText();
            uiManager.UpdateDeathCountText();
            uiManager.UpdateTimerText();
        }

        // Démarrer le timer quand une nouvelle scène est chargée
        StartTimer();
    }

    public void NewGame()
    {
        coins = 0;
        // Ne pas réinitialiser le compteur de morts pour conserver le nombre total
        ResetTimer();
        LoadLevel(1, 1);
    }

    public void GameOver()
    {
        NewGame();
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        LoadLevel(world, stage + 1);
    }

    public void ResetLevel(float delay)
    {
        CancelInvoke(nameof(ResetLevel));
        Invoke(nameof(ResetLevel), delay);
    }

    public void ResetLevel()
    {
        // Incrémenter le compteur de morts
        deathCount++;

        // Réinitialiser le timer
        ResetTimer();

        // Mettre à jour l'UI
        if (uiManager != null)
        {
            uiManager.UpdateDeathCountText();
            uiManager.UpdateTimerText();
        }

        // Avec une seule vie, reset direct au nouveau jeu
        GameOver();
    }

    public void AddCoin()
    {
        coins++;

        // Update UI when coins change
        if (uiManager != null)
        {
            uiManager.UpdateCoinText();
        }

        if (coins == 100)
        {
            coins = 0;
            // Pas d'ajout de vie, mais mise à jour de l'UI
            if (uiManager != null)
            {
                uiManager.UpdateCoinText();
            }
        }
    }

    // Méthodes pour gérer le timer
    public void StartTimer()
    {
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = 0f;
        if (uiManager != null)
        {
            uiManager.UpdateTimerText();
        }
    }

    // Méthode pour obtenir le temps formaté (MM:SS)
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}