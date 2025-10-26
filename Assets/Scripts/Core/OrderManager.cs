using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

// Ограниченный стек заказов
public class OrderManager : BaseCounter {
    // Все блюда какие есть ________________________________
    [SerializeField] private List<DishSO> _firstDishes;
    [SerializeField] private List<DishSO> _secondDishes;
    [SerializeField] private List<DishSO> _drinks;
    [SerializeField] private OrderCounterVisual _visual;
    [SerializeField] private ThiefCat _thief;

    // _____________________________________________________
    public Order CurrentOrder; // Все заказы в системе
    public bool orderIsAppointed;
    public int orderNumber { get; private set; } = 0;
    private float timeToShowIngredients = 5f;

    public static OrderManager Instance { get; private set; }
    public event Action OnOrderCompleted;
    
    private void Awake() {

        if (Instance != null) {
            Debug.Log("There is no more 2 order managers!");
        }
        Instance = this;
    }

    private void Start() {
        CreateNewOrder();
    }


    private float _newCompletedTime = 0;
    public IEnumerator OrderIsCompleted(Plate plate) {
        
        _visual.StopCompleteTimer();
        _newCompletedTime = _visual._completeTimer;
        
        ClientCat.Instance.GivePlate(plate);
        ClientCat.Instance.GoEatOrder();
        
        // Подождать пока уйдет клиент
        yield return new WaitUntil(() => ClientCat.Instance.ClientIsGone);
        _visual.ShowCanvas();

        int allIngredientsAdded = 0;
        Debug.Log("Время выполнения: " + _newCompletedTime);
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
        CreateNewOrder();
    }


    private float _timeToCompleteOrder;
    public void AssignOrderToPlate(Plate plate) {
        if (CurrentOrder == null || plate.Order != null) return;
        if(orderIsAppointed) return;
        orderIsAppointed = true;

        MessageUI.Instance.ShowPlayerPopup("Взят заказ: " + orderNumber);
        _visual.DeleteAllIcons();
        _visual.HideContainers();
        _visual.AddIcons();
        // както устанавливается
        _visual._countToShowOrder = 3;
        _visual.ShowCanvas();
        _visual.ShowButtonToOpenOrder(true);
        

        plate.SetOrder(CurrentOrder);
        StartCoroutine(_visual.TimerToCloseOrderInfo(timeToShowIngredients));
        // Както устанавливается
        _timeToCompleteOrder = 120f;
        _visual._timeToCompleteOrder = _timeToCompleteOrder;
    }
    

    private void CreateNewOrder() {
        // Тут можно регулировать сложность 
        ++orderNumber;

        _visual.ShowInfinityPopupText("Новый заказ №" + orderNumber.ToString("D3") + " !");
        ClientCat.Instance.GoSayOrder();
        DishSO[] orderDishes = new DishSO[3];
        
        
        // orderDishes[0] = GetRandomDish(_firstDishes);
        // orderDishes[1] = GetRandomDish(_secondDishes);
        orderDishes[2] = GetRandomDish(_drinks);

        // Уставновить в visual текст 
        // _visual.setPizzaTitle(orderDishes[0].dishName);
        // _visual.setPizzaTitle(orderDishes[1].dishName);
        // _visual.setPizzaTitle(orderDishes[2].dishName);

        
        CurrentOrder = new Order(orderDishes[0], orderDishes[1], orderDishes[2], orderNumber);
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
                MessageUI.Instance.ShowPlayerPopup("Добавьте блюда в заказ");
                return;
            }
            // Все ок

            StartCoroutine(OrderIsCompleted(plate));
        }
        else {
            MessageUI.Instance.ShowPlayerPopup("Возьмите поднос слева");
        }
         
    }

    private int _foreignExtraCount;
    private void DishValidateVisual(Order.DishStruct dishStruct, List<KitchenObjectSO> addedIngredientsList, List<GameObject> dishIcons, GameObject dishCanvas) {

        if (dishStruct.dish != null) {
            dishStruct.total = dishStruct.dish.ingredients.Count();
            List<KitchenObjectSO> tempAdded = new List<KitchenObjectSO>(addedIngredientsList);
                
            for (int i = 0; i < dishStruct.dish.ingredients.Count(); i++) {
                KitchenObjectSO neededIngredinet = dishStruct.dish.ingredients[i];
                if (addedIngredientsList.Contains(neededIngredinet)) {
                    dishStruct.correct++;
                    _visual.SetGood(dishIcons[i]);
                    addedIngredientsList.Remove(neededIngredinet);
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
                Debug.Log("SetExtra" + ingredient.objectName);
            }
        }
        
        if (dishStruct.dish == null) {
            foreach (var ingredient in addedIngredientsList) {
                Debug.Log("Лишние ингредиенты блюда которого нет: " + ingredient);
                _foreignExtraCount++;
                _visual.ShowCanvas(dishCanvas);
                GameObject newIcon = _visual.AddIcon(dishCanvas, ingredient, dishIcons);
                _visual.SetBad(newIcon);
            }
        }
    }

    
    
    
}
