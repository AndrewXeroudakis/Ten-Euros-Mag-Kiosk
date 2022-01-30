using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    #region Variables
    [Header("Menu UI Elements")]
    public GameObject menuContainer;
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
    public Button resetButton;
    public Button yesButton;
    [Space]
    [Header("Quit")]
    public GameObject quitContainer;
    public Button quitButton;
    public Button quitYesButton;
    public Button quitNoButton;

    // Music and Sound
    static readonly string SOUNDEFFECTS_VOLUME_KEY = "soundEffectsVolume";
    static readonly string MUSIC_VOLUME_KEY = "musicVolume";
    static readonly float defaultSoundEffectsVolume = 0.5f;
    static readonly float defaultMusicVolume = 0.5f;
    AudioSource menuMusic;
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

        // Options
        optionsButton.onClick.AddListener(OptionsButtonPressed);
        resetButton.onClick.AddListener(ResetButtonPressed);
        yesButton.onClick.AddListener(YesButtonPressed);

        // Quit
        quitButton.onClick.AddListener(QuitButtonPressed);
        quitYesButton.onClick.AddListener(QuitYesButtonPressed);
        quitNoButton.onClick.AddListener(QuitNoButtonPressed);
    }

    void SubscribeEvents()
    {
        soundEffectsVolumeSlider.onValueChanged.AddListener(delegate { SetVolume(SOUNDEFFECTS_VOLUME_KEY, soundEffectsVolumeSlider.value); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetVolume(MUSIC_VOLUME_KEY, musicVolumeSlider.value); });
    }

    public void OpenMainMenu()
    {
        // Unause
        Time.timeScale = 1;

        // Disable popUpDialogContainer
        HidePopupDialogContainer(true);

        // Enable menu options container
        HideMenuOptions(false);

        // Enable menu container
        HideMenu(false);

        // Play Music
        if (menuMusic == null)
            menuMusic = AudioManager.Instance.PlayMusic("MenuMusic");
        else
        {
            menuMusic.Stop();
            menuMusic.Play();
        }
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

    public void HideQuit(bool _isHidden)
    {
        if (_isHidden)
        {
            // Disable options container
            if (quitContainer.activeSelf)
                quitContainer.SetActive(false);
        }
        else
        {
            // Enable options container
            if (!quitContainer.activeSelf)
                quitContainer.SetActive(true);
        }
    }
    #endregion

    #region Play Game
    void PlayButtonPressed()
    {
        // Stop music
        if (menuMusic != null)
            menuMusic.Stop();

        // Play Sound
        UIManager.Instance.PlayOptionSelectedSFX();

        // Hide Menu
        HideMenu(true);

        // Open Game UI
        UIManager.Instance.gameUIController.OpenGameUI();

        // Start Game
        GameManager.Instance.StartGame();
    }
    #endregion

    #region Options
    void OptionsButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayOptionSelectedSFX();

        // Load volumes
        LoadVolume();

        // Show options
        HideOptions(false);

        // Hide quit
        HideQuit(true);

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

    void ResetButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayBackSelectedSFX();

        // Reset values
        soundEffectsVolumeSlider.value = defaultSoundEffectsVolume;
        musicVolumeSlider.value = defaultMusicVolume;
    }

    void YesButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayOptionSelectedSFX();

        //Save to player prefs
        SaveVolume();

        // Hide options
        HideOptions(true);

        // Hide popUpDialogContainer
        HidePopupDialogContainer(true);
    }
    #endregion

    #region Quit
    void QuitButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayOptionSelectedSFX();

        // Hide options
        HideOptions(true);

        // Show quit
        HideQuit(false);

        // Show popUpDialogContainer
        HidePopupDialogContainer(false);
    }

    void QuitYesButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayOptionSelectedSFX();

        Quit();
    }

    void QuitNoButtonPressed()
    {
        // Play Sound
        UIManager.Instance.PlayBackSelectedSFX();

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
