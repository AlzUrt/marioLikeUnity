using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI livesText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        UpdateCoinText();
        UpdateLivesText();
    }

    public void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "COINS: " + gameManager.coins.ToString("D2");
        }
    }

    public void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "LIVES: " + gameManager.lives.ToString();
        }
    }
}