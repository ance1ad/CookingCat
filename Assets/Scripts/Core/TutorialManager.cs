using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
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
    [SerializeField] private BaseCounter _trashCounter;
    [SerializeField] private BaseCounter _juicerCounter;
    [SerializeField] private BaseCounter _platesCounter;
    [SerializeField] private BaseCounter _orderCounter;


    [SerializeField] private BaseCounter _fridgeBun;
    [SerializeField] private BaseCounter _fridgeMeat;
    [SerializeField] private BaseCounter _fridgeTomato;
    
    
    
    [SerializeField] private Button _skinStoreButton;
    [SerializeField] private Button _productStoreButton;
    
    
    
    [SerializeField] private GameObject _focus;
    
    
    // Arrows
    [SerializeField] private GameObject _skinStoreArrow;
    [SerializeField] private GameObject _productStoreArrow;
    [SerializeField] private GameObject _fButtonArrow;
    [SerializeField] private GameObject _eButtonArrow;
    [SerializeField] private GameObject _orderResourceArrow; // Время и кол-во подсмотроу

    
    // Продукты
    [SerializeField] private KitchenObjectSO _bunKO; // Булочко
    [SerializeField] private KitchenObjectSO _tomatoSliced; // Помидорко
    [SerializeField] private KitchenObjectSO _fryedMeat; // Мясочкоу
     
    // Флаги разрешения
    // AllowCompleteOrder - в OrderManager






    private bool readyToNext = false;
    // Запоминание текста и эмодзи при смене языка
    private MessageUI.Emotions lastEmotion;
    private string lastStepKey;
    
    public static TutorialManager Instance {get; private set;}
    public bool TutorialStarted = false;
    
    public bool TutorialPassed = false;
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("TutorialManager: Instance already set");
            return;
        }
        Instance = this;
        HideArrows();
        
    }

    private void Start() {
        MessageUI.Instance.OnButtonClick += OnClickNextButton;
    }
    

    private void SettingsManagerOnOnSwipeLanguage() {
        if (!string.IsNullOrEmpty(lastStepKey)) {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get(lastStepKey),lastEmotion);
        }
    }

    private void OnClickNextButton() {
        Debug.Log("OnClickNextButton");
        if (TutorialStarted) {
            readyToNext = true;
        }
    }


    public void StartTutorial() {
        RewardManager.Instance.StopRewardTimerRoutine();
        StopAllCoroutines();
        SettingsManager.Instance.OnSwipeLanguage += SettingsManagerOnOnSwipeLanguage;
        HideArrows();
        TutorialStarted = true;
        _thiefCat.StopThiefCycle();
        StartCoroutine(TutorialRoutine());
    }

    private void ReadyToTutorial() {
        Player.Instance.StopWalking();
        CurrencyManager.Instance.CloseCanvas();
        _focus.SetActive(true);
        
        // Скрыть кнопки покашто, порционно показываем
        _skinStoreButton.gameObject.SetActive(false);
        _productStoreButton.gameObject.SetActive(false);
    }


    private IEnumerator TutorialRoutine() {

        ReadyToTutorial();

        yield return ShowStep("StepHello", MessageUI.Emotions.happy);
        ShowArrows(_eButtonArrow, _fButtonArrow);

        yield return ShowStep("StepButtons", MessageUI.Emotions.defaultFace);
        ShowArrows(_eButtonArrow);

        yield return ShowStep("StepRightButton", MessageUI.Emotions.eated);
        ShowArrows(_fButtonArrow);

        yield return ShowStep("StepLeftButton", MessageUI.Emotions.happy);
        _focus.SetActive(false);
        HideArrows();


        // РАБОТАЮ ТУТ



        // Поздравляет о взятом заказе
        yield return Step_TakeFirstOrder();
        yield return Step_PutPlate();
        yield return Step_BunPut();
        yield return Step_TomatoSlice();
        yield return Step_Meatfry();
        yield return Step_FirstOrderComplete();
        // Pizza
        yield return Step_PizzaDoingReady();
        yield return Step_HowOvenWork();
        yield return Step_PeekIngredients();
        yield return Step_CountIngredientsInOven();
        yield return Step_TakePizza();
        yield return Step_CompletePizzaOrder();
        // Juicer
        yield return Step_TakeDrinkOrder();
        yield return Step_JuicerWork();
        yield return Step_JuicerWorking();
        yield return Step_JuiceReady();
        MessageUI.Instance.ShowNextButton();
        yield return ShowStep("StepBonk", MessageUI.Emotions.bonk);
        yield return ShowStep("StepThief", MessageUI.Emotions.shocked);
        yield return ShowStep("StepGoodLuck", MessageUI.Emotions.happy, () => {
            if (CurrencyManager.Instance.Coins < 1000f) {
                CurrencyManager.Instance.UpdateCash(5000f, 5);
                CloseTutorial();
            }
        });
            


        
        
        // yield return ShowStep("StepGoodLuck", MessageUI.Emotions.happy, () =>
        // {
        //     MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("StepGoodLuck"), MessageUI.Emotions.happy);
        //     if (CurrencyManager.Instance.Coins < 1000f) {
        //         CurrencyManager.Instance.UpdateCash(5000f, 5);
        //     }
        //     CloseTutorial();
        //     tutorialPassed = true;
        //     StartCoroutine(OrderManager.Instance.CreateFirstOrderLater(3f));
        // });
    }
    

    private IEnumerator ShowStep(string stepKey, MessageUI.Emotions emotion, System.Action onStepStart = null) {
        lastStepKey = stepKey;
        lastEmotion = emotion;

        onStepStart?.Invoke();

        MessageUI.Instance.SetTextInfinity(LocalizationManager.Get(stepKey), emotion);

        yield return new WaitUntil(() => readyToNext);
        readyToNext = false;
    }

    private IEnumerator Step_TakeFirstOrder() {
        Player.Instance.StartWalking();
        SetHighlightOneCounter(_orderCounter);
        TutorialOrderCounter.Instance.CreateTutorialBurger();
        MessageUI.Instance.SetTextInfinity(LocalizationManager.Get("StepFirstOrder"), MessageUI.Emotions.eated);
        MessageUI.Instance.ShowPlatesArrow();
        MessageUI.Instance.HideNextButton();
        
        readyToNext = false;
        KitchenEvents.OnOrderTake += Handler;
        
        void Handler()
        {
            if (!TutorialStarted) return;
            HideHighlightOneCounter(_orderCounter);
            KitchenEvents.OnOrderTake-=Handler;
            readyToNext = true;
        }
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_PutPlate() {
        readyToNext = false;
        KitchenEvents.OnObjectPutInTable += Handler;
        SetHighlightOneCounter(_platesCounter);
        SetHighlightCountersGroup(_clearCounters);
            
        MessageUI.Instance.SetTextInfinity("Отлично, ты взял первый заказ, хватай поднос и клади его на стол", MessageUI.Emotions.happy);
        void Handler(KitchenObject obj) {
            if (!TutorialStarted) return;
            if (obj is not Plate) {
                return;
            }

            HideHighlightOneCounter(_platesCounter);
            HideHighlightCountersGroup(_clearCounters);
            
            KitchenEvents.OnObjectPutInTable -= Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    private IEnumerator Step_BunPut() {
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Давай начнём с булки, бери ее из холодильника и клади на поднос", MessageUI.Emotions.defaultFace);
        SetHighlightOneCounter(_fridgeBun);
        
        KitchenEvents.OnIngredientAddedOnPlate += Handler;
        void Handler(KitchenObjectSO obj)
        {
            if (!TutorialStarted) return;
            if (obj != _bunKO) {
                return; 
            }
            HideHighlightOneCounter(_fridgeBun);
            KitchenEvents.OnIngredientAddedOnPlate-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    private IEnumerator Step_TomatoSlice() {
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Теперь давай научимся нарезать продукты, нарежь помидор и положи на поднос", MessageUI.Emotions.happy);
        SetHighlightCountersGroup(_cuttingCounters);
        SetHighlightOneCounter(_fridgeTomato);
        KitchenEvents.OnIngredientAddedOnPlate += Handler;
        
        
        void Handler(KitchenObjectSO obj)
        {
            if (!TutorialStarted) return;
            if (obj != _tomatoSliced) {
                return; 
            }
            HideHighlightCountersGroup(_cuttingCounters);
            HideHighlightOneCounter(_fridgeTomato);

            KitchenEvents.OnIngredientAddedOnPlate-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    private IEnumerator Step_Meatfry() {
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Теперь давай научимся жарить мясо, смотри не пережарь!", MessageUI.Emotions.eated);
        MessageUI.Instance.ShowPlatesArrow();
        MessageUI.Instance.ShowStoveArrow();
        
        HideHighlightOneCounter(_trashCounter);
        SetHighlightOneCounter(_stoveCounter);
        SetHighlightOneCounter(_fridgeMeat);
        KitchenEvents.OnIngredientAddedOnPlate += Handler;
        
        
        void Handler(KitchenObjectSO obj)
        {
            if (!TutorialStarted) return;
            if (obj != _fryedMeat) {
                return; 
            }
            HideHighlightOneCounter(_stoveCounter);
            HideHighlightOneCounter(_fridgeMeat);

            KitchenEvents.OnIngredientAddedOnPlate-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_FirstOrderComplete() {
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Отлично! Бургер готов, скорей неси клиенту!", MessageUI.Emotions.happy);
        OrderManager.Instance.AllowCompleteOrder = true;
        SetHighlightOneCounter(_orderCounter);
        KitchenEvents.OnOrderCompleted += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            KitchenEvents.OnOrderCompleted-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_PizzaDoingReady() {
        // УБЕРИ!
        MessageUI.Instance.HideNextButton();
        Player.Instance.StartWalking();
        OrderManager.Instance.AllowCompleteOrder = false;
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Теперь научимся делать пиццу, принимай новый заказ!", MessageUI.Emotions.eated);
        TutorialOrderCounter.Instance.CreateTutorialPizza();
        
        SetHighlightOneCounter(_orderCounter);
        KitchenEvents.OnOrderTake += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            HideHighlightOneCounter(_stoveCounter);
            HideHighlightOneCounter(_fridgeMeat);

            KitchenEvents.OnOrderTake-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    
    
    
    // Давай собирать пиццу, все ингредиенты кладуться в духовку
    private IEnumerator Step_HowOvenWork() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Пицца собирается сразу в духовке, готовь ингредиенты и клади в духовку!", MessageUI.Emotions.happy);
        SetHighlightOneCounter(_ovenCounter);
        KitchenEvents.OnOvenIngredientAdded += Handler;
        
        
        void Handler(int count)
        {
            if (!TutorialStarted) return;
            SetHighlightOneCounter(_ovenCounter);
            KitchenEvents.OnOvenIngredientAdded-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_PeekIngredients() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Отлично! Продолжай собирать пиццу, тут можно подсмотреть ингредиенты еще раз", MessageUI.Emotions.eated);
        SetHighlightOneCounter(_ovenCounter);
        KitchenEvents.OnOvenIngredientAdded += Handler;
        ShowOrderResource(10f);
        
        
        void Handler(int count)
        {
            if (!TutorialStarted) return;
            SetHighlightOneCounter(_ovenCounter);
            if (count != 3) {
                return;
            }
            KitchenEvents.OnOvenIngredientAdded-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_CountIngredientsInOven() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Когда соберёшь все ингредиенты запускай духовку на F", MessageUI.Emotions.happy);
        SetHighlightOneCounter(_ovenCounter);
        KitchenEvents.OnOvenStarted += Handler;
        ShowFButton(3f);
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            SetHighlightOneCounter(_ovenCounter);

            KitchenEvents.OnOvenStarted-=Handler;
            readyToNext = true;
        }
        
        yield return new WaitUntil(() => readyToNext);
    }
    
    private IEnumerator Step_TakePizza() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Отлично! духовка запущена и пицца готовится... ", MessageUI.Emotions.eated);
        SetHighlightOneCounter(_ovenCounter);
        KitchenEvents.OnPizzaReady += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            KitchenEvents.OnPizzaReady-=Handler;
            readyToNext = true;
        }
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_CompletePizzaOrder() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Пицца готова, клади ее на поднос и неси клиенту! ", MessageUI.Emotions.happy);
        OrderManager.Instance.AllowCompleteOrder = true;
        SetHighlightOneCounter(_orderCounter);
        KitchenEvents.OnOrderCompleted += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            KitchenEvents.OnOrderCompleted-=Handler;
            readyToNext = true;
        }
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_TakeDrinkOrder() {
        // УБЕРИ!
        MessageUI.Instance.HideNextButton();
        Player.Instance.StartWalking();
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Заключительный этап, сделаем вкусный напиток! Бери заказ", MessageUI.Emotions.eated);
        OrderManager.Instance.AllowCompleteOrder = false;
        TutorialOrderCounter.Instance.CreateTutorialDrink();
        SetHighlightOneCounter(_juicerCounter);
        KitchenEvents.OnOrderTake += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            SetHighlightOneCounter(_juicerCounter);
            KitchenEvents.OnOrderTake-=Handler;
            readyToNext = true;
        }
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_JuicerWork() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Кидай ингредиенты в соковыжималку, она сама включится", MessageUI.Emotions.happy);
        SetHighlightOneCounter(_juicerCounter);
        KitchenEvents.OnJuicerStarted += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            SetHighlightOneCounter(_juicerCounter);
            KitchenEvents.OnJuicerStarted-=Handler;
            readyToNext = true;
        }
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_JuicerWorking() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Соковыжималка запущена", MessageUI.Emotions.eated);
        SetHighlightOneCounter(_juicerCounter);
        KitchenEvents.OnJuiceReady += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            SetHighlightOneCounter(_juicerCounter);
            KitchenEvents.OnJuiceReady-=Handler;
            readyToNext = true;
        }
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    private IEnumerator Step_JuiceReady() {
        
        readyToNext = false;
        MessageUI.Instance.SetTextInfinity("Отлично, сок готов, клади его на поднос и неси клиенту!", MessageUI.Emotions.happy);
        OrderManager.Instance.AllowCompleteOrder = true;
        SetHighlightOneCounter(_orderCounter);
        KitchenEvents.OnOrderCompleted += Handler;
        
        
        void Handler()
        {
            if (!TutorialStarted) return;
            HideHighlightOneCounter(_orderCounter);
            KitchenEvents.OnOrderCompleted-=Handler;
            readyToNext = true;
        }
        yield return new WaitUntil(() => readyToNext);
    }
    
    
    
    
    
    
    
    // После 1 ингредиента в духовке показать шо можно подсматривать ингредиенты
    // On ingredientsCheck...
    // После того, как положил все ингредиенты сказать что включается на F
    // Отлично, пицца готова! Клади на поднос и неси клиенту




    
    private void SetHighlightOneCounter(BaseCounter counter) {
        if (counter != null && counter.gameObject != null) {
            counter.SetHighlight(true);
        }
    }
    
    private void HideHighlightOneCounter(BaseCounter counter) {
        if (counter != null && counter.gameObject != null) {
            counter.SetHighlight(false);
        }
    }
    
    

    
    public void HideHighlightCountersGroup(List<BaseCounter> countersList) {
        foreach (var counter in countersList) {
            if (counter != null && counter.gameObject != null) {
                counter.SetHighlight(false);
            }
        }
    }
    
    
    private void HideAllCounters() {
        ClientCat.Instance.gameObject.SetActive(false);
        foreach (var counter in _allCounters) {
            if (counter != null && counter.gameObject != null) {
                counter.gameObject.SetActive(false);
            }
        }
    }
    

        
    
    // Методы шоб показать конкретную группу
    public void SetHighlightCountersGroup(List<BaseCounter> countersList) {
        foreach (var counter in countersList) {
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
        ResetAllElements();
        OrderManager.Instance.CreateNewOrder();
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

    public void ShowOrderResource(float time) {
        _orderResourceArrow.SetActive(true);
        StartCoroutine(ShowOrderResourceRoutine(time));
    }   
    
    private IEnumerator ShowOrderResourceRoutine(float time) {
        yield return new WaitForSeconds(time);
        _orderResourceArrow.SetActive(false);
    }
    
    public void ShowFButton(float time) {
        _fButtonArrow.SetActive(true);
        StartCoroutine(ShowFButtonRoutine(time));
    }

    private IEnumerator ShowFButtonRoutine(float time) {
        yield return new WaitForSeconds(time);
        _fButtonArrow.SetActive(false);
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
    
    
    

}
