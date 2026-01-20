using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum GameState
{
    Idle,           
    RoundActive,    
    RoundWon,      
    RoundLost       
}

public class GameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameConfig gameConfig;   
    public FSM<GameManager, GameState> gameStateMachine;
    private List<Chest> chests = new List<Chest>();
    private int remainingAttempts;
    private Chest currentlyOpeningChest;
    private Reward currentRoundReward;

    private CurrencyManager rewardManager;
    public event Action<int> OnAttemptsChanged;
    public event Action<Reward> OnRoundWon;
    public event Action OnRoundLost;
    public GameState CurrentGameState => gameStateMachine.CurrentState;
    public int RemainingAttempts => remainingAttempts;
    public int NumberOfChests => gameConfig.numberOfChests;
    public List<Chest> Chests => chests;

    void OnValidate()
    {
        Assert.IsNotNull(gameConfig, "GameConfig reference is missing in GameManager");
    }

    void Awake()
    {
        ServiceLocator.RegisterService<GameManager>(this);

        rewardManager = new CurrencyManager();
        ServiceLocator.RegisterService<CurrencyManager>(rewardManager); 

        gameStateMachine = new FSM<GameManager, GameState>(this);
        gameStateMachine.AddState(GameState.Idle, null);
        gameStateMachine.AddState(GameState.RoundActive, null);
        gameStateMachine.AddState(GameState.RoundWon, null);    
        gameStateMachine.AddState(GameState.RoundLost, null);   
    }

    void Start()
    {         
       gameStateMachine.ChangeState(GameState.Idle);
    }

    void OnDestroy()
    {
        foreach (var chest in chests)
        {
            chest.CancelOpening();
        }
    }

    public void StartNewRound()
    {
        // if (CurrentGameState != GameState.Idle)
        // {
        //     Debug.LogWarning("Cannot start a new round - game is not idle");
        //     return;
        // }

        currentRoundReward = rewardManager.GenerateRandomReward(
            gameConfig.minRewardAmount, 
            gameConfig.maxRewardAmount
        );
 
        InitializeChests();

        remainingAttempts = gameConfig.maxAttempts;
        OnAttemptsChanged?.Invoke(remainingAttempts);

        gameStateMachine.ChangeState(GameState.RoundActive);
    }
   
    public async void OnChestSelected(int chestIndex)
    {
        if (CurrentGameState != GameState.RoundActive)
        {
            return;
        }

        Chest selectedChest = chests[chestIndex];

        if (selectedChest.StateMachine.CurrentState != ChestState.Closed)
        {
            return;
        }

        if (currentlyOpeningChest != null && currentlyOpeningChest != selectedChest)
        {
            Debug.Log($"Cancelling chest {currentlyOpeningChest.Index} to open chest {chestIndex}");
            currentlyOpeningChest.CancelOpening();
        }

        currentlyOpeningChest = selectedChest;

        bool openingCompleted = await selectedChest.OpenAsync(gameConfig.chestOpeningDuration);

        if (!openingCompleted)
        {
            currentlyOpeningChest = null;
            return;
        }

        currentlyOpeningChest = null;

        if (selectedChest.IsWinning)
        {
            HandleRoundWin();
        }
        else
        {
            HandleChestFailure();
        }
    }   
  
    private void InitializeChests()
    {
        chests.Clear();

        int winningChestIndex = UnityEngine.Random.Range(0, gameConfig.numberOfChests);

        for (int i = 0; i < gameConfig.numberOfChests; i++)
        {
            bool isWinning = (i == winningChestIndex);
            Chest chest = new Chest(i, isWinning);           

            chests.Add(chest);

        }

        Debug.Log($"Initialized {gameConfig.numberOfChests} chests. Winning chest: {winningChestIndex}");
    }

    public Chest GetChestAtIndex(int index)
    {
        return chests[index];
    }


    private void HandleChestFailure()
    {
        remainingAttempts--;
        OnAttemptsChanged?.Invoke(remainingAttempts);

        Debug.Log($"Chest failed! Remaining attempts: {remainingAttempts}");

        if (remainingAttempts <= 0)
        {
            HandleRoundLoss();
        }
    }
    private void HandleRoundWin()
    {
        
        Debug.Log($"Round won! Reward: {currentRoundReward}");

        rewardManager.AddReward(currentRoundReward);

        OnRoundWon?.Invoke(currentRoundReward);

        gameStateMachine.ChangeState(GameState.RoundWon);
    }

    private void HandleRoundLoss()
    {
        Debug.Log("Round lost - out of attempts!");

        OnRoundLost?.Invoke();

        gameStateMachine.ChangeState(GameState.RoundLost);
    }
}
