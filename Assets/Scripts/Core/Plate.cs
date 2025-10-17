using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Plate : KitchenObject {
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform[] spawnPoints; // точки где спаунятся обьекты


    [SerializeField] private DishVisual _fullBurger; // В нем будут активироваться элементы бургера
    [SerializeField] private KitchenObjectSO[] _pizzes; // все пиццы которые имеем
    [SerializeField] private KitchenObjectSO[] _drinks; // все напитки которые имеем
    [SerializeField] private KitchenObjectSO bread; // если положили хлеб то возвращаем topPoint вверх
    [SerializeField] private List<KitchenObjectSO> forbiddenIngredients;
    [SerializeField] private KitchenObjectSO _testo;

    [SerializeField] public GameObject _popupCanvas;


    public List<KitchenObjectSO> pizzaIngredientsAdded = new List<KitchenObjectSO>();
    public List<KitchenObjectSO> burgerIngredientsAdded = new List<KitchenObjectSO>();
    public List<KitchenObjectSO> drinkIngredientsAdded = new List<KitchenObjectSO>();

    private float showPopupTime = 2.5f;

    private int pizzaScore = 0;
    private int burgerScore = 0;
    private int drinkScore = 0;
    private DishVisual[] dish = new DishVisual[3];

    public Order Order { get; set; } // Конкретный заказ

    private GameObject pizzaInstance;
    private GameObject drinkInstance;
    private float offset = -0.257f;


    public event Action<IngredientAddedArgs> OnIndridientAdded;
    public class IngredientAddedArgs {
        public KitchenObjectSO ingredient;
        public Sprite icon;
    }

    private void Start() {
        spawnPoints[1].localPosition += new Vector3(0f, offset, 0f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShowPopupText("Вы готовы отдать заказ");
            //OrderCompleteCheck();
        }
    }


    // Вызывает менеджер
    public void SetOrder(Order order) {
        Order = order;
        //ShowPopupText("Взят заказ №" + Order.orderNum);
    }




    public bool AddIngredient(KitchenObject ingredient) {
        if (!ingredient._isFresh) {
            ShowPopupText("Продукт испорчен, лучше его выкинуть");
            return false;
        }
        // Нельзя класть
        if (forbiddenIngredients.Contains(ingredient.GetKitchenObjectSO())) {
            ShowPopupText("Это нужно сначала порезать");
            return false;
        }

        if (ingredient == _testo) {
            ShowPopupText("Тесто положите в духовку для приготовления пиццы");
            return false;
        }

        DishType ingredientType = CheckIngredientType(ingredient.GetKitchenObjectSO());
        if (ingredientType == DishType.pizza) {
            if(pizzaInstance != null) {
                ShowPopupText("Пицца уже лежит на подносе");
                return false;
            }

            pizzaInstance = Instantiate(ingredient.gameObject, spawnPoints[0]);
            ShowOtherIngredients(ingredient, pizzaIngredientsAdded);
            ShowPopupText("Пицца добавлена на поднос");


        }
        else if(ingredientType == DishType.burger) {
            if (!_fullBurger.ShowIngredient(ingredient.GetKitchenObjectSO())) {
                ShowPopupText("Ингредиент уже добавлен или его нельзя добавить");
                return false;
            }
            if (ingredient.GetKitchenObjectSO() == bread) {
                spawnPoints[1].localPosition += new Vector3(0f, -offset, 0f);
            }
            ShowBurgerIcons(ingredient, burgerIngredientsAdded);
            ShowPopupText("Вы положили " + ingredient.GetKitchenObjectSO().objectName.ToLower() + " в бургер");

        }
        else {
            if (drinkInstance != null) {
                ShowPopupText("Напиток уже лежит на подносе");
                return false;
            }
            drinkInstance = Instantiate(ingredient.gameObject, spawnPoints[2]);

            ShowOtherIngredients(ingredient, drinkIngredientsAdded);
        }
        return true;
    }

    private void ShowOtherIngredients(KitchenObject ingredient, List<KitchenObjectSO> list) {
        var ingredientDishVisual = ingredient.GetComponent<DishVisual>();
        list.AddRange(ingredientDishVisual.Ingredients);
        foreach (var element in ingredientDishVisual.Ingredients) {
            OnIndridientAdded?.Invoke(new IngredientAddedArgs {
                ingredient = element,
                icon = element.sprite,
            }); ;
        }
        ingredient.DestroyMyself();
    }

    private void ShowBurgerIcons(KitchenObject ingredient, List<KitchenObjectSO> list) {
        OnIndridientAdded?.Invoke(new IngredientAddedArgs {
            ingredient = ingredient.GetKitchenObjectSO(),
            icon = ingredient.GetKitchenObjectSO().sprite,
        }); ;
        list.Add(ingredient.GetKitchenObjectSO());
        ingredient.DestroyMyself();
    }



    private enum DishType {
        pizza,
        drink,
        burger,
    }
    private DishType CheckIngredientType(KitchenObjectSO ingredient) {
        if (_pizzes.Contains(ingredient)) {
            return DishType.pizza;
        }
        if (_drinks.Contains(ingredient)) {
            return DishType.drink;
        }
        return DishType.burger;
    }

    private Coroutine corutine;

    public void ShowPopupText(string text) {
        _popupCanvas.SetActive(true);
        _popupCanvas.transform.GetChild(1).GetComponent<TMP_Text>().text = text;
        if (corutine != null) {
            StopCoroutine(corutine);
        }
        corutine = StartCoroutine(Timer(showPopupTime));
    }

    public IEnumerator Timer(float time) {
        yield return new WaitForSeconds(time);
        _popupCanvas.SetActive(false);
        corutine = null;
    }
}
