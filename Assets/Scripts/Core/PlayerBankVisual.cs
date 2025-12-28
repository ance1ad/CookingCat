using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;

public class PlayerBankVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _coinsCount;
    [SerializeField] private TMP_Text _coinsReward;
    [SerializeField] private TMP_Text _gemsCount;
    [SerializeField] private TMP_Text _gemsReward;
    [SerializeField] private GameObject _playerBank;

    private bool _shopOpen;
    
    
    private void Awake() {
        if (Instance != null) {
            // Debug.LogError("There can only be one instance of PlayerBankVisual");
            return;
        }
        Instance = this;
        KitchenEvents.OnShopOpen += ShowBank;
        KitchenEvents.OnShopClose += HideBank;
    }



    private void Start() {
        CurrencyManager.Instance.OnBankChangedAction += OnBankChangedAction;
        UpdateBank();
    }
    
    public static PlayerBankVisual Instance { get; private set; }

    public void ShowBank() {
        if (_playerBank != null) {
            _shopOpen = true;
            _playerBank.SetActive(true);
        }
    } 
    
    
    public void HideBank() {
        
        if (_playerBank != null && _playerBank.activeSelf) {
            _shopOpen = false;
            _playerBank.SetActive(false);
        }
    } 
    


    private Coroutine _currentCoinsRoutine;
    private Coroutine _currentGemsRoutine;
    private void OnBankChangedAction(CurrencyManager.CurrencyActionArgs obj) {
        UpdateBank();
        
        // Дальше показываем визуалом больше меньшэ
        // Debug.Log("countRewardsCoins" + obj.countRewardsCoins);
        // Debug.Log("countRewardsGems" + obj.countRewardsGems);
        
        // Debug.Log("Обновление countRewardsCoins и тп");
        StopAllCoroutines();
        
        if (obj.countRewardsCoins > 0) {
            _coinsReward.color = Color.green;
            _coinsReward.text = "+" + obj.countRewardsCoins.ToString("0");
            StartCoroutine(UpdateCoinsCountRoutine());
        }
        else if (obj.countRewardsCoins < 0) {
            _coinsReward.color = Color.red;
            _coinsReward.text = obj.countRewardsCoins.ToString("0");
            StartCoroutine(UpdateCoinsCountRoutine());
        }
        
        if (obj.countRewardsGems > 0) {
            _gemsReward.color = Color.green;
            _gemsReward.text = "+" + obj.countRewardsGems;
            _currentGemsRoutine = StartCoroutine(UpdateGemsCountRoutine());
        }
        
        else if (obj.countRewardsGems < 0) {
            _gemsReward.color = Color.red;
            _gemsReward.text = obj.countRewardsGems.ToString();
            _currentGemsRoutine = StartCoroutine(UpdateGemsCountRoutine());
        }
    }


    private IEnumerator UpdateCoinsCountRoutine() {
        yield return StartCoroutine(FadeTextRoutine(_coinsReward, 0.5f, true));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeTextRoutine(_coinsReward, 0.5f, false));
        if (!_shopOpen) {
            HideBank();
        }
    }
    
    private IEnumerator UpdateGemsCountRoutine() {
        yield return StartCoroutine(FadeTextRoutine(_gemsReward, 0.5f, true));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeTextRoutine(_gemsReward, 0.5f, false));
        if (!_shopOpen) {
            HideBank();
        }
    }
    
    
    private IEnumerator FadeTextRoutine(TMP_Text text, float duration, bool fadeIn) {
        Color baseColor = text.color;
        baseColor.a = 1f;
        
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        text.enabled = true;
        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, startAlpha);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, normalized);
            text.color = new Color(baseColor.r, baseColor.g, baseColor.b, newAlpha);
            yield return null;
        }

        text.color = new Color(baseColor.r, baseColor.g, baseColor.b, endAlpha);

        if (!fadeIn)
            text.enabled = false;
    }

    
    public void UpdateBank() {
        ShowBank();
        _coinsReward.enabled = false;
        _gemsReward.enabled = false;
        _coinsCount.text = FormatPrice(CurrencyManager.Instance.Coins);
        _gemsCount.text = CurrencyManager.Instance.Gems.ToString();
    }
    
    private string FormatPrice(float price) {
        return $"{price / 1000f:0.#}k";
    }
    
}
