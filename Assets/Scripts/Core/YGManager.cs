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
    private float _advCooldown = 240f; // каждые 5 минут реклама при 
    
    private DateTime _lastTimeShowAdv;

    private void Awake() {
        if (Instance != null) {
            // Debug.LogWarning("Duplicate YGManager instantiated");
            return;
        }
        Instance = this;
    }

    private void Start() {
        SetCanvasActive(false);
        _lastTimeShowAdv = DateTime.Now.AddSeconds(-_advCooldown);
        _advCooldown = 200f; // 4 minutes
        YG2.onOpenAnyAdv += OpenAnyAdv;
        YG2.onCloseAnyAdv += CloseAnyAdv;
    }
     
    
    
    public void ShowInterstitialWarningAds() {
        if (CalculateTimeToShow(_advCooldown)) {
            StartCoroutine(AdvTimer());
        }
    } 
    
    public void ShowInterstitialAds() {
        if (CalculateTimeToShow(_advCooldown)) {
            // // Debug.Log("ShowInterstitialAds");
            YG2.InterstitialAdvShow();
        }

    }
    

    private void OnDisable() {
        YG2.onOpenAnyAdv -= OpenAnyAdv;
        YG2.onCloseAnyAdv -= CloseAnyAdv;
    }
    
    private void OpenAnyAdv() {
        // // Debug.Log("OpenAnyAdv");
        SetCanvasActive(false);
    }
    
    private void CloseAnyAdv() {
        // // Debug.Log("CloseAnyAdv");
        Player.Instance.StartWalking();
        StartCoroutine(StartGameAfterAdvRoutine());
    }

    private IEnumerator StartGameAfterAdvRoutine() {
        yield return null;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }


    private IEnumerator AdvTimer() {
        // // Debug.Log("Пауза в AdvTimer");
        AudioListener.pause = true;
        Time.timeScale = 0;
        SetCanvasActive(true);
        float currentTime = 2f;
        while(currentTime > 0) {
            currentTime -= Time.unscaledDeltaTime;
            _timer.text = (LocalizationManager.Get("AdvTimer", currentTime.ToString("F1")));
            if (currentTime < 0) {
                break;
            }
            yield return null;
        }
        // Debug.Log("Конец таймера, вызов рекламы");
        Player.Instance.StopWalking();
        YG2.InterstitialAdvShow();
    }

    private void SetCanvasActive(bool active) {
        _canvas.SetActive(active);
    }


    private bool CalculateTimeToShow(float time) {
        TimeSpan timeDifference = DateTime.Now - _lastTimeShowAdv;
        // // Debug.Log(timeDifference.TotalSeconds);
        // // Debug.Log(_advCooldown);
        
        if (timeDifference.TotalSeconds < _advCooldown) {
            // // Debug.LogWarning("Reward time has not expired");
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
