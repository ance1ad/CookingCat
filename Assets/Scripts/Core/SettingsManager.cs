using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using YG;

public class SettingsManager : MonoBehaviour {
    [SerializeField] private TMP_Text _windowText;
    
    
    
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    [SerializeReference] private Button _musicButton;
    [SerializeReference] private Button _sfxButton;
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _offMusicLine;
    [SerializeField] private GameObject _offSoundLine;
    
    [SerializeField] private Button _localizationButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TMP_Text _textUnderBtnLocalization;
    [SerializeField] private TMP_Text _textInBtnLocalization;

    public static SettingsManager Instance { get; private set; }

    


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There can only be one Instance of type SettingsManager");
            return;
        }

        Instance = this;
        _settingsCanvas.SetActive(false);
        _localizationButton.onClick.AddListener(() => SwipeLanguage());
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
        SetText();
    }

    private void OnMainMenuButtonClick() {
        MainMenu.Instance.OpenMainMenu();
        _settingsCanvas.SetActive(false);
    }

    public event Action OnSwipeLanguage;
        
    private void SwipeLanguage() {
        LocalizationManager.SwipeLanguage();
        SetText();
        OnSwipeLanguage?.Invoke();
        Debug.Log("SwipeLanguage");
    }

    private void SetText() {
        if (LocalizationManager.CurrentLanguage == Language.EN) {
            _textUnderBtnLocalization.text = "English language";
            _textInBtnLocalization.text = "Eng";
            _windowText.text = "Settings";
        }
        else {
            _textUnderBtnLocalization.text = "Русский язык";
            _textInBtnLocalization.text = "Rus";
            _windowText.text = "Настройки";
        }
    }

    private void Start() {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        // muteToggle.isOn = PlayerPrefs.GetInt("AudioMuted", 0) == 1;

        musicSlider.onValueChanged.AddListener(SoundManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
        _musicButton.onClick.AddListener(() => OffOnMusic());
        _sfxButton.onClick.AddListener(()=> OffOnSoundEffects());

    }

    public void ShowHideSettings() {
        PlayerBankVisual.Instance.HideBank();
        _settingsCanvas.SetActive(!_settingsCanvas.activeSelf);
        Time.timeScale = _settingsCanvas.activeSelf ? 0 : 1;
        if (!_settingsCanvas.activeSelf) {
            YGManager.Instance.ShowInterstitialWarningAds();
        }
    } 
    
    public void OffOnMusic() {
        SoundManager.Instance.MuteMusic(!_offMusicLine.activeSelf);
        _offMusicLine.SetActive(!_offMusicLine.activeSelf); 
    
    }
    public void OffOnSoundEffects() {
        SoundManager.Instance.MuteSFX(!_offSoundLine.activeSelf);
        _offSoundLine.SetActive(!_offSoundLine.activeSelf);
    }
}
