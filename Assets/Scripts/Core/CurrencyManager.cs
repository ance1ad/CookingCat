using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Codice.Client.Common;
using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour {
    
    [SerializeField] private TMP_Text _coinsCountText;
    [SerializeField] private TMP_Text _gemsCountText;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _newCoinsText;
    [SerializeField] private TMP_Text _newGemsText;
    [SerializeField] private TMP_Text _orderTimeText;
    [SerializeField] private TMP_Text _accuracyText;
    [SerializeField] private TMP_Text _comboText;

    public void SetOrderResult(float newCoins, int newGems, string orderTime, string accuracy, string combo) {
        ShowCanvas();
        if (newCoins > 0) {
            _newCoinsText.text = ("+" + newCoins.ToString("0"));
            _newCoinsText.color = Color.green;
        }
        else if (newCoins <= 0) {
            _newCoinsText.text = (newCoins.ToString("0"));
            _newCoinsText.color = Color.red;
        } 
        AddCoins(newCoins);
        

        if (newGems > 0) {
            _newGemsText.text = ("+" + newGems);
            AddGems(newGems);
            _newGemsText.color = Color.green;
        }
        else if (newGems == 0) {
            _newGemsText.enabled = false;
        }
        
        if (orderTime != string.Empty) {
            _orderTimeText.text = orderTime;
            _orderTimeText.enabled = true;
        }
        if (accuracy != string.Empty) {
            _accuracyText.text = accuracy;
            _accuracyText.enabled = true;
        }
        if (combo != string.Empty) {
            _comboText.text = combo;
            _comboText.enabled = true;
        }
    }
    
    // Банк игрока
    public static CurrencyManager Instance { get; private set; }
    public float Coins { get; private set; } = 0f; // обычные монетки
    public int Gems { get; private set; } = 0; // ебать крутая валюта

    
    
    public void SpentCoins(float price) {
        if (price <= Coins) {
            Coins -= price;
            _coinsCountText.text = Coins.ToString("0");
        }
        else {
            Debug.Log("НЕ хватает монетов");
        }
    } 
    
    
    public void AddCoins(float price) {
        Coins += price;
        _coinsCountText.text = Coins.ToString("0");
    }
    
    public void SpentGems(int price) {
        if (price <= Gems) {
            Gems -= price;
            _gemsCountText.text = Gems.ToString();
        }
        else {
            Debug.Log("НЕ хватает гемов");
        }
    } 
    


    public void AddGems(int price) {
        Gems += price;
        _gemsCountText.text = Gems.ToString();
    } 
    
    
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("There can only be one instance of CurrencyManager");
            return;
        }
        Instance = this;
        _gemsCountText.text = Gems.ToString();
        _coinsCountText.text = Coins.ToString("0");
        
    }

    public void CloseCanvas() {
        _canvas.SetActive(false);
    }
    
    public void ShowCanvas() {
        _canvas.SetActive(true);
    }
    
}
