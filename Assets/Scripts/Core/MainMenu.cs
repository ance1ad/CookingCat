using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class MainMenu : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Button _playGame;
    [SerializeField] private Button _playTutorial;
    [SerializeField] private Image _level;
    [SerializeField] private GameObject _levelBackground;
    
    [SerializeField] private TMP_Text _playGameBtnTxt;
    [SerializeField] private TMP_Text _tutorialBtnTxt;

    
    
    
    public static MainMenu Instance { get; private set; }

    private void Awake() {
        if (Instance) {
            Debug.Log("MainMenu > 1");
            return;
        }
        Instance = this;
        _playGame.onClick.AddListener(StartGame);
        _playTutorial.onClick.AddListener(StartTutorial);
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
        _playGameBtnTxt.text = LocalizationManager.Get("PlayGame");
        _tutorialBtnTxt.text = LocalizationManager.Get("StartTutorial");
    }


    bool firstTime = true;
    private bool isLoad = false;
    private Coroutine loadingRoutine;
    private void StartGame() {
        _playTutorial.enabled = false;
        _playGame.enabled = false;
        
        MessageUI.Instance.HideInfinityText();
        TutorialManager.Instance.ResetAllElements(); // На всякий сбрасываем все
        StartCoroutine(FillLoadingLevel());
        StartCoroutine(FirstTimeCreateOrder());
        // Анимация загрузки
        
    }

    private IEnumerator FirstTimeCreateOrder() {
        yield return new WaitUntil(() => isLoad);

        if (!TutorialManager.Instance.tutorialPassed) {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("TutorialInvitation"), MessageUI.Emotions.happy);
            yield return new WaitForSeconds(4f);
            TutorialManager.Instance.StartTutorial();
            yield break;
        }
        else {
            if (CurrencyManager.Instance.Coins < 1000f) {
                CurrencyManager.Instance.UpdateCash(5000f, 5);
                MessageUI.Instance.SetText(LocalizationManager.Get("GladSeeYouReward"), MessageUI.Emotions.eated);
            }
            else {
                MessageUI.Instance.SetText(LocalizationManager.Get("GladSeeYou"), MessageUI.Emotions.eated);
            }  
        }
        
        yield return new WaitForSeconds(4f);
        if (firstTime) {
            firstTime = false;
            OrderManager.Instance.CreateNewOrder("MM");
        }
    }
    
    private IEnumerator WaitLoadingLevelToTutorial() {
        yield return new WaitUntil(() => isLoad);
        TutorialManager.Instance.StartTutorial();

    }
    
    
    private void StartTutorial() {
        _playTutorial.enabled = false;
        _playGame.enabled = false;

        StartCoroutine(FillLoadingLevel());
        StartCoroutine(WaitLoadingLevelToTutorial());
    }
    



    public void HideCloseMainMenu() {
        _canvas.SetActive(!_canvas.activeSelf); 
        Time.timeScale = _canvas.activeSelf ?  0 : 1;
    }

    public void OpenMainMenu() {
        if (_canvas.activeSelf) {
            return;
        }
        _canvas.SetActive(true); 
    }

    private void ChangeButtonsState(bool on) {
        _playTutorial.enabled = on;
        _playGame.enabled = on;
        _level.gameObject.SetActive(!on);
        _levelBackground.SetActive(!on);
    }


    private IEnumerator FillLoadingLevel() {
        ChangeButtonsState(false);
        
        
        isLoad = false;

        _level.fillAmount = 0;
        float timeToLoad = 25;
        float timer = 0;
        while (timer <= timeToLoad) {
            timer += Random.Range(0f, 0.1f);
            _level.fillAmount = timer / timeToLoad;
            yield return null;
        }

        ChangeButtonsState(true);
        HideCloseMainMenu();
        isLoad = true;
    }
}
