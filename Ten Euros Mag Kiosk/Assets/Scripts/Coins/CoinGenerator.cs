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
    CoinData coinData;

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
        for (int i = 0; i < 10; i++)
        {
            Coin coin = Instantiate(coinPrefab, startPosition, Quaternion.identity, coinsParent);
            coin.SetupCoin(coinData);

            Vector3 randomPosition = new Vector3(Random.Range(minRangeX, maxRangeX), Random.Range(minRangeY, maxRangeY), 0f);
            float randomDuration = Random.Range(minDuration, maxDuration);
            StartCoroutine(coin.MoveTo(randomPosition, randomDuration));
        }
    }
    #endregion
}
