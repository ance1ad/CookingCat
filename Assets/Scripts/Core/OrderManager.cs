using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

// ������������ ���� �������
public class OrderManager : BaseCounter {
    // ��� ����� ����� ���� ________________________________
    [SerializeField] private List<DishSO> _firstDishes;
    [SerializeField] private List<DishSO> _secondDishes;
    [SerializeField] private List<DishSO> _drinks;
    [SerializeField] private OrderCounterVisual _visual;
    [SerializeField] private ThiefCat _thief;

    // _____________________________________________________
    //[SerializeField] private bool stopSpawnOrders = false;
    public Order CurrentOrder; // ��� ������ � �������
    public bool orderIsAppointed;
    public int orderNumber { get; private set; } = 0;
    private float timeToShowIngredients = 5f;

    // Singletone
    public static OrderManager Instance { get; private set; }

    private void Awake() {
        CreateNewOrder();
        if (Instance != null) {
            Debug.Log("There is no more 2 order managers!");
        }
        // ������ �� ������� ����� ������ ����������� � Instance
        Instance = this;
    }



    public void OrderIsCompleted() {
        orderIsAppointed = false;
        CreateNewOrder();
    }



    public void AssignOrderToPlate(Plate plate) {
        if (CurrentOrder == null || plate.Order != null) return;
        if(orderIsAppointed) return;
        orderIsAppointed = true;

        ShowPopupText("���� �����: " + orderNumber);
        _visual.DeleteAllIcons();
        _visual.AddIcons();
        _visual.ShowCanvas();

        plate.SetOrder(CurrentOrder);
        StartCoroutine(_visual.TimerToCloseOrderInfo(timeToShowIngredients));
    }




    private void CreateNewOrder() {
        ShowPopupText("�������� ������ ������ " + ++orderNumber);
        DishSO[] orderDishes = new DishSO[3];
        orderDishes[0] = GetRandomDish(_firstDishes);
        orderDishes[1] = GetRandomDish(_secondDishes);
        orderDishes[2] = GetRandomDish(_drinks);

        CurrentOrder = new Order(orderDishes[0], orderDishes[1], orderDishes[2], orderNumber);
    }


    private DishSO GetRandomDish(List<DishSO> dishes) {
        if (dishes == null || dishes.Count == 0) return null;
        return dishes[UnityEngine.Random.Range(0, dishes.Count)];
    }


    public override void Interact(Player player) {
        // ������ �����
        if (player.HasKitchenObject() && player.GetKitchenObject() is Plate) {
            Plate plate = player.GetKitchenObject() as Plate;
            // ������ �����
            if (!orderIsAppointed) {
                Instance.AssignOrderToPlate(plate);
                // �������� �����
                return;
            }
            if (plate.burgerIngredientsAdded.Count == 0 && plate.pizzaIngredientsAdded.Count == 0 && plate.drinkIngredientsAdded.Count == 0) {
                ShowPopupText("�������� ����� � �����");
                return;
            }
            // ��� ��
            _visual.ShowCanvas();
            DishValidate(CurrentOrder.first, plate.pizzaIngredientsAdded, _visual.pizzaIcons, _visual._canvasPizza);
            DishValidate(CurrentOrder.second, plate.burgerIngredientsAdded, _visual.burgerIcons, _visual._canvasBurger);
            DishValidate(CurrentOrder.drink, plate.drinkIngredientsAdded, _visual.drinkIcons, _visual._canvasDrink);
            plate.DestroyMyself();
            OrderIsCompleted();
        }
        else {
            ShowPopupText("�������� ������ �����");
        }
         
    }

    private void DishValidate(DishSO dish, List<KitchenObjectSO> addedIngredientsList, List<GameObject> dishIcons, GameObject dishCanvas) {

        for (int i = 0; i < dish.ingredients.Count(); i++) {
            KitchenObjectSO ingredient = dish.ingredients[i];
            if (addedIngredientsList.Contains(ingredient)) {
                // �������� ������� ���� ��� ������� � _visual 
                _visual.SetGood(dishIcons[i]);
                addedIngredientsList.Remove(ingredient);
            }
            else {
                // �������� ������� _visual 
                _visual.SetBad(dishIcons[i]);
                _visual.SetGrayColor(dishIcons[i]);

            }
        }
        // ������
        foreach (var ingredient in addedIngredientsList) {
            // �������� � �������� ������� _visual 
            ShowPopupText("���������� " + ingredient + " ������");
            GameObject newIcon = _visual.AddIcon(dishCanvas, ingredient, dishIcons);
            _visual.SetBad(newIcon);
        }
    }
}
