using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour {
    
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    [SerializeReference] private Button _musicButton;
    [SerializeReference] private Button _sfxButton;
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _offMusicLine;
    [SerializeField] private GameObject _offSoundLine;

    
    private void Awake() {
        _settingsCanvas.SetActive(false);
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
