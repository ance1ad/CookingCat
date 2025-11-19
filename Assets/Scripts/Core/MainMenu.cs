using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class MainMenu : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _playGameButton;
    [SerializeField] private Button _playTutorialButton;
    [SerializeField] private Image _level;
    [SerializeField] private GameObject _levelBackground;
    
    [SerializeField] private TMP_Text _playGameButtonBtnTxt;
    [SerializeField] private TMP_Text _tutorialBtnTxt;

    private readonly float _timeToLoad = 4f;
    
    
    
    public static MainMenu Instance { get; private set; }

    private void Awake() {
        if (Instance) {
            Debug.Log("MainMenu > 1");
            return;
        }
        Instance = this;
        _playGameButton.onClick.AddListener(StartGame);
        _playTutorialButton.onClick.AddListener(StartTutorial);
        Time.timeScale = 0;

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


    bool firstTime = true;
    private bool isLoad = false;
    private Coroutine loadingRoutine;
    private void StartGame() {
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

        RewardManager.Instance.StartRewardTimerRoutine();
        if (!TutorialManager.Instance.TutorialPassed) {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("TutorialInvitation"), MessageUI.Emotions.happy);
            yield return new WaitForSeconds(3f);
            StartCoroutine(WaitLoadingLevelToTutorial());
            yield break;
        }
        
        
        if (CurrencyManager.Instance.Coins < 1000f) {
            CurrencyManager.Instance.UpdateCash(5000f, 5);
            MessageUI.Instance.SetText(LocalizationManager.Get("GladSeeYouReward"), MessageUI.Emotions.eated);
        }
        else {
            MessageUI.Instance.SetText(LocalizationManager.Get("GladSeeYou"), MessageUI.Emotions.eated);
        }  
        
        StartCoroutine(OrderManager.Instance.CreateFirstOrderLater(3f));
    }
    
    private IEnumerator WaitLoadingLevelToTutorial() {
        yield return new WaitUntil(() => isLoad);
        TutorialManager.Instance.StartTutorial();

    }
    
    
    private void StartTutorial() {
        _playTutorialButton.enabled = false;
        _playGameButton.enabled = false;

        StartCoroutine(FillLoadingLevel());
        StartCoroutine(WaitLoadingLevelToTutorial());
    }
    



    public void HideCloseMainMenu() {
        _canvas.SetActive(!_canvas.activeSelf); 
        Time.timeScale = _canvas.activeSelf ?  0 : 1;
        PlayerBankVisual.Instance.HideBank();
    }

    public void OpenMainMenu() {
        if (_canvas.activeSelf) {
            return;
        }
        _canvas.SetActive(true); 
    }

    private void ChangeButtonsState(bool on) {
        _playTutorialButton.enabled = on;
        _playGameButton.enabled = on;
        _level.gameObject.SetActive(!on);
        _levelBackground.SetActive(!on);
    }

        
    private IEnumerator FillLoadingLevel() {
        ChangeButtonsState(false);
        
        
        isLoad = false;

        _level.fillAmount = 0;
        float timer = 0;
        while (timer <= _timeToLoad) {
            timer += Random.Range(0f, 0.1f);
            _level.fillAmount = timer / _timeToLoad;
            yield return null;
        }

        ChangeButtonsState(true);
        HideCloseMainMenu();
        isLoad = true;
    }
}
