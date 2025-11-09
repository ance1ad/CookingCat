using System;
using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour {
    

    
    [SerializeField] private TMP_Text _orderTimeText;
    [SerializeField] private TMP_Text _accuracyText;
    [SerializeField] private TMP_Text _comboText;

    [SerializeField] private TMP_Text _title;
    
    
    public event Action<CurrencyActionArgs> OnBankChangedAction;


    private void Start() {
        CloseCanvas();
        _title.text = LocalizationManager.Get("OrderResultsTitle");
    }

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("There can only be one instance of CurrencyManager");
            return;
        }
        Instance = this;
        
        
    }
    public class CurrencyActionArgs {
        public float countRewardsCoins;
        public int countRewardsGems;
    }
    
    public void SetOrderResult(float newCoins, int newGems, string orderTime, string accuracy, string combo) {
        ShowCanvas();
        UpdateCash(newCoins, newGems); // в окошке сверху визуал
        ShowRewardInfinity(newCoins, newGems); // визуал окна поздравления
        
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

    
    
    public void UpdateCash(float newCoins, int newGems) {
        Coins += newCoins;
        Gems += newGems;
        SoundManager.Instance.PlaySFX("NewMoney");
        OnBankChangedAction?.Invoke(new CurrencyActionArgs { countRewardsCoins = newCoins, countRewardsGems = newGems  });
    }
    

    public void CloseCanvas() {
        if (_canvas.activeSelf) {
            _canvas.SetActive(false);
        }
    }
    
    public void ShowCanvas() {
        _canvas.SetActive(true);
        _title.text = LocalizationManager.Get("OrderResultsTitle");
    }

    
    [SerializeField] private TMP_Text _coinsCountText;
    [SerializeField] private TMP_Text _gemsCountText;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _newCoinsText;
    [SerializeField] private TMP_Text _newGemsText;

    

    public void ShowRewardInfinity(float newCoins, int newGems) {
        _coinsCountText.text = Coins.ToString("0");
        _gemsCountText.text = Gems.ToString();
        
        _newCoinsText.enabled = true;
        _newGemsText.enabled = false;
        if (newCoins > 0) {
            _newCoinsText.color = Color.green;
            _newCoinsText.text = "+" + newCoins.ToString("0");
        }
        if (newCoins <= 0) {
            _newCoinsText.color = Color.red;
            _newCoinsText.text = "-" + newCoins.ToString("0");
        }
        
        if (newGems > 0) {
            _newGemsText.enabled = true;
            _newGemsText.color = Color.green;
            _newGemsText.text = "+" + newGems;
        }
        
        if (newGems < 0) {
            _newGemsText.enabled = true;
            _newGemsText.color = Color.red;
            _newGemsText.text = "-" + newGems;
        }
        
        
    }
}
