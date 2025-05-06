using UnityEngine;
using TMPro;
using Dan.Main;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private GameObject entryPrefab; // Préfab pour chaque entrée
    [SerializeField] private Transform contentParent; // Le transform "Content"
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private string leaderboardPublicKey = "bc202dffd55a3e3e6cc4ecb4f5b10cab2f0eada7ff38aaa81f7566c48e49f86a";
    
    private GameManager gameManager;
    private List<GameObject> entryObjects = new List<GameObject>();

    private void Awake()
    {
        gameManager = GameManager.Instance;
        
        // Vérification des références
        if (leaderboardPanel == null) Debug.LogError("LeaderboardPanel est null!");
        if (entryPrefab == null) Debug.LogError("EntryPrefab est null!");
        if (contentParent == null) Debug.LogError("ContentParent est null!");
        if (usernameInputField == null) Debug.LogError("UsernameInputField est null!");
        
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    public void ShowLeaderboard()
    {
        Debug.Log("ShowLeaderboard appelé");
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            LoadEntries();
        }
        else
        {
            Debug.LogError("LeaderboardPanel est null dans ShowLeaderboard!");
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
        Debug.Log("LoadEntries appelé, clé: " + leaderboardPublicKey);
        
        // Nettoyer les entrées précédentes
        ClearEntries();
        
        // Vérifier si le SDK est bien initialisé
        if (string.IsNullOrEmpty(leaderboardPublicKey))
        {
            Debug.LogError("Clé publique du leaderboard non définie!");
            return;
        }
        
        // Utiliser la méthode GetLeaderboard avec la clé publique
        LeaderboardCreator.GetLeaderboard(leaderboardPublicKey, entries =>
        {
            Debug.Log($"Réponse reçue: {entries.Length} entrées trouvées");
            
            // Créer une entrée visuelle pour chaque entrée de données
            for (int i = 0; i < entries.Length; i++)
            {
                CreateLeaderboardEntry(entries[i]);
            }
        }, error =>
        {
            Debug.LogError($"Erreur lors du chargement du leaderboard: {error}");
        });
    }
    
    private void ClearEntries()
    {
        Debug.Log($"Nettoyage de {entryObjects.Count} entrées existantes");
        
        // Détruire toutes les entrées existantes
        foreach (var entryObject in entryObjects)
        {
            Destroy(entryObject);
        }
        entryObjects.Clear();
    }
    
    private void CreateLeaderboardEntry(Dan.Models.Entry entry)
    {
        Debug.Log($"Création d'une entrée: Rang {entry.Rank}, Nom {entry.Username}, Score {entry.Score}");
        
        if (entryPrefab == null)
        {
            Debug.LogError("EntryPrefab est null lors de la création d'entrée!");
            return;
        }
        
        if (contentParent == null)
        {
            Debug.LogError("ContentParent est null lors de la création d'entrée!");
            return;
        }
        
        // Instantier le préfab d'entrée
        GameObject entryObject = Instantiate(entryPrefab, contentParent);
        entryObjects.Add(entryObject);
        
        // Configurer le texte de l'entrée
        TMP_Text entryText = entryObject.GetComponentInChildren<TMP_Text>();
        if (entryText != null)
        {
            entryText.text = $"{entry.Rank}. {entry.Username} - {entry.Score}";
        }
        else
        {
            Debug.LogError("Pas de composant TMP_Text trouvé dans l'entrée!");
        }
    }

    public void UploadEntry()
    {
        if (string.IsNullOrEmpty(usernameInputField.text))
        {
            Debug.Log("Nom d'utilisateur vide!");
            return;
        }

        // Calcul du score basé sur le temps et le nombre de pièces
        int score = CalculateScore();
        Debug.Log($"Envoi du score: {score} pour l'utilisateur: {usernameInputField.text}");

        LeaderboardCreator.UploadNewEntry(leaderboardPublicKey, usernameInputField.text, score, isSuccessful =>
        {
            if (isSuccessful)
            {
                Debug.Log("Score envoyé avec succès!");
                LoadEntries();
            }
            else
            {
                Debug.LogError("Erreur lors de l'envoi du score");
            }
        });
    }

    private int CalculateScore()
    {
        // Exemple de calcul de score basé sur le temps de jeu et les pièces collectées
        float timePenalty = gameManager.currentTime;
        int coinBonus = gameManager.coins * 100;

        // Score inversé pour que moins de temps = meilleur score (10000 est une valeur arbitraire)
        int timeScore = Mathf.Max(0, 10000 - Mathf.FloorToInt(timePenalty * 10));
        
        int finalScore = timeScore + coinBonus;
        Debug.Log($"Calcul du score: Time: {timePenalty}, TimeScore: {timeScore}, Coins: {gameManager.coins}, CoinBonus: {coinBonus}, Score Final: {finalScore}");

        return finalScore;
    }

    public void RestartLevel()
    {
        Debug.Log("Restart appelé");
        HideLeaderboard();
        gameManager.ResetLevel();
    }
}