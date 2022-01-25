using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : Singleton<CoinGenerator>
{
    #region Variables
    [SerializeField]
    Coin coinPrefab;
    // Current generated coins parent's transform
    [SerializeField]
    Transform coinsParent;
    // The world position a coin is instantiated in
    [SerializeField]
    Vector3 startPosition;

    [SerializeField]
    CoinData[] coinData;

    // Get minimum coin count
    int maxCents = 1000;
    int maxCoins = 20;
    //int remainingCents;
    static readonly int[] centAmounts = { 1, 2, 5, 10, 20, 50, 100, 200};

    // Random Position
    static readonly float minRangeX = -4f;
    static readonly float maxRangeX = 4f;
    static readonly float minRangeY = -4.5f;
    static readonly float maxRangeY = 0f;

    // Random Duration
    static readonly float minDuration = 1f;
    static readonly float maxDuration = 2f;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        GenerateCoins();
    }
    #endregion

    #region Methods
    void GenerateCoins()
    {
        // Generate cent amounts
        List<int> generatedCentAmounts = GenerateCentAmounts();

        for (int i = 0; i < generatedCentAmounts.Count; i++)
        {
            Coin coin = Instantiate(coinPrefab, startPosition, Quaternion.identity, coinsParent);
            int centAmount = generatedCentAmounts[i];
            coin.SetupCoin(coinData[Array.IndexOf(centAmounts, centAmount)]);

            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(minRangeX, maxRangeX), UnityEngine.Random.Range(minRangeY, maxRangeY), 0f);
            float randomDuration = UnityEngine.Random.Range(minDuration, maxDuration);
            StartCoroutine(coin.MoveTo(randomPosition, randomDuration));
        }
    }

    List<int> GenerateCentAmounts()
    {
        // Initialize variables
        int remainingCents = maxCents;
        List<int> generatedCentAmounts = new List<int>();

        while (remainingCents > 0)
        {
            // Get remaining cent amounts
            List<int> remainingCentAmounts = GetRemainingCentAmounts(remainingCents);

            // Check if there are enough coin slots left
            if (remainingCentAmounts.Count < maxCoins)
            {
                // Randomly select cent amount
                int randomIndex = UnityEngine.Random.Range(0, centAmounts.Length);

                // Add amount to generatedCentAmounts list
                int centAmount = centAmounts[randomIndex];
                generatedCentAmounts.Add(centAmount);

                // Subtract amount from remainingCents
                remainingCents -= centAmount;
            }
            else
            {
                // Join the two lists 
                generatedCentAmounts.AddRange(remainingCentAmounts);
            }
        }

        return generatedCentAmounts;
    }

    List<int> GetRemainingCentAmounts(int _remainingCents)
    {
        List<int> remainingCentAmounts = new List<int>();

        int remainingCents = _remainingCents;

        while (remainingCents > 0)
        {
            for (int i = centAmounts.Length - 1; i >= 0; i--)
            {
                if (remainingCents >= centAmounts[i])
                {
                    remainingCents -= centAmounts[i];
                    remainingCentAmounts.Add(centAmounts[i]);
                    break;
                }
            }
        }

        return remainingCentAmounts;
    }
    #endregion
}
