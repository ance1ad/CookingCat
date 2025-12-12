using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;


public class MainMenu : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _playGameButton;
    [SerializeField] private Button _playTutorialButton;
    [SerializeField] private Image _level;
    [SerializeField] private GameObject _levelBackground;
    
    [SerializeField] private TMP_Text _playGameButtonBtnTxt;
    [SerializeField] private TMP_Text _tutorialBtnTxt;

    private readonly float _timeToLoad = 5f;
    private bool _autoGraEnable;
    
    
    public static MainMenu Instance { get; private set; }

    private void Awake() {
        if (Instance) {
            Debug.Log("MainMenu count > 1");
            return;
        }
        Instance = this;
        Time.timeScale = 0;
        YG2.onGetSDKData += OnGetSDKData;
    }

    
    private bool dataIsLoaded = false;
    private void OnGetSDKData() {
        LocalizationManager.SetTextLocalization();
        _playGameButton.onClick.AddListener(StartGame);
        _playTutorialButton.onClick.AddListener(StartTutorial);
        dataIsLoaded = true;
        CheckServerTime();
    }


    private void CheckServerTime() {

        // Получить серверное время и сравнить со старым
        long currentServerTime = YG2.ServerTime();
        long lastServerTime = YG2.saves.lastPlayTime;
        
        if (lastServerTime == 0) {
            Debug.Log("Играет первый раз, фиксируем UTC: currentDateTime");
            YG2.saves.lastPlayTime = currentServerTime;
            YG2.SaveProgress();
            return;
        }
        // Понятное время
        DateTime currentDateTime = GetUTCTime(currentServerTime);
        DateTime lastDateTime = GetUTCTime(lastServerTime); 
        

        
        // Есть запись сравниваем иначе присваиваем
        TimeSpan timeSpan = currentDateTime - lastDateTime;
        if (timeSpan.TotalHours >= 12) {
            Debug.Log("С возвращением, держи награду!");
            // Награду вручить ...
            RewardManager.Instance.dailyReward = true;
            YG2.saves.lastPlayTime = currentServerTime;
            YG2.SaveProgress();
        }

    }

    private DateTime GetUTCTime(long unixTime) {
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        if (unixTime > 1000000000000) { // проверка, что это милисекунды
            return epoch.AddMilliseconds(unixTime);
        }
        return epoch.AddSeconds(unixTime);
    }


    private void Start() {
        
        _canvas.SetActive(true);
        _level.gameObject.SetActive(false);
        _levelBackground.SetActive(false);
        OnSwipeLanguage();
        SettingsManager.Instance.OnSwipeLanguage += OnSwipeLanguage;
    }

    private void OnSwipeLanguage() {
        _playGameButtonBtnTxt.text = LocalizationManager.Get("PlayGame");
        _tutorialBtnTxt.text = LocalizationManager.Get("StartTutorial");
    }


    private bool isLoad = false;
    private Coroutine loadingRoutine;
    private void StartGame() {
        Debug.Log("Включение рекламы");
        YGManager.Instance.ShowInterstitialAds();
        
        _playTutorialButton.enabled = false;
        _playGameButton.enabled = false;
        
        
        MessageUI.Instance.HideInfinityText();
        TutorialManager.Instance.ResetAllElements(); // На всякий сбрасываем все
        StartCoroutine(FillLoadingLevel());
        StartCoroutine(FirstTimeCreateOrder());
        // Анимация загрузки
        
    }

    private IEnumerator FirstTimeCreateOrder() {
        yield return new WaitUntil(() => isLoad);
        if (!_autoGraEnable) {
            _autoGraEnable = true;
            Debug.Log(_autoGraEnable);
            YG2.GameReadyAPI();
        }
        
        
        RewardManager.Instance.StartRewardTimerRoutine();
        if (!TutorialManager.Instance.TutorialPassed) {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("TutorialInvitation"), MessageUI.Emotions.happy);
            yield return new WaitForSeconds(3f);
            StartCoroutine(WaitLoadingLevelToTutorial());
            yield break;
        }
        
        
        if (RewardManager.Instance.dailyReward) {
            RewardManager.Instance.DailyReward();
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("GladSeeYouReward"), MessageUI.Emotions.eated);
        }
        else {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("GladSeeYou"), MessageUI.Emotions.eated);
        }  
        
        StartCoroutine(OrderManager.Instance.CreateFirstOrderLater(3f));
    }
    
    private IEnumerator WaitLoadingLevelToTutorial() {
        yield return new WaitUntil(() => isLoad);
        if (!_autoGraEnable) {
            _autoGraEnable = true;
            Debug.Log(_autoGraEnable);
            YG2.GameReadyAPI();
        }
        TutorialManager.Instance.StartTutorial();
    }
    
    
    private void StartTutorial() {
        YGManager.Instance.ShowInterstitialAds();

        
        _playTutorialButton.enabled = false;
        _playGameButton.enabled = false;

        StartCoroutine(FillLoadingLevel());
        StartCoroutine(WaitLoadingLevelToTutorial());
    }
    



    public void HideCloseMainMenu() {
        _canvas.SetActive(!_canvas.activeSelf); 
        Time.timeScale = _canvas.activeSelf ?  0 : 1;
        PlayerBankVisual.Instance.HideBank();
        // Сохранить продукты
    }

    public void OpenMainMenu() {
        PlayerBankVisual.Instance.HideBank();
        if (_canvas.activeSelf) {
            return;
        }
        _canvas.SetActive(true); 
        Debug.Log("Сохранение кол-ва продуктов при переходе в главное меню");
        ProductSaveManager.Instance.SaveProductsCount();
        
    }

    private void ChangeButtonsState(bool on) {
        _playTutorialButton.enabled = on;
        _playGameButton.enabled = on;
        _level.gameObject.SetActive(!on);
        _levelBackground.SetActive(!on);
    }

        
    private IEnumerator FillLoadingLevel() {
        ChangeButtonsState(false);
        yield return new WaitUntil(() => dataIsLoaded);

        _level.fillAmount = 0;
        float timer = 0;
        while (timer <= _timeToLoad) {
            timer += Random.Range(0f, 0.1f);
            _level.fillAmount = timer / _timeToLoad;
            yield return null;
        }

        isLoad = true;

        if (!_autoGraEnable) {
            _autoGraEnable = true;
            YG2.GameReadyAPI();
        }

        ChangeButtonsState(true);
        HideCloseMainMenu();
    }

}
