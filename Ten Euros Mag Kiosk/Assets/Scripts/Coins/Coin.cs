using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class Coin : MonoBehaviour
{
    #region Variables
    CoinData coinData;
    SpriteRenderer spriteRenderer;
    CircleCollider2D circleCollider2D;
    public bool Interactable { get; private set; }

    public static int orderInLayer;
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
        circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
    }

    void SetCoinData(CoinData _coinData)
    {
        coinData = _coinData;
    }

    void SetSprite(Sprite _sprite)
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = _sprite;

        // Set order in layer
        spriteRenderer.sortingOrder = orderInLayer + 1;
        orderInLayer = spriteRenderer.sortingOrder;

        // Set Collider size
        SetCircleCollider2DSize();
    }

    void SetCircleCollider2DSize()
    {
        Vector3 spriteHalfSize = spriteRenderer.sprite.bounds.extents;
        circleCollider2D.radius = spriteHalfSize.x > spriteHalfSize.y ? spriteHalfSize.x : spriteHalfSize.y;
    }

    public void SetupCoin(CoinData _coinData)
    {
        SetCoinData(_coinData);
        SetSprite(_coinData.sprite);
    }

    public IEnumerator MoveTo(Vector3 _position, float _duration)
    {
        Tween tween = transform.DOMove(_position, _duration).SetEase(Ease.OutQuart);

        yield return tween.WaitForCompletion();

        // Do something?
    }

    public void CollectCoin()
    {
        // Play Sound
        AudioManager.Instance.PlaySound("CoinPickUp");

        // Remove coin from coins
        CoinGenerator.Instance.RemoveCoin(this);

        // Destroy coin
        Destroy(gameObject);
    }

    public void EnableInteractable()
    {
        Interactable = true;
    }
    #endregion
}
