using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    #region variables
    #endregion

    #region Unity Callbacks
    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 10);

            if (hit.collider != null)
            {
                if (hit.transform.name == "Coin(Clone)")
                {
                    Coin coin = hit.transform.gameObject.GetComponent<Coin>();

                    if (coin != null && coin.Interactable)
                        coin.CollectCoin();
                }
            }
        }
    }
    #endregion

    #region Methods
    #endregion
}
