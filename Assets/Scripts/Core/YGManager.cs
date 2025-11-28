using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using YG;


public class YGManager : MonoBehaviour {
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private GameObject _canvas;
    public static YGManager Instance { get; private set; }
    private float _advCooldown = 300f; // каждые 5 минут реклама при 
    
    private DateTime _lastTimeShowAdv;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("Duplicate YGManager instantiated");
            return;
        }
        Instance = this;
    }

    private void Start() {
        SetCanvasActive(false);
        _lastTimeShowAdv = DateTime.Now;
        _advCooldown = 60f;
    }
     
    
    
    public void ShowInterstitialWarningAds() {
        if (CalculateTimeToShow(_advCooldown)) {
            StartCoroutine(AdvTimer());
        }
    } 
    
    public void ShowInterstitialAds() {
        if (CalculateTimeToShow(_advCooldown)) {
            Debug.Log("ShowInterstitialAds");
            YG2.InterstitialAdvShow();
        }

    }
    
    

    private IEnumerator AdvTimer() {
        SetCanvasActive(true);
        float currentTime = 3f;
        while(currentTime >= 0) {
            currentTime -= Time.deltaTime;
            _timer.text = (LocalizationManager.Get("AdvTimer", currentTime.ToString("F1")));
            yield return null;
        }
        SetCanvasActive(false);
        YG2.InterstitialAdvShow();
    }

    private void SetCanvasActive(bool active) {
        _canvas.SetActive(active);
    }


    private bool CalculateTimeToShow(float time) {
        TimeSpan timeDifference = DateTime.Now - _lastTimeShowAdv;
        Debug.Log(timeDifference.TotalSeconds);
        Debug.Log(_advCooldown);
        
        if (timeDifference.TotalSeconds < _advCooldown) {
            Debug.LogWarning("Reward time has not expired");
            return false;
        }
        _lastTimeShowAdv = DateTime.Now;
        return true;
    }


    public void ShowRewardAdv(string id ,System.Action OnRewardActions = null) {
        YG2.RewardedAdvShow(id, () => {
            OnRewardActions?.Invoke();
        });
    }
    
}
