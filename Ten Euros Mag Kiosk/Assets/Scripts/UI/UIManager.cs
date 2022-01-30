using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : Singleton<UIManager>
{
    #region Variables
    public MenuUIController menuUIController;
    public GameUIController gameUIController;

    [SerializeField]
    CanvasGroup canvasGroup;
    #endregion

    #region Unity Callbacks
    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        // Open Main Menu
        menuUIController.OpenMainMenu();
    }
    #endregion

    #region Methods
    public void PlayOptionSelectedSFX()
    {
        AudioManager.Instance.PlaySound("MenuClick");
    }

    public void PlayBackSelectedSFX()
    {
        AudioManager.Instance.PlaySound("MenuBack");
    }

    /*public IEnumerator MoveMenuTransition(bool _down, Action _endAction)
    {
        // Disable canvas interactivity
        canvasGroup.interactable = false;

        // Move
        yield return new WaitForSeconds(0f);

        // Invoke Action
        _endAction?.Invoke();

        // Enable canvas interactivity
        canvasGroup.interactable = true;
    }*/
    #endregion
}

