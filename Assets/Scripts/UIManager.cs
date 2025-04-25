using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI deathCountText;

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        UpdateCoinText();
        UpdateDeathCountText();
        UpdateTimerText();
    }

    public void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "COINS: " + gameManager.coins.ToString("D2");
        }
    }

    public void UpdateDeathCountText()
    {
        if (deathCountText != null)
        {
            deathCountText.text = "DEATHS: " + gameManager.deathCount.ToString();
        }
    }

    public void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = "TIME: " + gameManager.GetFormattedTime();
        }
    }
}