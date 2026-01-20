using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;

public class GameUI : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI gameStateText;

    [Header("Reward Displays")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI gemsText;

    [Header("Game Info")]
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private TextMeshProUGUI roundResultText;

    [Header("Chest Container")]
    [SerializeField] private Transform chestContainer;
    [SerializeField] private GameObject chestPrefab;

    private GameManager gameManager;
    private CurrencyManager rewardManager;
    private List<ChestUI> chestUIList = new List<ChestUI>();

    void OnValidate()
    {
        Assert.IsNotNull(startGameButton, "StartGameButton reference is missing in GameUI");
        Assert.IsNotNull(gameStateText, "GameStateText reference is missing in GameUI");
        Assert.IsNotNull(coinsText, "CoinsText reference is missing in GameUI");
        Assert.IsNotNull(gemsText, "GemsText reference is missing in GameUI");
        Assert.IsNotNull(attemptsText, "AttemptsText reference is missing in GameUI");
        Assert.IsNotNull(roundResultText, "RoundResultText reference is missing in GameUI");
        Assert.IsNotNull(chestContainer, "ChestContainer reference is missing in GameUI");
        Assert.IsNotNull(chestPrefab, "ChestPrefab reference is missing in GameUI");

    }
    

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        gameManager = ServiceLocator.GetService<GameManager>();
        rewardManager = ServiceLocator.GetService<CurrencyManager>();

        rewardManager.OnCurrencyChanged += HandleRewardChanged;

        startGameButton.onClick.AddListener(OnStartGameClicked);        


        if (gameManager != null)
        {
            gameManager.gameStateMachine.OnStateChanged += HandleGameStateChanged;
            gameManager.OnAttemptsChanged += HandleAttemptsChanged;
            gameManager.OnRoundWon += HandleRoundWon;
            gameManager.OnRoundLost += HandleRoundLost;
        }

        UpdateGameStateDisplay(GameState.Idle);
        if (roundResultText != null)
        {
            roundResultText.gameObject.SetActive(false);
        }
    }


    void OnDestroy()
    {       
        if (gameManager != null)
        {
            gameManager.gameStateMachine.OnStateChanged -= HandleGameStateChanged;
            gameManager.OnAttemptsChanged -= HandleAttemptsChanged;
            gameManager.OnRoundWon -= HandleRoundWon;
            gameManager.OnRoundLost -= HandleRoundLost;
        }
        
        startGameButton.onClick.RemoveListener(OnStartGameClicked);
        
    }

    private void OnStartGameClicked()
    {
        if (gameManager != null)
        {
            if (gameManager.CurrentGameState == GameState.Idle || 
                gameManager.CurrentGameState == GameState.RoundWon || 
                gameManager.CurrentGameState == GameState.RoundLost)
            {
                gameManager.StartNewRound();
            }           
        }
    }

    private void HandleGameStateChanged(GameManager gameManager, GameState newState)
    {
        UpdateGameStateDisplay(newState);

        if (newState == GameState.RoundActive)
        {
            CreateChestUI();

            roundResultText.gameObject.SetActive(false);            
        }
    }

    private void HandleAttemptsChanged(int remainingAttempts)
    {
        attemptsText.text = $"Attempts: {remainingAttempts}";
    }

    private void HandleRoundWon(Reward reward)
    {

            roundResultText.text = $"YOU WIN!\nReward: {reward}";
            roundResultText.color = Color.green;
            roundResultText.gameObject.SetActive(true);
        
    }

    private void HandleRoundLost()
    {

            roundResultText.text = "YOU LOSE!\nOut of attempts";
            roundResultText.color = Color.red;
            roundResultText.gameObject.SetActive(true);
        
    }
    private void HandleRewardChanged(CurrencyType type, int amount)
    {
        UpdateRewardUI();
    }

    private void UpdateRewardUI()
    {
        UpdateRewardDisplay(CurrencyType.Coins, rewardManager.GetCurrencyAmount(CurrencyType.Coins));
        UpdateRewardDisplay(CurrencyType.Gems, rewardManager.GetCurrencyAmount(CurrencyType.Gems));
    }

    private void UpdateGameStateDisplay(GameState state)
    {

            gameStateText.text = state switch
            {
                GameState.Idle => "Ready to Start",
                GameState.RoundActive => "Round Active - Pick a Chest!",
                GameState.RoundWon => "Round Won!",
                GameState.RoundLost => "Round Lost",
                _ => ""
            };
            
            startGameButton.interactable = (state == GameState.Idle || state == GameState.RoundWon || state == GameState.RoundLost);
    }

    private void CreateChestUI()
    {        
        foreach (var chestUI in chestUIList)
        {
            Destroy(chestUI.gameObject);            
        }
        chestUIList.Clear();
       
        for (int i = 0; i < gameManager.NumberOfChests; i++)
        {
            GameObject chestObj = Instantiate(chestPrefab, chestContainer);
            ChestUI chestUI = chestObj.GetComponent<ChestUI>();
            
            chestUI.Initialize(i, gameManager);
            chestUIList.Add(chestUI);            
        }
    }

    public void UpdateRewardDisplay(CurrencyType type, int amount)
    {
        switch (type)
        {
            case CurrencyType.Coins:
                    coinsText.text = $"Coins: {amount}";
                break;
            case CurrencyType.Gems:
                    gemsText.text = $"Gems: {amount}";
                break;
        }
    }
}
