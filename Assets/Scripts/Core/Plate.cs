using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Plate : KitchenObject {
    [SerializeField] private Transform _topPoint;
    [SerializeField] private Transform[] spawnPoints; // ����� ��� ��������� �������


    [SerializeField] private DishVisual _fullBurger; // � ��� ����� �������������� �������� �������
    [SerializeField] private KitchenObjectSO[] _pizzes; // ��� ����� ������� �����
    [SerializeField] private KitchenObjectSO[] _drinks; // ��� ������� ������� �����
    [SerializeField] private KitchenObjectSO bread; // ���� �������� ���� �� ���������� topPoint �����
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

    public Order Order { get; set; } // ���������� �����

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
            ShowPopupText("�� ������ ������ �����");
            //OrderCompleteCheck();
        }
    }


    // �������� ��������
    public void SetOrder(Order order) {
        Order = order;
        //ShowPopupText("���� ����� �" + Order.orderNum);
    }




    public bool AddIngredient(KitchenObject ingredient) {
        if (!ingredient._isFresh) {
            ShowPopupText("������� ��������, ����� ��� ��������");
            return false;
        }
        // ������ ������
        if (forbiddenIngredients.Contains(ingredient.GetKitchenObjectSO())) {
            ShowPopupText("��� ����� ������� ��������");
            return false;
        }

        if (ingredient == _testo) {
            ShowPopupText("����� �������� � ������� ��� ������������� �����");
            return false;
        }

        DishType ingredientType = CheckIngredientType(ingredient.GetKitchenObjectSO());
        if (ingredientType == DishType.pizza) {
            if(pizzaInstance != null) {
                ShowPopupText("����� ��� ����� �� �������");
                return false;
            }

            pizzaInstance = Instantiate(ingredient.gameObject, spawnPoints[0]);
            ShowOtherIngredients(ingredient, pizzaIngredientsAdded);
            ShowPopupText("����� ��������� �� ������");


        }
        else if(ingredientType == DishType.burger) {
            if (!_fullBurger.ShowIngredient(ingredient.GetKitchenObjectSO())) {
                ShowPopupText("���������� ��� �������� ��� ��� ������ ��������");
                return false;
            }
            if (ingredient.GetKitchenObjectSO() == bread) {
                spawnPoints[1].localPosition += new Vector3(0f, -offset, 0f);
            }
            ShowBurgerIcons(ingredient, burgerIngredientsAdded);
            ShowPopupText("�� �������� " + ingredient.GetKitchenObjectSO().objectName.ToLower() + " � ������");

        }
        else {
            if (drinkInstance != null) {
                ShowPopupText("������� ��� ����� �� �������");
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
