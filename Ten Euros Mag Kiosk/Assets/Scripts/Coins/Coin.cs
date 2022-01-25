using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    #region Variables
    CoinData coinData;
    SpriteRenderer spriteRenderer;
    //Vector3 endPosition;
    public bool Interactable { get; private set; }
    //static readonly float defaultTweenDuration = 2f; //0.25f;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        InitializeVariables();
    }

    void Start()
    {

    }
    #endregion

    #region Methods
    void InitializeVariables()
    {
        Interactable = false;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    void SetCoinData(CoinData _coinData)
    {
        coinData = _coinData;
    }

    void SetSprite(Sprite _sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = _sprite;
    }

    /*void SetEndPosition(Vector3 _endPosition)
    {
        endPosition = _endPosition;
    }*/

    public void SetupCoin(CoinData _coinData/*, Vector3 _endPosition*/)
    {
        //Debug.Log(Interactable);
        SetCoinData(_coinData);
        SetSprite(_coinData.sprite);
        //SetEndPosition(_endPosition);
    }

    public IEnumerator MoveTo(Vector3 _position, float _duration)
    {
        Tween tween = transform.DOMove(_position, _duration).SetEase(Ease.OutQuart);

        yield return tween.Elapsed();

        Interactable = true;
        //Debug.Log(Interactable);
    }
    #endregion
}
