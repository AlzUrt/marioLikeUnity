using UnityEngine;
using TMPro;
using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private string leaderboardPublicKey = "bc202dffd55a3e3e6cc4ecb4f5b10cab2f0eada7ff38aaa81f7566c48e49f86a";

    private GameManager gameManager;
    private List<GameObject> entryObjects = new List<GameObject>();

    private void Awake()
    {
        gameManager = GameManager.Instance;

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(UploadEntry);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartLevel);
        }
    }

    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            LoadEntries();
        }
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    private void LoadEntries()
    {
        ClearEntries();

        if (string.IsNullOrEmpty(leaderboardPublicKey))
        {
            return;
        }

        LeaderboardCreator.GetLeaderboard(leaderboardPublicKey, entries =>
        {
            for (int i = 0; i < entries.Length; i++)
            {
                CreateLeaderboardEntry(entries[i]);
            }
        }, error => { });
    }

    private void ClearEntries()
    {
        foreach (var entryObject in entryObjects)
        {
            Destroy(entryObject);
        }
        entryObjects.Clear();
    }

    private void CreateLeaderboardEntry(Dan.Models.Entry entry)
    {
        if (entryPrefab == null || contentParent == null)
        {
            return;
        }

        GameObject entryObject = Instantiate(entryPrefab, contentParent);
        entryObjects.Add(entryObject);

        TMP_Text entryText = entryObject.GetComponentInChildren<TMP_Text>();
        if (entryText != null)
        {
            // Convertir le score inversé en temps réel
            int maxScore = 1000000000;
            float seconds = (maxScore - entry.Score) / 1000f;
            string formattedTime = FormatTimeWithMilliseconds(seconds);
            entryText.text = $"{entry.Rank}. {entry.Username} - {formattedTime}";
        }
    }

    private string FormatTimeWithMilliseconds(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        int milliseconds = Mathf.FloorToInt((seconds * 1000) % 1000);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, secs, milliseconds);
    }

    public void UploadEntry()
    {
        if (string.IsNullOrEmpty(usernameInputField.text))
        {
            return;
        }

        int score = CalculateTimeScore();

        Button submitButton = usernameInputField.transform.parent.GetComponentInChildren<Button>();
        if (submitButton != null) submitButton.interactable = false;

        LeaderboardCreator.UploadNewEntry(leaderboardPublicKey, usernameInputField.text, score, isSuccessful =>
        {
            if (submitButton != null) submitButton.interactable = true;

            if (isSuccessful)
            {
                StartCoroutine(DelayedLeaderboardRefresh());
            }
        });
    }

    private IEnumerator DelayedLeaderboardRefresh()
    {
        yield return new WaitForSeconds(1f);

        if (usernameInputField != null)
        {
            usernameInputField.text = "";
        }

        LoadEntries();
    }

    private int CalculateTimeScore()
    {
        // Pour un leaderboard où les scores les plus bas sont les meilleurs,
        // on utilise une valeur élevée et on soustrait le temps
        // 1000000000 représente ~2h45m de jeu max en millisecondes
        int maxScore = 1000000000;
        int timeScore = maxScore - Mathf.RoundToInt(gameManager.currentTime * 1000);
        return timeScore;
    }

    public void RestartLevel()
    {
        HideLeaderboard();

        if (gameManager != null)
        {
            gameManager.ResetLevel();
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetLevel();
        }
    }
}