using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI deathCountText;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    private void Start()
    {
        UpdateCoinText();
        UpdateDeathCountText();
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
}