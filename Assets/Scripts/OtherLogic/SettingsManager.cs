using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


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
    [SerializeField] private TMP_Text _textUnderBtnLocalization;
    [SerializeField] private TMP_Text _textInBtnLocalization;


    
    private void Awake() {
        _settingsCanvas.SetActive(false);
        _localizationButton.onClick.AddListener(() => SwipeLanguage());
        SetText();
    }

    private void SwipeLanguage() {
        LocalizationManager.SwipeLanguage();
        SetText();
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

    public void ShowHideSettings() => _settingsCanvas.SetActive(!_settingsCanvas.activeSelf);
    
    public void OffOnMusic() {
        SoundManager.Instance.MuteMusic(!_offMusicLine.activeSelf);
        _offMusicLine.SetActive(!_offMusicLine.activeSelf); 
    
    }
    public void OffOnSoundEffects() {
        SoundManager.Instance.MuteSFX(!_offSoundLine.activeSelf);
        _offSoundLine.SetActive(!_offSoundLine.activeSelf);
    }
}
