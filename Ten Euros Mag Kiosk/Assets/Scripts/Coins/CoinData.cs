using UnityEngine;

[CreateAssetMenu]
public class CoinData : ScriptableObject
{
    // The sprite of the coin
    public Sprite sprite;
    // The amount of cents it is worth
    public int centAmount;
}
