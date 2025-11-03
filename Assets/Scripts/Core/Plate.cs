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
    [SerializeField] private List<KitchenObjectSO> _maySlicedForBurger;
    [SerializeField] private List<KitchenObjectSO> _forbiddenObjects;
    [SerializeField] private List<KitchenObjectSO> _onlyPizzaObjects;
    [SerializeField] private List<KitchenObjectSO> _onlyJuicerObjects;
    [SerializeField] private KitchenObjectSO _meatUncooked;
    [SerializeField] private KitchenObjectSO _meatOvercooked;



    public List<KitchenObjectSO> pizzaIngredientsAdded = new List<KitchenObjectSO>();
    public List<KitchenObjectSO> burgerIngredientsAdded = new List<KitchenObjectSO>();
    public List<KitchenObjectSO> drinkIngredientsAdded = new List<KitchenObjectSO>();


    
    private int pizzaScore = 0;
    private int burgerScore = 0;
    private int drinkScore = 0;
    private DishVisual[] dish = new DishVisual[3];

    public Order Order { get; set; } // Конкретный заказ

    private GameObject pizzaInstance;
    private GameObject drinkInstance;
    private float offset = -0.228f;


    public event Action<IngredientAddedArgs> OnIndridientAdded;
    public class IngredientAddedArgs {
        public KitchenObjectSO ingredient;
        public Sprite icon;
    }

    private void Start() {
        spawnPoints[1].localPosition += new Vector3(0f, offset, 0f);
    }

    // Вызывает менеджер
    public void SetOrder(Order order) {
        Order = order;
        //ShowPopupText("Взят заказ №" + Order.orderNum);
    }




    public bool AddIngredient(KitchenObject ingredient) {
        var koSO =  ingredient.GetKitchenObjectSO();
        if (!CheckSuitability(ingredient, koSO)) return false;

        DishType ingredientType = CheckIngredientType(koSO);
        if (ingredientType == DishType.pizza) {
            if(pizzaInstance != null) {
                MessageUI.Instance.ShowPlayerPopup("Пицца уже лежит на подносе");
                return false;
            }

            pizzaInstance = Instantiate(ingredient.gameObject, spawnPoints[0]);
            ShowOtherIngredients(ingredient, pizzaIngredientsAdded);
            MessageUI.Instance.ShowPlayerPopup("Пицца добавлена на поднос");
            
        }
        else if(ingredientType == DishType.burger) {
            if (!_fullBurger.ShowIngredient(koSO)) {
                MessageUI.Instance.ShowPlayerPopup("Ингредиент уже добавлен или его нельзя добавить");
                return false;
            }
            if (koSO == bread) {
                spawnPoints[1].localPosition += new Vector3(0f, -offset, 0f);
            }
            ShowBurgerIcons(ingredient, burgerIngredientsAdded);
            

        }
        else {
            if (drinkInstance != null) {
                MessageUI.Instance.ShowPlayerPopup("Напиток уже лежит на подносе");
                
                
                return false;
            }
            drinkInstance = Instantiate(ingredient.gameObject, spawnPoints[2]);

            ShowOtherIngredients(ingredient, drinkIngredientsAdded);
            
        }
        SoundManager.Instance.PlaySFX("PutProduct");
        return true;
    }

    private bool CheckSuitability(KitchenObject ingredient, KitchenObjectSO koSO) {
        if (!ingredient._isFresh) {
            MessageUI.Instance.ShowPlayerPopup("Продукт испорчен, его только выкинуть");
            SoundManager.Instance.PlaySFX("ProductRotten");
            return false;
        }
        if (_forbiddenObjects.Contains(koSO)) {
            MessageUI.Instance.ShowPlayerPopup("Это нельзя положить в бургер");
            return false;
        } 
        if (_maySlicedForBurger.Contains(koSO)) {
            MessageUI.Instance.ShowPlayerPopup(koSO.objectName + " нужно порезать");
            return false;
        }
        if (koSO == _meatUncooked) {
            MessageUI.Instance.ShowPlayerPopup("Мясо нужно пожарить");
            return false;
        }
        if (koSO == _meatOvercooked) {
            MessageUI.Instance.ShowPlayerPopup("Мясо пережарилось");
            return false;
        }
        
        
        if (_onlyJuicerObjects.Contains(koSO)) {
            MessageUI.Instance.ShowPlayerPopup("Это для соковыжималки");
            return false;
        }

        if (_onlyPizzaObjects.Contains(koSO)) {
            MessageUI.Instance.ShowPlayerPopup("Это только для духовки");
            return false;
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
    
    
    public void DestroyPlate() {
        // Очистить все ингредиенты перед уничтожением
        if (pizzaIngredientsAdded != null) pizzaIngredientsAdded.Clear();
        if (burgerIngredientsAdded != null) burgerIngredientsAdded.Clear();
        if (drinkIngredientsAdded != null) drinkIngredientsAdded.Clear();
        if (pizzaInstance != null) Destroy(pizzaInstance);
        if (drinkInstance != null) Destroy(drinkInstance);
        this.DestroyMyself();
    }


    
}
