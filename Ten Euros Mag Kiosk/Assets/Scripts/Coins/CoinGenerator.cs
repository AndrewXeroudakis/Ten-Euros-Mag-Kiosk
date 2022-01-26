using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    static readonly int[] centAmounts = { 1, 2, 5, 10, 20, 50, 100, 200};

    static readonly int[] safetyCentAmounts = { 200, 200, 200, 200, 200 };

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
        maxCents = 1000;
        maxCoins = 20;
        Debug.Log("maxCents = " + maxCents);
        Debug.Log("maxCoins = " + maxCoins);
        GenerateCoins();
    }
    #endregion

    #region Methods
    void GenerateCoins()
    {
        // Generate cent amounts
        List<int> generatedCentAmounts = GenerateCentAmounts();

        int totalCents = 0;
        Debug.Log("generatedCentAmounts.Count = " + generatedCentAmounts.Count);

        for (int i = 0; i < generatedCentAmounts.Count; i++)
        {
            Coin coin = Instantiate(coinPrefab, startPosition, Quaternion.identity, coinsParent);
            int centAmount = generatedCentAmounts[i];
            coin.SetupCoin(coinData[Array.IndexOf(centAmounts, centAmount)]);

            Debug.Log("centAmount = " + centAmount);
            totalCents += centAmount;
            
            Vector3 randomPosition = new Vector3(UnityEngine.Random.Range(minRangeX, maxRangeX), UnityEngine.Random.Range(minRangeY, maxRangeY), 0f);
            float randomDuration = UnityEngine.Random.Range(minDuration, maxDuration);
            StartCoroutine(coin.MoveTo(randomPosition, randomDuration));
        }

        Debug.Log("totalCents = " + totalCents);
    }

    List<int> GenerateCentAmounts()
    {
        // Initialize variables
        int remainingCents = maxCents;
        List<int> generatedCentAmounts = new List<int>();

        // Get remaining cent amounts
        List<int> remainingCentAmounts = GetRemainingCentAmounts(remainingCents);
        Debug.Log("remainingCentAmounts.Count = " + remainingCentAmounts.Count);

        while (remainingCents > 0)
        {
            // Check if maxCoins have been reached
            /*if (generatedCentAmounts.Count + remainingCentAmounts.Count == maxCoins)
            {
                // Join the two lists 
                generatedCentAmounts.AddRange(remainingCentAmounts);

                // Debug
                foreach (int amount in remainingCentAmounts)
                {
                    remainingCents -= amount;
                }
                Debug.Log("remainingCents = " + remainingCents);
                break;
            }*/

            // Select random cent amount
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
            Debug.Log("---------------------------------------------");

            // Check if the remaining coins surpass the maxCoins,
            // 1 is for the centAmount that has been randomly selected but has not been added yet
            while (1 + generatedCentAmounts.Count + remainingCentAmountsTest.Count > maxCoins)
            {
                // Remove cent amount from centAmountsCopy
                centAmountsCopy.Remove(centAmount);
                Debug.Log("Removed cent amount = " + centAmount);

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

            // No cent amount could be added
            if (centAmount == 0)
            {
                Debug.Log("No cent amount could be added");
                // Join the two lists 
                generatedCentAmounts.AddRange(remainingCentAmounts);

                // Debug
                foreach (int amount in remainingCentAmounts)
                {
                    remainingCents -= amount;
                }
                Debug.Log("remainingCents = " + remainingCents);
                break;
            }

            Debug.Log("Selected cent amount = " + centAmount);
            // Set remainingCentAmounts
            remainingCentAmounts = remainingCentAmountsTest;
            Debug.Log("remainingCentAmounts.Count = " + remainingCentAmounts.Count);

            // Add amount to generatedCentAmounts list
            //int centAmount = centAmounts[randomIndex];
            generatedCentAmounts.Add(centAmount);
            Debug.Log("generatedCentAmounts.Count = " + generatedCentAmounts.Count);
            Debug.Log("---------------------------------------------");

            // Subtract amount from remainingCents
            remainingCents -= centAmount;


            // Check if there are enough coin slots left to randomly generate cent amounts
            /*if (generatedCentAmounts.Count + remainingCentAmounts.Count < maxCoins)
            {
                // Create a cent amounts copy list
                List<int> centAmountsCopy = centAmounts.ToList();

                // Randomly select cent amount
                int randomIndex = UnityEngine.Random.Range(0, centAmountsCopy.Count);
                int centAmount = centAmountsCopy[randomIndex];

                // Create remainingCentAmountsTest and get remaining cent amounts
                List<int> remainingCentAmountsTest = GetRemainingCentAmounts(remainingCents - centAmount);
                Debug.Log("---------------------------------------------");
                // Make sure the remaining cent amounts don't surpass the maxCoins
                while (generatedCentAmounts.Count + remainingCentAmountsTest.Count > maxCoins)
                {
                    // Remove cent amount from centAmountsCopy
                    centAmountsCopy.Remove(centAmount);
                    Debug.Log("Removed cent amount = " + centAmount);
                    // Randomly select new cent amount
                    randomIndex = UnityEngine.Random.Range(0, centAmountsCopy.Count);
                    centAmount = centAmountsCopy[randomIndex];
                    
                    // Get remaining cent amounts
                    remainingCentAmountsTest = GetRemainingCentAmounts(remainingCents - centAmount);
                }
                Debug.Log("Selected cent amount = " + centAmount);
                // Set remainingCentAmounts
                remainingCentAmounts = remainingCentAmountsTest;
                Debug.Log("remainingCentAmounts.Count = " + remainingCentAmounts.Count);
                
                // Add amount to generatedCentAmounts list
                //int centAmount = centAmounts[randomIndex];
                generatedCentAmounts.Add(centAmount);
                Debug.Log("generatedCentAmounts.Count = " + generatedCentAmounts.Count);
                Debug.Log("---------------------------------------------");
                // Subtract amount from remainingCents
                remainingCents -= centAmount;
            }
            else
            {
                // Join the two lists 
                generatedCentAmounts.AddRange(remainingCentAmounts);
                break;
            }*/
        }

        return generatedCentAmounts;
    }

    List<int> GetRemainingCentAmounts(int _remainingCents)
    {
        List<int> remainingCentAmounts = new List<int>();

        if (_remainingCents < 0)
            Debug.LogWarning("Negative remainingCents");

        int remainingCents = _remainingCents;
        //Debug.Log("remainingCents = " + remainingCents);
        while (remainingCents > 0)
        {
            for (int i = centAmounts.Length - 1; i >= 0; i--)
            {
                if (remainingCents >= centAmounts[i])
                {
                    remainingCents -= centAmounts[i];
                    remainingCentAmounts.Add(centAmounts[i]);
                    //Debug.Log("centAmount = " + centAmounts[i]);
                    //Debug.Log("remainingCents = " + remainingCents);
                    break;
                }
            }
        }
        //Debug.Log("remainingCentAmounts.Count = " + remainingCentAmounts.Count);
        //Debug.Log("---------------------------------------------");
        return remainingCentAmounts;
    }
    #endregion
}
