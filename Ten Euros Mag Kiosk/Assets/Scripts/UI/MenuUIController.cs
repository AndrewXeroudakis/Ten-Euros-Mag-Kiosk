using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    #region Variables
    [Header("Menu UI Elements")]
    public GameObject menuContainer;
    public GameObject menuTransition;
    public GameObject menuOptionsContainer;
    public GameObject popUpDialogContainer;
    [Space]
    [Header("Play")]
    public Button playButton;
    [Space]
    [Header("Options")]
    public Button optionsButton;
    public GameObject optionsContainer;
    public Slider soundEffectsVolumeSlider;
    public Slider musicVolumeSlider;
    public Button noButton;
    public Button yesButton;
    [Space]
    [Header("Quit")]
    public GameObject quitDialogPanel;
    public Button quitButton;
    public Button quitYesButton;
    public Button quitNoButton;

    static readonly string SOUNDEFFECTS_VOLUME_KEY = "soundEffectsVolume";
    static readonly string MUSIC_VOLUME_KEY = "musicVolume";
    static readonly float defaultSoundEffectsVolume = 0.5f;
    static readonly float defaultMusicVolume = 0.75f;
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        SubscribeButtons();
        SubscribeEvents();
    }
    #endregion

    #region Methods
    void SubscribeButtons()
    {
        // Play Game
        playButton.onClick.AddListener(PlayButtonPressed);
        //playButton.onClick.AddListener(UIManager.Instance.PlayOptionSelectedSFX);

        // Options
        optionsButton.onClick.AddListener(OptionsButtonPressed);
        //optionsButton.onClick.AddListener(UIManager.Instance.PlayOptionSelectedSFX);
        noButton.onClick.AddListener(NoButtonPressed);
        //noButton.onClick.AddListener(UIManager.Instance.PlayOptionSelectedSFX);
        yesButton.onClick.AddListener(YesButtonPressed);
        //yesButton.onClick.AddListener(UIManager.Instance.PlayBackSelectedSFX);

        // Quit
        quitButton.onClick.AddListener(QuitButtonPressed);
        //quitButton.onClick.AddListener(UIManager.Instance.PlayBackSelectedSFX);
        quitYesButton.onClick.AddListener(QuitYesButtonPressed);
        //quitYesButton.onClick.AddListener(UIManager.Instance.PlayOptionSelectedSFX);
        quitNoButton.onClick.AddListener(QuitNoButtonPressed);
        //quitNoButton.onClick.AddListener(UIManager.Instance.PlayBackSelectedSFX);
    }

    void SubscribeEvents()
    {
        soundEffectsVolumeSlider.onValueChanged.AddListener(delegate { SetVolume(SOUNDEFFECTS_VOLUME_KEY, soundEffectsVolumeSlider.value); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetVolume(MUSIC_VOLUME_KEY, musicVolumeSlider.value); });
    }

    public void OpenMainMenu()
    {
        // Disable popUpDialogContainer
        HidePopupDialogContainer(true);

        // Enable menu options container
        HideMenuOptions(false);

        // Enable menu container
        HideMenu(false);

        // Play Music
        //AudioManager.Instance.PlayMusic("");
    }

    #region Hide Scripts
    public void HideMenu(bool _isHidden)
    {
        if (_isHidden)
        {
            // Disable main menu container
            if (menuContainer.activeSelf)
                menuContainer.SetActive(false);
        }
        else
        {
            // Enable main menu container
            if (!menuContainer.activeSelf)
                menuContainer.SetActive(true);
        }
    }

    public void HideMenuOptions(bool _isHidden)
    {
        if (_isHidden)
        {
            // Disable menu options container
            if (menuOptionsContainer.activeSelf)
                menuOptionsContainer.SetActive(false);
        }
        else
        {
            // Enable main menu options container
            if (!menuOptionsContainer.activeSelf)
                menuOptionsContainer.SetActive(true);
        }
    }

    void HidePopupDialogContainer(bool _isHidden)
    {
        if (_isHidden)
        {
            // Disable popUpDialogContainer
            if (popUpDialogContainer.activeSelf)
                popUpDialogContainer.SetActive(false);
        }
        else
        {
            // Enable popUpDialogContainer
            if (!popUpDialogContainer.activeSelf)
                popUpDialogContainer.SetActive(true);
        }
    }

    public void HideOptions(bool _isHidden)
    {
        if (_isHidden)
        {
            // Disable options container
            if (optionsContainer.activeSelf)
                optionsContainer.SetActive(false);
        }
        else
        {
            // Enable options container
            if (!optionsContainer.activeSelf)
                optionsContainer.SetActive(true);
        }
    }
    #endregion

    #region Play Game
    void PlayButtonPressed()
    {
        // Hide Menu
        HideMenu(true);

        // Start Game
        GameManager.Instance.StartGame();

        // Move Transition
        //StartCoroutine(UIManager.Instance.MoveMenuTransition(true, delegate { GameManager.Instance.StartGame(); }));
    }
    #endregion

    #region Options
    void OptionsButtonPressed()
    {
        // Load volumes
        LoadVolume();

        // Show options
        HideOptions(false);

        // Show popUpDialogContainer
        HidePopupDialogContainer(false);
    }

    void SetVolume(string _key, float _value)
    {
        AudioManager.Instance.audioMixer.SetFloat(_key, Mathf.Log10(_value) * 20);
    }

    void SaveVolume()
    {
        PlayerPrefs.SetFloat(SOUNDEFFECTS_VOLUME_KEY, soundEffectsVolumeSlider.value);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolumeSlider.value);
    }

    void LoadVolume()
    {
        if (PlayerPrefs.HasKey(SOUNDEFFECTS_VOLUME_KEY))
            soundEffectsVolumeSlider.value = PlayerPrefs.GetFloat(SOUNDEFFECTS_VOLUME_KEY);
        else
            soundEffectsVolumeSlider.value = defaultSoundEffectsVolume;

        if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        else
            musicVolumeSlider.value = defaultMusicVolume;
    }

    void NoButtonPressed()
    {
        // Reset values
        soundEffectsVolumeSlider.value = defaultSoundEffectsVolume;
        musicVolumeSlider.value = defaultMusicVolume;

        // Hide options
        HideOptions(true);

        // Hide popUpDialogContainer
        HidePopupDialogContainer(true);
    }

    void YesButtonPressed()
    {
        //Save to player prefs
        SaveVolume();

        // Hide options
        HideOptions(true);

        // Hide popUpDialogContainer
        HidePopupDialogContainer(true);

        // Fade Transition
        /*StartCoroutine(UIManager.Instance.FadeTransition(delegate { HideAudio(true); },
                                                         delegate { UIManager.Instance.optionsController.HideOptions(false); }));*/
    }
    #endregion

    #region Quit
    void QuitButtonPressed()
    {
        // Enable popUpDialogContainer
        if (!popUpDialogContainer.activeSelf)
            popUpDialogContainer.SetActive(true);

        // Open Quit Dialog
        if (!quitDialogPanel.activeSelf)
            quitDialogPanel.SetActive(true);
    }

    void QuitYesButtonPressed()
    {
        Quit();
    }

    void QuitNoButtonPressed()
    {
        // Disable popUpDialogContainer
        if (popUpDialogContainer.activeSelf)
            popUpDialogContainer.SetActive(false);
    }

    void Quit()
    {
        Application.Quit();
    }
    #endregion

    #endregion
}
