using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    [SerializeField]
    bool debug;
    //[SerializeField]
    int maxLeaderboardSlots = 5;

    // Timer
    Coroutine timerCoroutine;

    // Leaderboard
    List<Score> leaderboard;

    // PlayerPrefs keys
    static readonly string ROUND_KEY = "round_";
    static readonly string DATETIME_KEY = "dateTime_";

    // DateTime parsing method
    const string FMT = "O";

    // Music
    AudioSource gameMusic;
    AudioSource leaderboardMusic;
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
    }
    #endregion

    #region Methods
    void InitializeVariables()
    {
        leaderboard = LoadScores();
    }

    void SubscribeToEvents()
    {
        CoinGenerator.Instance.OnCollectedCoin += OnCollectedCoin;
        CoinGenerator.Instance.OnCollectedAll += OnCollectedAllCoins;
    }

    IEnumerator StartRound()
    {
        // Set timer
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timer = maxTime - currentRound;
        UIManager.Instance.gameUIController.SetSliderMaxValue(timer);
        UIManager.Instance.gameUIController.SetTimer(timer);

        // Set chance for max coins
        CoinGenerator.Instance.AddValueToChanceForMaxCoins(currentRound);

        // Set Round
        currentRound++;
        UIManager.Instance.gameUIController.SetRoundText(currentRound);

        // Generate Coins
        CoinGenerator.Instance.GenerateCoins();

        // Set Coins
        UIManager.Instance.gameUIController.ResetCoinsText();
        UIManager.Instance.gameUIController.SetTotalCoinsText(CoinGenerator.Instance.GetCoins());

        // Display graphics: Ready
        UIManager.Instance.gameUIController.DisplayReady();

        // Play Sound
        AudioManager.Instance.PlaySound("ClockTick");

        yield return new WaitForSeconds(1f);

        // Display graphics: Set
        UIManager.Instance.gameUIController.DisplaySet();

        // Play Sound
        AudioManager.Instance.PlaySound("ClockTick");

        yield return new WaitForSeconds(1f);

        // Display graphics: Go
        UIManager.Instance.gameUIController.DisplayGo();

        // Play Sounds
        AudioManager.Instance.PlaySound("ClockTick2");
        AudioManager.Instance.PlaySound("Bell");

        // Make all coins interactable
        CoinGenerator.Instance.SetAllCoinsInteractable();

        // Start timer
        timerCoroutine = StartCoroutine(Timer());

        yield return new WaitForSeconds(1);

        // Display graphics: Nothing
        UIManager.Instance.gameUIController.DisplayNothing();
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

        // Set timer
        timer--;
        UIManager.Instance.gameUIController.SetTimer(timer);

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
        // Stop music
        if (gameMusic != null)
            gameMusic.Stop();

        // Play Sound
        AudioManager.Instance.PlaySound("GameOver");

        // Play Music
        if (leaderboardMusic == null)
            leaderboardMusic = AudioManager.Instance.PlayMusic("LeaderboardMusic");
        else
            leaderboardMusic.Play();

        // Game Over Graphic
        UIManager.Instance.gameUIController.DisplayGameOver();

        // Check and save new score
        if (leaderboard != null)
            if (leaderboard.Count < maxLeaderboardSlots ||
                leaderboard[leaderboard.Count - 1].round < currentRound)
            {
                SaveScore(new Score(currentRound, DateTime.Now));
            }   

        // Stop timer
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        // Remove remaining coins
        CoinGenerator.Instance.RemoveAllCoins();

        yield return new WaitForSeconds(1);

        // Display Leaderboard
        UIManager.Instance.gameUIController.DisplayLeaderboard(leaderboard);
    }

    public void RestartGame()
    {
        // Clear Game
        ClearGame();

        StartGame();
    }

    IEnumerator WinRound()
    {
        // Display graphics: You Win!
        UIManager.Instance.gameUIController.DisplayWin();

        // Play Sound
        AudioManager.Instance.PlaySound("RoundWon");

        yield return new WaitForSeconds(1);

        // Start next round
        StartCoroutine(StartRound());
    }

    void OnCollectedCoin()
    {
        // Set Coins
        UIManager.Instance.gameUIController.SetCoinsText(CoinGenerator.Instance.GetCoins());
    }

    void OnCollectedAllCoins()
    {
        // Stop timer
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);

        StartCoroutine(WinRound());
    }

    public void StartGame()
    {
        StartCoroutine(StartGameEnumerator());
    }

    IEnumerator StartGameEnumerator()
    {
        // Play Music
        if (gameMusic == null)
            gameMusic = AudioManager.Instance.PlayMusic("GameMusic");
        else
        {
            // Play Music From the beginning
            gameMusic.Stop();
            gameMusic.Play();
        }

        // Set current round
        currentRound = 0;

        // Reset chance for max coins
        CoinGenerator.Instance.ResetChanceForMaxCoins();

        yield return new WaitForSeconds(0);

        // Start First Round
        StartCoroutine(StartRound());
    }
    
    void SaveScore(Score _score)
    {
        // Add new score to leaderboard list
        leaderboard.Add(_score);

        // Sort the list
        if (leaderboard.Count > 1)
            leaderboard = leaderboard.OrderByDescending(s => s.round).ToList();

        // Save all scores
        for (int i = 0; i < maxLeaderboardSlots; i++)
        {
            if (i < leaderboard.Count)
            {
                PlayerPrefs.SetInt(ROUND_KEY + i.ToString(), leaderboard[i].round);
                PlayerPrefs.SetString(DATETIME_KEY + i.ToString(), leaderboard[i].dateTime.ToString(FMT));
            }
            else
                break;
        }

        // Debug
        if (debug)
            PrintScores();
    }

    Score LoadScore(int _index)
    {
        // Get values from playerPrefs
        int round = PlayerPrefs.GetInt(ROUND_KEY + _index.ToString());
        string dateTimeString = PlayerPrefs.GetString(DATETIME_KEY + _index.ToString());
        DateTime dateTime = DateTime.Now;
        if (!string.IsNullOrEmpty(dateTimeString))
            dateTime = DateTime.ParseExact(dateTimeString, FMT, CultureInfo.InvariantCulture);

        // Check if round is 0 then the slot is empty
        if (round != 0)
        {
            Score score = new Score(round, dateTime);
            return score;
        }
        else
            return null;
    }

    List<Score> LoadScores()
    {
        // Initialize loadedScores list
        List<Score> loadedScores = new List<Score>();

        // Load scores and add them to loadedScores list
        for (int i = 0; i < maxLeaderboardSlots; i++)
        {
            Score score = LoadScore(i);
            if (score != null)
                loadedScores.Add(score);
        }

        return loadedScores;
    }

    void PrintScores()
    {
        List<Score> loadedScores = LoadScores();

        if (loadedScores != null)
        {
            for (int i = 0; i < loadedScores.Count; i++)
            {
                Debug.Log(string.Format("score {0} : round = {1}, dateTime = {2}", i, loadedScores[i].round, loadedScores[i].dateTime));
            }
        }
    }

    public void PauseGame(bool _pause)
    {
        if (_pause)
        {
            // Pause
            Time.timeScale = 0;

            // Pause music
            if (gameMusic != null)
                gameMusic.Pause();

            // Play Music
            if (leaderboardMusic == null)
                leaderboardMusic = AudioManager.Instance.PlayMusic("LeaderboardMusic");
            else
                leaderboardMusic.Play();

            // Display Leaderboard
            UIManager.Instance.gameUIController.DisplayLeaderboard(leaderboard);
        }
        else
        {
            // Unause
            Time.timeScale = 1;

            // Stop music
            if (leaderboardMusic != null)
                leaderboardMusic.Stop();

            // Play music
            if (gameMusic != null)
                gameMusic.Play();
        }
    }

    public void ClearGame()
    {
        // Stop All Coroutines
        StopAllCoroutines();

        // Stop music
        if (leaderboardMusic != null)
            leaderboardMusic.Stop();

        // Reset Game UI
        UIManager.Instance.gameUIController.OpenGameUI();

        // Remove remaining coins
        CoinGenerator.Instance.RemoveAllCoins();

        // Unause
        Time.timeScale = 1;
    }
    #endregion
}
