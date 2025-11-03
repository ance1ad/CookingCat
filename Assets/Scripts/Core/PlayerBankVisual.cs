using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerBankVisual : MonoBehaviour {
    [SerializeField] private TMP_Text _coinsCount;
    [SerializeField] private TMP_Text _coinsReward;
    [SerializeField] private TMP_Text _gemsCount;
    [SerializeField] private TMP_Text _gemsReward;

    private void Start() {
        CurrencyManager.Instance.OnBankChangedAction += OnOnBankChangedAction;
        UpdateBank();
    }

    private void OnOnBankChangedAction(CurrencyManager.CurrencyActionArgs obj) {
        UpdateBank();
        
        // Дальше показываем визуалом больше меньшэ
        Debug.Log("countRewardsCoins" + obj.countRewardsCoins);
        Debug.Log("countRewardsGems" + obj.countRewardsGems);
        
        Debug.Log("Обновление countRewardsCoins и тп");
        if (obj.countRewardsCoins > 0) {
            StartCoroutine(UpdateCoinsCountRoutine());
            _coinsReward.color = Color.green;
            _coinsReward.text = "+" + obj.countRewardsCoins.ToString("0");
        }
        else if (obj.countRewardsCoins < 0) {
            _coinsReward.color = Color.red;
            StartCoroutine(UpdateCoinsCountRoutine());
            _coinsReward.text = obj.countRewardsCoins.ToString("0");
        }
        
        if (obj.countRewardsGems > 0) {
            _gemsReward.color = Color.green;
            StartCoroutine(UpdateGemsCountRoutine());
            _gemsReward.text = "+" + obj.countRewardsGems;
        }
        
        else if (obj.countRewardsGems < 0) {
            _gemsReward.color = Color.red;
            StartCoroutine(UpdateGemsCountRoutine());
            _gemsReward.text = obj.countRewardsGems.ToString();
        }
    }


    private IEnumerator UpdateCoinsCountRoutine() {
        yield return StartCoroutine(FadeTextRoutine(_coinsReward, 0.5f, true));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeTextRoutine(_coinsReward, 0.5f, false));
    }
    
    private IEnumerator UpdateGemsCountRoutine() {
        yield return StartCoroutine(FadeTextRoutine(_gemsReward, 0.5f, true));
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(FadeTextRoutine(_gemsReward, 0.5f, false)); 

    }
    
    
    private IEnumerator FadeTextRoutine(TMP_Text text, float duration, bool fadeIn)
    {
        Color startColor = text.color;
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;

        text.color = new Color(startColor.r, startColor.g, startColor.b, startAlpha);
        text.enabled = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, normalized);
            text.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        text.color = new Color(startColor.r, startColor.g, startColor.b, endAlpha);

        if (!fadeIn)
            text.enabled = false;
    }

    
    public void UpdateBank() {
        Debug.Log("Обновление банка");
        _coinsReward.enabled = false;
        _gemsReward.enabled = false;
        _coinsCount.text = CurrencyManager.Instance.Coins.ToString("0");
        _gemsCount.text = CurrencyManager.Instance.Gems.ToString();
    }
}
