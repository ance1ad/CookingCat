using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CongratulationManager : MonoBehaviour {
    
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _congratGreenText;
    [SerializeField] private TMP_Text _newStatusLevelText;
    
    
    [SerializeField] private TMP_Text _newCoinsCount;
    [SerializeField] private TMP_Text _newGemsCount;

    private void Start() {
        CloseWindow();
        SettingsManager.Instance.OnSwipeLanguage += GetTextLocalization;
        GetTextLocalization();
    }

    private void GetTextLocalization() {
        _congratGreenText.text = LocalizationManager.Get("Congratulations");
    }


    public void CloseWindow() {
        _canvas.SetActive(false);
        PlayerBankVisual.Instance.HideBank();
    }
    
    public void OpenWindow() {
        _canvas.SetActive(true);
    }

    public void CongratWithNewStatus(int level, string status) {
        OpenWindow();
        _title.text = LocalizationManager.Get("StatusUp");
        _newStatusLevelText.text = LocalizationManager.Get("YouHaveNewStatus", status);
        
        float newCoins = level/5f * 10000;
        int newGems = level/5 * 10;
        
        CurrencyManager.Instance.UpdateCash(newCoins, newGems);
        _newCoinsCount.text = "+" + newCoins;
        _newGemsCount.text = "+" + newGems;
    }
    

    public void CongratWithNewLevel(int level) {
        OpenWindow();
        _title.text = LocalizationManager.Get("LevelUp");
        _newStatusLevelText.text = LocalizationManager.Get("YouHaveNewLevel", level);
        float newCoins = level * 1000;
        int newGems = level * 2;
        CurrencyManager.Instance.UpdateCash(newCoins, newGems);
        _newCoinsCount.text = "+" + newCoins;
        _newGemsCount.text = "+" + newGems;
    }



}
