using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    #region Variables
    [Header("Game UI Elements")]
    [SerializeField]
    Slider timerSlider;
    [SerializeField]
    TMP_Text timerText;
    [SerializeField]
    TMP_Text roundText;
    [SerializeField]
    TMP_Text roundInfoText;
    [Space]
    [Header("Leaderboard")]
    [SerializeField]
    GameObject leaderboardPanel;
    [SerializeField]
    Button menuButton;
    [SerializeField]
    Button restartButton;
    [SerializeField]
    TMP_Text[] scoreTexts;
    
    static readonly string READY_TEXT = "READY";
    static readonly string SET_TEXT = "SET";
    static readonly string GO_TEXT = "GO!";
    static readonly string WIN_TEXT = "ROUND WON!";
    static readonly string LOSE_TEXT = "GAME OVER";
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        SubscribeButtons();
    }
    #endregion

    #region Methods
    void SubscribeButtons()
    {
        // Leaderboard
        menuButton.onClick.AddListener(MenuButtonPressed);
        restartButton.onClick.AddListener(RestartButtonPressed);
    }

    #region Round
    public void SetRoundText(int _round) => roundText.text = _round.ToString();
    #endregion

    #region Timer
    public void SetTimer(int _secondsLeft)
    {
        // Set timer text
        timerText.text = _secondsLeft.ToString();

        // Set slider
        timerSlider.value = _secondsLeft;
    }

    public void SetSliderMaxValue(int _maxValue) => timerSlider.maxValue = _maxValue;
    #endregion

    #region Round Info Text
    public void DisplayReady() => roundInfoText.text = READY_TEXT;
    public void DisplaySet() => roundInfoText.text = SET_TEXT;
    public void DisplayGo() => roundInfoText.text = GO_TEXT;
    public void DisplayWin() => roundInfoText.text = WIN_TEXT;
    public void DisplayGameOver() => roundInfoText.text = LOSE_TEXT;
    public void DisplayNothing() => roundInfoText.text = string.Empty;
    #endregion

    #region Leaderboard
    public void DisplayLeaderboard(List<Score> _leaderboard)
    {
        // Set scoreTexts
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if (i < _leaderboard.Count)
            {
                // Set score text
                scoreTexts[i].text = string.Format("{0} | {1} | {2}",
                    _leaderboard[i].round.ToString(),
                    _leaderboard[i].dateTime.ToShortDateString(),
                    _leaderboard[i].dateTime.ToShortTimeString());

                // Enable score text game object
                if (!scoreTexts[i].transform.parent.gameObject.activeSelf)
                    scoreTexts[i].transform.parent.gameObject.SetActive(true);
            }
            else
                break;
        }

        // Enable leaderboard panel
        leaderboardPanel.SetActive(true);
    }

    void MenuButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayOptionSelectedSFX();

        // Disable leaderboard panel
        leaderboardPanel.SetActive(false);

        // Stop Music
        GameManager.Instance.StopLeaderboardMusic();

        // Display menu
        UIManager.Instance.menuUIController.OpenMainMenu();
    }

    void RestartButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayOptionSelectedSFX();

        // Disable leaderboard panel
        leaderboardPanel.SetActive(false);

        // Restart Game
        GameManager.Instance.RestartGame();
    }
    #endregion

    #endregion
}
