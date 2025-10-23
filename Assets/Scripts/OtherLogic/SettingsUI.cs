using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour {
    [SerializeField] private GameObject _offMusicLine;
    [SerializeField] private GameObject _offSoundLine;
    [SerializeField] private GameObject _productStore;
    [SerializeField] private GameObject _settingsCanvas;
    [SerializeField] private GameObject _scinStore;


    private void Awake() {
        _settingsCanvas.SetActive(false);
        _scinStore.SetActive(false);
        _productStore.SetActive(false);
    }


    public void ShowHideSettings() => _settingsCanvas.SetActive(!_settingsCanvas.activeSelf);
    public void OnClickProductStore() => _productStore.SetActive(!_productStore.activeSelf);
    
    public void OnClickScinStore() => _scinStore.SetActive(!_scinStore.activeSelf);
    public void OffOnMusic() {
        // some music logic
        _offMusicLine.SetActive(!_offMusicLine.activeSelf); 

    }
    public void OffOnSoundEffects() {
        // some sound logic
        _offSoundLine.SetActive(!_offSoundLine.activeSelf);
    }
    
    

    
}
