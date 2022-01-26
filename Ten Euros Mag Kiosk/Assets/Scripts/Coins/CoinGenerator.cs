using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinGenerator : Singleton<CoinGenerator>
{
    #region Variables
    [Header("Customize Values")]
    [SerializeField]
    [Range(1, 1000000)]
    [Tooltip("Maximum number of cents.")]
    int maxCents = 1000;
    [SerializeField]
    [Range(1, 100)]
    [Tooltip("Maximum number of coins.")]
    int maxCoins = 20;
    [SerializeField]
    [Range(0, 100)]
    [Tooltip("Chance to generate maximum number of coins. \nThe higher the number, the more coins are generated.")]
    int chanceForMaxCoins = 6;
    [SerializeField]
    bool debug;

    static readonly int[] centAmounts = { 1, 2, 5, 10, 20, 50, 100, 200 };

    List<Coin> coins;
    int startChanceForMaxCoins;

    [Space(25)]
    [SerializeField]
    Coin coinPrefab;
    // Current generated coins parent's transform
    [SerializeField]
    Transform coinsParent;
    // The world position a coin is instantiated in
    [SerializeField]
    Vector3 startPosition;
    // All coin data
    [SerializeField]
    CoinData[] coinData;

    // Random Position
    static readonly float minRangeX = -4f;
    static readonly float maxRangeX = 4f;
    static readonly float minRangeY = -4.5f;
    static readonly float maxRangeY = 0f;

    // Random Duration
    static readonly float minDuration = 1f;
    static readonly float maxDuration = 2f;

    // Events
    public delegate void CollectedAll();
    public event CollectedAll OnCollectedAll;
    #endregion

    #region Unity Callbacks
    protected override void Awake()
    {
        base.Awake();

        startChanceForMaxCoins = chanceForMaxCoins;
    }
    #endregion

    #region Methods
    public void GenerateCoins()
    {
        // Generate cent amounts
        List<int> generatedCentAmounts = GenerateCentAmounts();

        // Reset coin order in layer
        if (Coin.orderInLayer != 0)
            Coin.orderInLayer = 0;

        // Initialize coins list
        coins = new List<Coin>();

        // Instantiate coins at start position
        for (int i = 0; i < generatedCentAmounts.Count; i++)
        {
            Coin coin = Instantiate(coinPrefab, startPosition, Quaternion.identity, coinsParent);
            coins.Add(coin);
            int centAmount = generatedCentAmounts[i];
            coin.SetupCoin(coinData[Array.IndexOf(centAmounts, centAmount)]);
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(minRangeX, maxRangeX), UnityEngine.Random.Range(minRangeY, maxRangeY), 0f);
            float randomDuration = UnityEngine.Random.Range(minDuration, maxDuration);
            StartCoroutine(coin.MoveTo(randomPosition, randomDuration));
        }

        // Debug
        if (debug)
        {
            int totalCents = 0;
            Debug.Log("Coins = " + generatedCentAmounts.Count);

            foreach (int amount in generatedCentAmounts)
            {
                Debug.Log("Cent amount = " + amount);
                totalCents += amount;
            }

            Debug.Log("Total cents = " + totalCents);
        }
    }

    List<int> GenerateCentAmounts()
    {
        // Initialize variables
        int remainingCents = maxCents;
        List<int> generatedCentAmounts = new List<int>();

        // Get remaining cent amounts
        List<int> remainingCentAmounts = GetRemainingCentAmounts(remainingCents);

        while (remainingCents > 0)
        {
            // Create a new cent amounts copy list
            List<int> centAmountsCopy = centAmounts.ToList();

            // Remove centAmounts greater than the remainingCents
            if (centAmountsCopy[centAmountsCopy.Count - 1] > remainingCents)
            {
                for (int i = centAmountsCopy.Count - 1; i >= 0; i--)
                {
                    if (centAmountsCopy[i] > remainingCents)
                    {
                        centAmountsCopy.Remove(centAmountsCopy[i]);
                    }
                    else
                        break;
                }
            }

            // Randomly select cent amount
            int randomIndex = UnityEngine.Random.Range(0, centAmountsCopy.Count);
            int centAmount = centAmountsCopy[randomIndex];

            // Create remainingCentAmountsTest and get remaining cent amounts
            List<int> remainingCentAmountsTest = GetRemainingCentAmounts(remainingCents - centAmount);

            // Check if the remaining coins surpass the maxCoins,
            // 1 is for the centAmount that has been randomly selected but has not been added yet
            while (1 + generatedCentAmounts.Count + remainingCentAmountsTest.Count > maxCoins)
            {
                // Remove cent amount from centAmountsCopy
                centAmountsCopy.Remove(centAmount);

                // Escape loop if there are no more centAmounts to choose 
                if (centAmountsCopy.Count <= 0)
                {
                    centAmount = 0;
                    break;
                }

                // Randomly select new cent amount
                randomIndex = UnityEngine.Random.Range(0, centAmountsCopy.Count);
                centAmount = centAmountsCopy[randomIndex];

                // Get remaining cent amounts
                remainingCentAmountsTest = GetRemainingCentAmounts(remainingCents - centAmount);
            }

            // Calculate chance to stop before it reaches maxCoins
            int randomDice = UnityEngine.Random.Range(0, chanceForMaxCoins);

            // No cent amount could be added
            if (centAmount == 0 || randomDice == 0)
            {
                // Join the two lists 
                generatedCentAmounts.AddRange(remainingCentAmounts);
                break;
            }

            // Set remainingCentAmounts
            remainingCentAmounts = remainingCentAmountsTest;

            // Add amount to generatedCentAmounts list
            generatedCentAmounts.Add(centAmount);

            // Subtract amount from remainingCents
            remainingCents -= centAmount;
        }

        return generatedCentAmounts;
    }

    List<int> GetRemainingCentAmounts(int _remainingCents)
    {
        // Create remainingCentAmounts list
        List<int> remainingCentAmounts = new List<int>();

        // Set remainingCentAmounts
        while (_remainingCents > 0)
        {
            for (int i = centAmounts.Length - 1; i >= 0; i--)
            {
                if (_remainingCents >= centAmounts[i])
                {
                    _remainingCents -= centAmounts[i];
                    remainingCentAmounts.Add(centAmounts[i]);
                    break;
                }
            }
        }

        return remainingCentAmounts;
    }

    public void RemoveCoin(Coin _coin)
    {
        // Remove coin from List
        coins.Remove(_coin);

        // Check if all coins are collected
        if (coins.Count <= 0)
            OnCollectedAll?.Invoke();
    }

    public void RemoveAllCoins()
    {
        // Destroy all coins
        foreach (Coin coin in coins)
            Destroy(coin.gameObject);

        // Clear coins list
        coins.Clear();
    }

    public void AddValueToChanceForMaxCoins(int _value)
    {
        chanceForMaxCoins = startChanceForMaxCoins + _value;
    }

    public void ResetChanceForMaxCoins()
    {
        chanceForMaxCoins = startChanceForMaxCoins;
    }
    #endregion
}
