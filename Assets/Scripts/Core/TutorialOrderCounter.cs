using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialOrderCounter : MonoBehaviour {
    [SerializeField] private DishSO _easyBurger;
    [SerializeField] private DishSO _easyPizza;
    [SerializeField] private DishSO _defaultDrink;
    
    
    public static TutorialOrderCounter Instance {get; private set;}

    private void Awake() {
        if (Instance!=null) {
            Debug.Log("TutorialOrderCounter Awoken");
            return; 
        }
        Instance = this;
    }


    private void CreateTutorialState() {
        OrderManager.Instance.orderIsCreated = true;
        OrderManager.Instance.SetEducationParams();
        
        ClientCat.Instance.GoSayOrder();
        OrderManager.Instance.orderNumber++;
        
        SoundManager.Instance.PlaySFX("NewOrder");
        OrderManager.Instance._visual.ShowInfinityPopupText(LocalizationManager.Get("NewOrder", OrderManager.Instance.orderNumber.ToString("D3")));
        
    }
    
    public void CreateTutorialBurger() {
        Debug.Log("CreateTutorialBurger");
        
        CreateTutorialState();
        DishSO[] orderDishes = new DishSO[3];
        orderDishes[1] = _easyBurger;
        Debug.Log("Ожидание на создание нового заказа");
        StartCoroutine(CreateTutorialOrderRoutine(orderDishes));
    }
    
    public void CreateTutorialPizza() {
        Debug.Log("CreateTutorialPizza");
        
        CreateTutorialState();
        DishSO[] orderDishes = new DishSO[3];
        orderDishes[0] = _easyPizza;
        Debug.Log("Ожидание на создание нового заказа");

        StartCoroutine(CreateTutorialOrderRoutine(orderDishes));
        
    }


    public void CreateTutorialDrink() {
        Debug.Log("CreateTutorialDrink");
        
        CreateTutorialState();
        DishSO[] orderDishes = new DishSO[3];
        orderDishes[2] = _defaultDrink;
        Debug.Log("Ожидание на создание нового заказа");

        StartCoroutine(CreateTutorialOrderRoutine(orderDishes));
    }

    private IEnumerator CreateTutorialOrderRoutine(DishSO[] orderDishes) {
        yield return new WaitUntil(() => OrderManager.Instance.NewOrderCreateReady);
        OrderManager.Instance.SetTimeToShowIngredients(orderDishes);
        OrderManager.Instance.CurrentOrder = new Order(orderDishes[0], orderDishes[1], orderDishes[2], OrderManager.Instance.orderNumber); 
        Debug.Log("Создание нового заказа");
        
    }
}
