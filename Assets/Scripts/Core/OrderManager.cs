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
using Random = System.Random;

// Ограниченный стек заказов
public class OrderManager : BaseCounter {
    // Все блюда какие есть ________________________________
    [SerializeField] private List<DishSO> _firstDishes;
    [SerializeField] private List<DishSO> _secondDishes;
    [SerializeField] private List<DishSO> _drinks;
    [SerializeField] private OrderCounterVisual _visual;
    [SerializeField] private ThiefCat _thief;

    // Запоминание текста и эмодзи при смене языка
    private MessageUI.Emotions lastEmotion;
    private string lastStepKey;
        
    // _____________________________________________________
    public Order CurrentOrder; // Все заказы в системе
    public bool orderIsAppointed;
    public int orderNumber { get; private set; } = 0;
    private int timeToShowIngredients;
    private float _timeToCompleteOrder;
    public int Level; // Увеличивается извне в Reward

    public static OrderManager Instance { get; private set; }
    public event Action OnOrderCompleted;
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("There is no more 2 order managers!");
        }
        Instance = this;
    }
    
    private void Start() {
        Level = 1;
        SettingsManager.Instance.OnSwipeLanguage += OnSwipeLanguage;
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
        orderIsCreated = false;
        // Пусть постоит
        // Player.Instance.StopWalking();
        _visual.HideOrderNumVisual();
        
        _visual.StopCompleteTimer();
        _newCompletedTime = _visual._completeTimer;
        
        ClientCat.Instance.GivePlate(plate);
        ClientCat.Instance.GoEatOrder();
        
        // Подождать пока уйдет клиент
        // yield return new WaitUntil(() => ClientCat.Instance.ClientIsGone);
        _visual.ShowCanvas();

        int allIngredientsAdded = 0;
        DishValidateVisual(CurrentOrder.dishStruct[0], plate.pizzaIngredientsAdded, _visual.pizzaIcons, _visual._canvasPizza);
        DishValidateVisual(CurrentOrder.dishStruct[1], plate.burgerIngredientsAdded, _visual.burgerIcons, _visual._canvasBurger);
        DishValidateVisual(CurrentOrder.dishStruct[2], plate.drinkIngredientsAdded, _visual.drinkIcons, _visual._canvasDrink);
        CurrentOrder.foreignExtraCount = _foreignExtraCount;
        CurrentOrder.elapsedTime = _newCompletedTime; // пишем с + если успел с минусом если опоздание
        CurrentOrder.maxTime = _visual._timeToCompleteOrder;
        // Подсчет общего accuracy
        // Передаем в RewardManager Order он разберется
        RewardManager.Instance.CalculateOrderStatistic(CurrentOrder);
        _foreignExtraCount = 0;
       
        orderIsAppointed = false;
        OnOrderCompleted?.Invoke();
        yield return new WaitForSeconds(10f);
        plate.DestroyPlate();
        plate.DestroyMyself();
        Player.Instance._stopWalking = false;
        CurrentOrder.ClearOrder();
        CreateNewOrder("OM");
    }


    public void AssignOrderToPlate(Plate plate) {
        if (CurrentOrder == null || plate.Order != null) return;
        if(orderIsAppointed) return;
        orderIsAppointed = true;

        MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("OrderTaken", orderNumber));
        
        _visual.SetOrderNumVisual(LocalizationManager.Get("OrderNumber", orderNumber.ToString("D3")) );

        
        _visual.DeleteAllIcons();
        _visual.HideContainers();
        _visual.AddIcons();
       
        _visual.ShowCanvas();
        _visual.SetOrderDishesName(CurrentOrder);
        
        _visual.ShowButtonToOpenOrder(true);
        

        plate.SetOrder(CurrentOrder);
        StartCoroutine(_visual.TimerToCloseOrderInfo(timeToShowIngredients));
 
    }


    
    private string _levelProgressText;
    public void SayLevelResult() {
        MessageUI.Instance.SetText(LocalizationManager.Get(_levelProgressText), MessageUI.Emotions.happy);
    }

    public int _tryCounts = 0;


    private bool orderIsCreated = false;
    public void CreateNewOrder(string client) {
        Debug.Log(client);
        if (orderIsCreated) {
            return;
        }
        orderIsCreated = true;
        
        Debug.Log("CreateNewOrder");
        Debug.Log("Level:" + Level);
        ++orderNumber;
        SoundManager.Instance.PlaySFX("NewOrder");
        _visual.ShowInfinityPopupText(LocalizationManager.Get("NewOrder", orderNumber.ToString("D3")));
        ClientCat.Instance.GoSayOrder();
        
        
        DishSO[] orderDishes = new DishSO[3];
        
                
        if (Level == 5) {
            lastEmotion = MessageUI.Emotions.happy;
            lastStepKey = "GoodJobAndReady";
            MessageUI.Instance.SetText(LocalizationManager.Get(lastStepKey), MessageUI.Emotions.happy);
        }
        
        if (Level <= 4) {
            // Тут можно регулировать сложность 
            _tryCounts++;
            if (Level == 1) {
                if (_tryCounts == 1) {
                    Debug.Log("CreateBurgerTutorial");
                    lastEmotion = MessageUI.Emotions.eated;
                    lastStepKey = "CreateBurgerTutorial";
                    MessageUI.Instance.SetTextInfinity(LocalizationManager.Get(lastStepKey), lastEmotion);
                }
                orderDishes[1] = GetRandomDish(_secondDishes);
                _levelProgressText = "TryAgainBurger";
                // Показать стрелку вверх влева
            }
            if (Level == 2) {
                if (_tryCounts == 1) {
                    lastEmotion = MessageUI.Emotions.defaultFace;
                    lastStepKey = "CreatePizzaTutorial";
                    MessageUI.Instance.SetText(LocalizationManager.Get(lastStepKey), lastEmotion);
                }
                orderDishes[0] = GetRandomDish(_firstDishes);
                _levelProgressText = "TryAgainPizza";
            
            }
            if (Level == 3) {
                if (_tryCounts == 1) {
                    lastEmotion = MessageUI.Emotions.eated;
                    lastStepKey = "CreateDrinkTutorial";
                    MessageUI.Instance.SetText(LocalizationManager.Get(lastStepKey), lastEmotion);
                }
                orderDishes[2] = GetRandomDish(_drinks);
                _levelProgressText = "TryAgainDrink";
            }
            if (Level == 4) {
                lastEmotion = MessageUI.Emotions.happy;
                lastStepKey = "CombineOrderTutorial";
                MessageUI.Instance.SetText(LocalizationManager.Get(lastStepKey), lastEmotion);
                orderDishes[0] = GetRandomDish(_firstDishes);
                orderDishes[2] = GetRandomDish(_drinks);
            }
            SetEducationParams();
        }
        else {
            if (UnityEngine.Random.Range(0f, 1f) < 0.4) {
                orderDishes[0] = GetRandomDish(_firstDishes);
            }

            if (UnityEngine.Random.Range(0f, 1f) < 0.4) {
                orderDishes[1] = GetRandomDish(_secondDishes);
            
            }
            if (UnityEngine.Random.Range(0f, 1f) < 0.4) {
                orderDishes[2] = GetRandomDish(_drinks);
            }

            if (orderDishes[0] == null || orderDishes[1] == null || orderDishes[2] == null) {
                orderDishes[1] = GetRandomDish(_secondDishes);
            }
            CountTimesToShowOrder(orderDishes);
            SetTimeToCompleteOrder(orderDishes);
        }
        SetTimeToShowIngredients(orderDishes);
        CurrentOrder = new Order(orderDishes[0], orderDishes[1], orderDishes[2], orderNumber);

        if (Level == 1) {
            _visual._firstTimeShowResourceArrow = true;
        }
    }


    private void SetEducationParams() {
        _visual._countToShowOrder = 99;
        _timeToCompleteOrder = 1000;
        _visual._timeToCompleteOrder = _timeToCompleteOrder;
    }

    private void CountTimesToShowOrder(DishSO[] orderDishes)
    {
        var countToShowOrder = 3;
        foreach (var dish in orderDishes) {
            if (dish == null) {
                countToShowOrder--;
            }
        }
        _visual._countToShowOrder = countToShowOrder;
    }


    private void SetTimeToCompleteOrder(DishSO[] orderDishes) {
        int countIngredients = CountOrderIngredients(orderDishes);
        // условно на 1 ингредиент чтоб собрать 5 секунд
        int secondsMultiply = 15;
        _timeToCompleteOrder = countIngredients * secondsMultiply;
        _visual._timeToCompleteOrder = _timeToCompleteOrder;
    }
    
    private void SetTimeToShowIngredients(DishSO[] orderDishes) {
        int countIngredients = CountOrderIngredients(orderDishes);
        int secondsMultiply = 1;
        timeToShowIngredients = countIngredients * secondsMultiply;
        timeToShowIngredients--;
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
        return dishes[UnityEngine.Random.Range(0, dishes.Count)];
    }


    public override void Interact(Player player) {
        // Отдать заказ
        if (player.HasKitchenObject() && player.GetKitchenObject() is Plate) {
            Plate plate = player.GetKitchenObject() as Plate;
            // Выдать заказ
            if (!orderIsAppointed) {
                Instance.AssignOrderToPlate(plate);
                _visual.HideInfinityPopupText();
                // показать заказ
                return;
            }
            if (plate.burgerIngredientsAdded.Count == 0 && plate.pizzaIngredientsAdded.Count == 0 && plate.drinkIngredientsAdded.Count == 0) {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("AddDishes"));
                return;
            }
            // Все ок

            StartCoroutine(OrderIsCompleted(plate));
        }
        else {
            MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("DontHaveTray"));
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
                _visual.ShowCanvas(dishCanvas);
                GameObject newIcon = _visual.AddIcon(dishCanvas, ingredient, dishIcons);
                _visual.SetBad(newIcon);
            }
        }
        
        if (dishStruct.dish == null) {
            foreach (var ingredient in addedIngredientsList) {
                _foreignExtraCount++;
                _visual.ShowCanvas(dishCanvas);
                GameObject newIcon = _visual.AddIcon(dishCanvas, ingredient, dishIcons);
                _visual.SetBad(newIcon);
            }
        }
    }


    
    
    
}
