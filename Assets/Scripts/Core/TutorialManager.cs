using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.CheckIn.CodeReview;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
    [SerializeField] private ThiefCat _thiefCat;
    
    // Для тутора
    [SerializeField] private List<BaseCounter> _allCounters;
    
    [SerializeField] private List<BaseCounter> _clearCounters;
    [SerializeField] private List<BaseCounter> _containerCounters;
    [SerializeField] private List<BaseCounter> _cuttingCounters;
    [SerializeField] private BaseCounter _ovenCounter;
    [SerializeField] private BaseCounter _stoveCounter;
    [SerializeField] private BaseCounter _trashCounters;
    [SerializeField] private BaseCounter _juicerCounters;
    [SerializeField] private BaseCounter _platesCounter;
    [SerializeField] private BaseCounter _orderCounter;
    
    
    [SerializeField] private Button _skinStoreButton;
    [SerializeField] private Button _productStoreButton;
    
    
    
    [SerializeField] private Button _nextButton;
    [SerializeField] private GameObject _focus;
    
    
    // Arrows
    [SerializeField] private GameObject _skinStoreArrow;
    [SerializeField] private GameObject _productStoreArrow;
    [SerializeField] private GameObject _fButtonArrow;
    [SerializeField] private GameObject _eButtonArrow;
    [SerializeField] private GameObject _orderResourceArrow; // Время и кол-во подсмотроу



    
    private bool readyToNext = false;
    // Запоминание текста и эмодзи при смене языка
    private MessageUI.Emotions lastEmotion;
    private string lastStepKey;
    
    public static TutorialManager Instance {get; private set;}
    public bool TutorialStarted = false;
    private void Start() {
        _nextButton.gameObject.SetActive(false);
    }
    
    public bool tutorialPassed = false;
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("TutorialManager: Instance already set");
            return;
        }
        Instance = this;
        HideArrows();
        
    }

    private void SettingsManagerOnOnSwipeLanguage() {
        if (!string.IsNullOrEmpty(lastStepKey)) {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get(lastStepKey),lastEmotion);
        }
    }

    private void ClickNextButton() {
        readyToNext = true;
    }


    public void StartTutorial() {
        StopAllCoroutines();
        _nextButton.onClick.AddListener(ClickNextButton);
        SettingsManager.Instance.OnSwipeLanguage += SettingsManagerOnOnSwipeLanguage;
        HideArrows();
        TutorialStarted = true;
        _thiefCat.StopThiefCycle();
        StartCoroutine(TutorialRoutine());
    }

    private void ReadyToTutorial() {
        Player.Instance.StopWalking();
        _nextButton.gameObject.SetActive(true);
        CurrencyManager.Instance.CloseCanvas();
        _focus.SetActive(true);
        
        // Скрыть кнопки покашто, порционно показываем
        _skinStoreButton.gameObject.SetActive(false);
        _productStoreButton.gameObject.SetActive(false);
    }


    private IEnumerator TutorialRoutine() {

        ReadyToTutorial();

        yield return ShowStep("Step1Hello", MessageUI.Emotions.happy);
        ShowArrows(_eButtonArrow, _fButtonArrow);

        yield return ShowStep("Step2Buttons", MessageUI.Emotions.defaultFace);
        ShowArrows(_eButtonArrow);

        yield return ShowStep("Step3RightButton", MessageUI.Emotions.eated);
        ShowArrows(_fButtonArrow);

        yield return ShowStep("Step4LeftButton", MessageUI.Emotions.happy);
        _focus.SetActive(false);

        HideArrows();
        yield return ShowStep("Step5IntroObjects", MessageUI.Emotions.eated);
        HideAllCounters();

        yield return ShowStep("Step6Containers", MessageUI.Emotions.happy, () => ShowCountersGroup(_containerCounters));

        yield return ShowStep("Step7Trash", MessageUI.Emotions.sad, () =>
        {
            HideHighLight(_containerCounters);
            _trashCounters.gameObject.SetActive(true);
            _trashCounters.SetHighlight(true);
        });

        yield return ShowStep("Step8Tables", MessageUI.Emotions.defaultFace, () =>
        {
            _trashCounters.SetHighlight(false);
            ShowCountersGroup(_clearCounters);
        });

        yield return ShowStep("Step9Cutting", MessageUI.Emotions.happy, () =>
        {
            ShowArrows(_fButtonArrow);
            HideHighLight(_clearCounters);
            ShowCountersGroup(_cuttingCounters);
        });

        yield return ShowStep("Step10Stove", MessageUI.Emotions.eated, () =>
        {
            HideArrows();
            HideHighLight(_cuttingCounters);
            _stoveCounter.gameObject.SetActive(true);
            _stoveCounter.SetHighlight(true);
        });

        yield return ShowStep("Step11Oven", MessageUI.Emotions.happy, () =>
        {
            _ovenCounter.gameObject.SetActive(true);
            _ovenCounter.SetHighlight(true);
            _stoveCounter.SetHighlight(false);
        });

        yield return ShowStep("Step12OvenUse", MessageUI.Emotions.eated, () => ShowArrows(_fButtonArrow));

        yield return ShowStep("Step13JuicerPrep", MessageUI.Emotions.happy, () =>
        {
            HideArrows();
            _juicerCounters.gameObject.SetActive(true);
            _juicerCounters.SetHighlight(true);
            _ovenCounter.SetHighlight(false);
        });

        yield return ShowStep("Step14JuicerUse", MessageUI.Emotions.eated);

        yield return ShowStep("Step15Delivery", MessageUI.Emotions.defaultFace, () =>
        {
            ShowArrows(_productStoreArrow);
            _focus.SetActive(true);
            _productStoreButton.gameObject.SetActive(true);
            _productStoreButton.enabled = false;
            _juicerCounters.SetHighlight(false);
        });

        yield return ShowStep("Step16OrderStart", MessageUI.Emotions.happy, () =>
        {
            HideArrows();
            _focus.SetActive(false);
            ClientCat.Instance.gameObject.SetActive(true);
            _platesCounter.gameObject.SetActive(true);
            _orderCounter.gameObject.SetActive(true);
            _platesCounter.SetHighlight(true);
            _orderCounter.SetHighlight(true);
        });

        yield return ShowStep("Step17OrderHint", MessageUI.Emotions.eated);
        yield return ShowStep("Step18Timer", MessageUI.Emotions.defaultFace);
        yield return ShowStep("Step19Burger", MessageUI.Emotions.eated);
        yield return ShowStep("Step20Pizza", MessageUI.Emotions.happy);
        yield return ShowStep("Step21JuiceReady", MessageUI.Emotions.eated);

        yield return ShowStep("Step22Serve", MessageUI.Emotions.happy, () =>
        {
            _orderCounter.SetHighlight(false);
            _platesCounter.SetHighlight(false);
        });

        yield return ShowStep("Step23Shop", MessageUI.Emotions.eated, () =>
        {
            ShowArrows(_skinStoreArrow);
            _skinStoreButton.gameObject.SetActive(true);
            _skinStoreButton.enabled = false;
            _focus.SetActive(true);
        });

        yield return ShowStep("Step24Bonk", MessageUI.Emotions.bonk, () =>
        {
            HideArrows();
            SoundManager.Instance.PlaySFX("Bonk");
        });

        yield return ShowStep("Step25Thief", MessageUI.Emotions.shocked);
        yield return ShowStep("Step26GoodLuck", MessageUI.Emotions.happy, () =>
        {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("Step26GoodLuck"), MessageUI.Emotions.happy);
            if (CurrencyManager.Instance.Coins < 1000f) {
                CurrencyManager.Instance.UpdateCash(5000f, 5);
            }
            CloseTutorial();
            tutorialPassed = true;
            StartCoroutine(CreateFirstOrderLater());
        });
    }
    

    private IEnumerator ShowStep(string stepKey, MessageUI.Emotions emotion, System.Action onStepStart = null) {
        lastStepKey = stepKey;
        lastEmotion = emotion;

        onStepStart?.Invoke();

        MessageUI.Instance.SetTextInfinity(LocalizationManager.Get(stepKey), emotion);

        yield return new WaitUntil(() => readyToNext);
        readyToNext = false;
    }



    private void HideHighLight(List<BaseCounter> counters) {
        foreach (var counter in counters) {
            counter.SetHighlight(false);
        }
        
    }


    private void HideAllCounters() {
        ClientCat.Instance.gameObject.SetActive(false);
        foreach (var counter in _allCounters) {
            counter.gameObject.SetActive(false);
        }
    }
    
        
    
    // Методы шоб показать конкретную группу
    public void ShowCountersGroup(List<BaseCounter> countersList) {
        foreach (var counter in countersList) {
            counter.gameObject.SetActive(true);
            counter.SetHighlight(true);
        }
    }

    
    public void CloseTutorial() {
        lastStepKey = "";
        TutorialStarted = false;
        _skinStoreButton.enabled = true;
        _productStoreButton.enabled = true;
        StartCoroutine(MessageUI.Instance.HideFocusRoutine());
        Player.Instance._stopWalking = false;
        RewardManager.Instance.StartRewardTimerRoutine();
    }

    private void ShowArrows(params GameObject[] arrows) {
        HideArrows();
        foreach (var arrow in arrows) {
            arrow.SetActive(true);
        }
    }
    
    public void HideArrows() {
        _skinStoreArrow.SetActive(false);
        _productStoreArrow.SetActive(false);
        _fButtonArrow.SetActive(false);
        _eButtonArrow.SetActive(false);
        _orderResourceArrow.SetActive(false);
    }

    public void ShowOrderResource() {
        _orderResourceArrow.SetActive(true);
        StartCoroutine(ShowOrderResourceRoutine());
    }

    private IEnumerator ShowOrderResourceRoutine() {
        yield return new WaitForSeconds(3f);
        _orderResourceArrow.SetActive(false);
    }

    public void ResetAllElements() {
        HideArrows();
        _skinStoreButton.enabled = true;
        _productStoreButton.enabled = true;
        
        StopAllCoroutines();
        Player.Instance._stopWalking = false;
        // Buttons
        if (!_skinStoreButton.gameObject.activeSelf) {
            _skinStoreButton.gameObject.SetActive(true);    
        }
        if (!_productStoreButton.gameObject.activeSelf) {
            _productStoreButton.gameObject.SetActive(true);    
        }
        if (_nextButton.gameObject.activeSelf) {
            _nextButton.gameObject.SetActive(false);
        }
        if (_focus.gameObject.activeSelf) { 
            _focus.gameObject.SetActive(false);
        }
        // Counters
        foreach (var counter in _allCounters) {
            if (!counter.gameObject.activeSelf) {
                counter.gameObject.SetActive(true);
            }
            counter.SetHighlight(false);
        }

        if (!ClientCat.Instance.gameObject.activeSelf) {
            ClientCat.Instance.gameObject.SetActive(true);
        }
        TutorialStarted = false;
    }
    
    private IEnumerator CreateFirstOrderLater() {
        yield return new WaitForSeconds(3f);
        OrderManager.Instance.CreateNewOrder("TM");
    }
    

}
