using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int world { get; private set; } = 1;
    public int stage { get; private set; } = 1;
    public int coins { get; private set; } = 0;
    public int deathCount { get; private set; } = 0;

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
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateCoinText();
            uiManager.UpdateDeathCountText();
        }
    }

    public void NewGame()
    {
        coins = 0;
        // Ne pas réinitialiser le compteur de morts pour conserver le nombre total
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

        // Mettre à jour l'UI
        if (uiManager != null)
        {
            uiManager.UpdateDeathCountText();
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
}