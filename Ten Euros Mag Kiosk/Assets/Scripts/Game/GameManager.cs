using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Variables
    [SerializeField]
    int currentRound;
    [SerializeField]
    int maxTime = 30;
    [SerializeField]
    int timer;

    Coroutine timerCoroutine;
    #endregion

    #region Unity Callbacks
    protected override void Awake()
    {
        base.Awake();

        InitializeVariables();
    }

    void Start()
    {
        SubscribeToEvents();

        // Start Game
        StartCoroutine(StartGame());
    }

    void Update()
    {
        
    }
    #endregion

    #region Methods
    void InitializeVariables()
    {
        currentRound = 0;
    }

    void SubscribeToEvents()
    {
        CoinGenerator.Instance.OnCollectedAll += OnCollectedAllCoins;
    }

    IEnumerator StartRound()
    {
        // Set timer
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timer = maxTime - currentRound;

        // Set chance for max coins
        CoinGenerator.Instance.AddValueToChanceForMaxCoins(currentRound);

        // Set Round
        currentRound++;

        // Generate Coins
        CoinGenerator.Instance.GenerateCoins();

        // Play graphics: Round #


        yield return new WaitForSeconds(2);

        // Play graphics: Go!

        // Start timer
        timerCoroutine = StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

        timer--;

        // Check timer
        if (timer <= 0)
        {
            // Game over
            StartCoroutine(GameOver());
        }
        else
        {
            timerCoroutine = StartCoroutine(Timer());
        }
    }

    IEnumerator GameOver()
    {
        // Game Over Graphic
        Debug.Log("GAME OVER");

        // Set timer
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        // Remove remaining coins
        CoinGenerator.Instance.RemoveAllCoins();

        yield return new WaitForSeconds(1);

        // Restart game
        StartCoroutine(StartGame());
    }

    IEnumerator WinRound()
    {
        yield return new WaitForSeconds(1);

        StartCoroutine(StartRound());
    }

    void OnCollectedAllCoins()
    {
        StartCoroutine(WinRound());
    }

    IEnumerator StartGame()
    {
        // Reset chance for max coins
        CoinGenerator.Instance.ResetChanceForMaxCoins();

        yield return new WaitForSeconds(1);

        // Start First Round
        StartCoroutine(StartRound());
    }
    
    #endregion
}
