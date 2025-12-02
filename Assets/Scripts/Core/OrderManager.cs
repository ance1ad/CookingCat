using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using TMPro;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Profiling;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;
using YG;
using YG.Utils.LB;

// Ограниченный стек заказов
public class OrderManager : BaseCounter {
    // Все блюда какие есть ________________________________
    [SerializeField] private List<DishSO> _firstDishes;
    [SerializeField] private List<DishSO> _secondDishes;
    [SerializeField] private List<DishSO> _drinks;
   
    [SerializeField] public OrderCounterVisual _visual;

    // Запоминание текста и эмодзи при смене языка
    private MessageUI.Emotions lastEmotion;
    private string lastStepKey;
    public bool NewOrderCreateReady { get; private set; } = true;



    // _____________________________________________________
    public Order CurrentOrder; // Все заказы в системе
    public bool orderIsAppointed;
    public int orderNumber { get; set; } = 0;
    private int timeToShowIngredients;
    private float _timeToCompleteOrder;
    public int CountCompleteOrders; 
    
    
    
    public static OrderManager Instance { get; private set; }
    public event Action OnOrderCompleted;
    
    // Разрешения
    public bool AllowCompleteOrder { get; set; }
    public bool StopInteract = false;
    public bool orderIsCreated = false;


    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("There is no more 2 order managers!");
        }
        Instance = this;
        YG2.onGetSDKData += OnGetSDKData;
        YG2.onGetLeaderboard += OnGetLeaderboard;
    }
    
    

    private void OnGetSDKData() {
        CountCompleteOrders = YG2.saves.CountCompleteOrders;
        if (YG2.saves.TutorialPassed) {
            AllowCompleteOrder = true;
        }
    }

    private void Start() {
        // Потом увеличить после тутора
        SettingsManager.Instance.OnSwipeLanguage += OnSwipeLanguage;

    }

    private int record;
    private void OnGetLeaderboard(LBData lb) {
        record = lb.currentPlayer.score;
        if (record < CountCompleteOrders) {
            YG2.SetLeaderboard("MeowLeaderboard", CountCompleteOrders);
        }
        Debug.Log("Текущий рекорд: "  + record);
        Debug.Log("Ранк: "  + lb.currentPlayer.rank);
    }

    private void OnSwipeLanguage() {
        if (!string.IsNullOrEmpty(lastStepKey)) {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get(lastStepKey),lastEmotion);
        }
        if (orderNumber != 0) {
            _visual.ShowInfinityPopupText(LocalizationManager.Get("NewOrder", orderNumber.ToString("D3")));
            _visual.ChangeTextLanguage(LocalizationManager.Get("OrderNumber", orderNumber.ToString("D3")) );
        }
    }

    private float _newCompletedTime = 0;
    public IEnumerator OrderIsCompleted(Plate plate) {
        CountCompleteOrders++;
        YG2.GetLeaderboard("MeowLeaderboard");

        
        YG2.SaveProgress();
        YG2.saves.CountCompleteOrders = CountCompleteOrders;
        orderIsCreated = false;

        _visual.HideOrderNumVisual();
        
        _visual.StopCompleteTimer();
        _newCompletedTime = _visual._completeTimer;
        
        ClientCat.Instance.GivePlate(plate);
        ClientCat.Instance.GoEatOrder();
        
        
        _visual.ShowCanvas();
        _visual.SetResultFlag(true);

        int allIngredientsAdded = 0;
        DishValidateVisual(CurrentOrder.dishStruct[0], plate.pizzaIngredientsAdded, _visual.pizzaIcons, _visual._canvasPizza);
        DishValidateVisual(CurrentOrder.dishStruct[1], plate.burgerIngredientsAdded, _visual.burgerIcons, _visual._canvasBurger);
        DishValidateVisual(CurrentOrder.dishStruct[2], plate.drinkIngredientsAdded, _visual.drinkIcons, _visual._canvasDrink);
        CurrentOrder.foreignExtraCount = _foreignExtraCount;
        CurrentOrder.elapsedTime = _newCompletedTime; // пишем с + если успел с минусом если опоздание
        CurrentOrder.maxTime = _visual._timeToCompleteOrder;

        RewardManager.Instance.CalculateOrderStatistic(CurrentOrder);
        _foreignExtraCount = 0;
       
        orderIsAppointed = false;
        OnOrderCompleted?.Invoke();
        
        Player.Instance._stopWalking = false;
        CurrentOrder.ClearOrder();
        NewOrderCreateReady = true;
        if (TutorialManager.Instance.TutorialStarted) {
            yield return new WaitForSeconds(2f);
            plate.DestroyPlate();
        } 
        else {
            yield return new WaitForSeconds(2f);
            plate.DestroyPlate();
            yield return new WaitForSeconds(6f);
            CreateNewOrder();
        }
        
    }

    
    
    public void AssignOrder() {
        orderIsAppointed = true;

        _visual.SetOrderNumVisual(LocalizationManager.Get("OrderNumber", orderNumber.ToString("D3")) );

        ResetParams();

        StartCoroutine(_visual.TimerToCloseOrderInfo(timeToShowIngredients, orderNumber));
 
    }

    private void ResetParams() {
        _visual.DeleteAllIcons();
        _visual.HideContainers();
        _visual.AddIcons();
       
        _visual.ShowCanvas();
        _visual.SetOrderDishesName(CurrentOrder);
        
        // _visual.ShowButtonToOpenOrder(true);
    }


    
    private string _levelProgressText;
    public void SayLevelResult() {
        if (!string.IsNullOrEmpty(_levelProgressText)) {
            MessageUI.Instance.SetTextInfinity(LocalizationManager.Get(_levelProgressText), MessageUI.Emotions.happy);
        }
    }



    public IEnumerator CreateFirstOrderLater(float time) {
        yield return new WaitForSeconds(time);
        CreateNewOrder();
    }

    
    public bool _burgerIsCreated = false;
    public bool _pizzaIsCreated = false;


    public void CreateNewOrder() {
        if (!NewOrderCreateReady) {
            Debug.Log("Нельзя создавать новый заказ пока активен настоящий ");
            return;
        }
        if (orderIsCreated || TutorialManager.Instance.TutorialStarted) {
            return;
        }

        orderIsCreated = true;
        
        ++orderNumber;
        SoundManager.Instance.PlaySFX("NewOrder");
        _visual.ShowInfinityPopupText(LocalizationManager.Get("NewOrder", orderNumber.ToString("D3")));
        ClientCat.Instance.GoSayOrder();
        
        
        DishSO[] orderDishes = new DishSO[3];
        
        
        if (CountCompleteOrders< 4) {
            if (CountCompleteOrders == 0) {
                orderDishes[1] = GetRandomDish(_secondDishes);
            }
            if (CountCompleteOrders== 1) {
                orderDishes[0] = GetRandomDish(_firstDishes);
            }
            if (CountCompleteOrders== 2) {
                orderDishes[2] = GetRandomDish(_drinks);
            }
            if (CountCompleteOrders == 3) {
                orderDishes[0] = GetRandomDish(_firstDishes);
                orderDishes[2] = GetRandomDish(_drinks);
                _levelProgressText = "CombineOrderTutorial";
            }
            
        }
        else {
            if (Random.Range(0f, 1f) < 0.25) {
                orderDishes[0] = GetRandomDish(_firstDishes);
            }

            if (Random.Range(0f, 1f) < 0.25) {
                orderDishes[1] = GetRandomDish(_secondDishes);
            
            }
            if (Random.Range(0f, 1f) < 0.25) {
                orderDishes[2] = GetRandomDish(_drinks);
            }

            if (orderDishes[0] == null || orderDishes[1] == null || orderDishes[2] == null) {
                orderDishes[1] = GetRandomDish(_secondDishes);
            }
        }
        CountTimesToShowOrder(orderDishes);
        SetTimeToCompleteOrder(orderDishes);
        SetTimeToShowIngredients(orderDishes);
        CurrentOrder = new Order(orderDishes[0], orderDishes[1], orderDishes[2], orderNumber);

        if (CountCompleteOrders<= 2) {
            _visual._firstTimeShowResourceArrow = true;
        }
    }


    public void SetEducationParams() {
        _visual._countToShowOrder = 10;
        _timeToCompleteOrder = 1000;
        _visual._timeToCompleteOrder = _timeToCompleteOrder;
    }

    private void CountTimesToShowOrder(DishSO[] orderDishes)
    {
        var countToShowOrder = 1;
        foreach (var dish in orderDishes) {
            if (dish == null) {
                countToShowOrder--;
            }
        }

        countToShowOrder = 1 + PlayerUpgradeManager.Instance.OrderPeekCount;
        Debug.Log(PlayerUpgradeManager.Instance.OrderPeekCount + " PlayerUpgradeManager.Instance.OrderPeekCount");
        _visual._countToShowOrder = countToShowOrder;
        _visual.SetStateAdvIcon(false);
    }


    private void SetTimeToCompleteOrder(DishSO[] orderDishes) {
        int countIngredients = CountOrderIngredients(orderDishes);
        // условно на 1 ингредиент чтоб собрать 15! секунд ахуеть
        int secondsMultiply = 13;
        _timeToCompleteOrder = countIngredients * secondsMultiply;
        _visual._timeToCompleteOrder = _timeToCompleteOrder;
    }
    
    public void SetTimeToShowIngredients(DishSO[] orderDishes) {
        int countIngredients = CountOrderIngredients(orderDishes);
        int secondsMultiply = 1;
        timeToShowIngredients = countIngredients * secondsMultiply;
        int minimalTime = 7;
        if (timeToShowIngredients < minimalTime) {
            timeToShowIngredients = minimalTime;
        }
    }

    private int CountOrderIngredients(DishSO[] orderDishes) {
        int count = 0;
        foreach (var ingredients in orderDishes) {
            if (ingredients != null) {
                count += ingredients.ingredients.Length;
            }
        }
        return count;
    }


    private DishSO GetRandomDish(List<DishSO> dishes) {
        if (dishes == null || dishes.Count == 0) return null;
        return dishes[Random.Range(0, dishes.Count)];
    }


    public override void Interact(Player player) {
        if (StopInteract) {
            Debug.LogWarning("StopInteract");
            return;
        }
        // Выдача логики без подноса

        if (orderIsAppointed) {
            
            // Проверка выполнения заказа
            if (player.HasKitchenObject() && player.GetKitchenObject() is Plate) {
                Plate plate = player.GetKitchenObject() as Plate;
                // Проверка, что хоть чето есть
                if (plate.burgerIngredientsAdded.Count == 0 && plate.pizzaIngredientsAdded.Count == 0 && plate.drinkIngredientsAdded.Count == 0) {
                    MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("AddDishes"));
                    return;
                }
                // Все ок
                if (_visual._orderIsShowed && AllowCompleteOrder) {
                    
                    KitchenEvents.OrderCompleted();
                    orderIsCreated = false;
                    StartCoroutine(OrderIsCompleted(plate));
                }

                else if (!AllowCompleteOrder) {
                    MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("FirstCompleteOrder"));
                }
            }
            else {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("DontHaveTray"));
            }
        }   
        // Выдача заказа
        else if (!orderIsAppointed && orderIsCreated) {
            Debug.Log("Выдача заказа");
            Instance.AssignOrder();
            NewOrderCreateReady = false; // выдан
            _visual.HideInfinityPopupText();
        }

        if (!orderIsCreated) {
            MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("NoOrders"));
        }
         
    }

    private int _foreignExtraCount;
    private void DishValidateVisual(Order.DishStruct dishStruct, List<KitchenObjectSO> addedIngredientsList, List<GameObject> dishIcons, GameObject dishCanvas) {

        if (dishStruct.dish != null) {
            dishStruct.total = dishStruct.dish.ingredients.Count();
                
            // Пробегаемся по нужным
            for (int i = 0; i < dishStruct.dish.ingredients.Count(); i++) {
                KitchenObjectSO neededIngredinet = dishStruct.dish.ingredients[i];
                // Проверяем что добавленные ингредиенты содержат
                if (addedIngredientsList.Contains(neededIngredinet)) {
                    dishStruct.correct++;
                    _visual.SetGood(dishIcons[i]);
                    addedIngredientsList.Remove(neededIngredinet); // проверили
                }
                else {
                    dishStruct.missing++;
                    _visual.SetBad(dishIcons[i]);
                    _visual.SetGrayColor(dishIcons[i]);
                }
            }
            // Лишние
            foreach (var ingredient in addedIngredientsList) {
                // Добавить и показать крестик _visual 
                dishStruct.extra++;
                _visual.ShowDishCanvas(dishCanvas);
                GameObject newIcon = _visual.AddIcon(dishCanvas, ingredient, dishIcons);
                _visual.SetBad(newIcon);
            }
        }
        
        if (dishStruct.dish == null) {
            foreach (var ingredient in addedIngredientsList) {
                _foreignExtraCount++;
                _visual.ShowDishCanvas(dishCanvas);
                GameObject newIcon = _visual.AddIcon(dishCanvas, ingredient, dishIcons);
                _visual.SetBad(newIcon);
            }
        }
    }


    
    
    
}
