using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class XpManager : MonoBehaviour
{
    public static XpManager Instance;

    [Header("UI References")]
    public Slider xpBar;
    public TextMeshProUGUI timerText;
    public GameObject powerUpSelectionPanel;
    public Transform powerUpButtonContainer;
    public GameObject powerUpButtonPrefab;

    public float survivalTimer = 0f;
    public float globalTimer = 0f;
    private const float timeToLevelUp = 10f; // 5 seconds to level up
    private bool firstLevelUp = true;
    private bool isPlayerAlive = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        xpBar.maxValue = timeToLevelUp;
        xpBar.value = 0;
        powerUpSelectionPanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPlayerAlive) return;

        survivalTimer += Time.deltaTime;
        globalTimer += Time.deltaTime;
        xpBar.value = survivalTimer;

        timerText.text = Mathf.RoundToInt(globalTimer) + "s";

        if (survivalTimer >= timeToLevelUp)
        {
            survivalTimer = 0f;
            LevelUp();
        }
    }

    public void SetPlayerAlive(bool alive)
    {
        isPlayerAlive = alive;
        if (!alive)
        {
            survivalTimer = 0f;
            xpBar.value = 0f;
        }
    }

    private void LevelUp()
    {
        Time.timeScale = 0; // Pause game

        // Clear previous power-up buttons
        foreach (Transform child in powerUpButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Get power-up options
        List<PowerUp> options;

        if (firstLevelUp)
        {
            // Only show basic shooting first time
            options = new List<PowerUp> { PowerUpManager.Instance.allPowerUps[0] };
            firstLevelUp = false;
        }
        else
        {
            // Get 3 random power-ups
            options = PowerUpManager.Instance.GetRandomPowerUps(3);
        }

        // Create buttons for each option with specific positions
        float[] xPositions = { 0f, 193f, -193f };
        for (int i = 0; i < options.Count; i++)
        {
            var buttonObj = Instantiate(powerUpButtonPrefab, powerUpButtonContainer);

            // Set position based on index
            RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(xPositions[i], rectTransform.anchoredPosition.y);

            var button = buttonObj.GetComponent<PowerUpButton>();
            button.Setup(options[i]);
        }

        powerUpSelectionPanel.SetActive(true);
    }

    public void SelectPowerUp(PowerUp powerUp)
    {
        // Apply power-up effect
        var player = FindObjectOfType<Player>();
        powerUp.applyEffect(player);

        // Remove power-up from available list if it is rare
        if (powerUp.rarity == "Rare")
        {
            PowerUpManager.Instance.allPowerUps.Remove(powerUp);
        }

        // Resume game
        powerUpSelectionPanel.SetActive(false);
        Time.timeScale = 1;
    }
}