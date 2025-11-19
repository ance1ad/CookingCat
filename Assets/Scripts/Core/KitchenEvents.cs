using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class KitchenEvents {
    public static event Action OnOrderTake;
    public static event Action<KitchenObjectSO> OnIngredientAddedOnPlate;
    public static event Action<KitchenObject> OnObjectPutInTable;
    public static event Action OnOrderCompleted;
    
    
    // OVA
    public static event Action<int> OnOvenIngredientAdded;
    public static event Action OnOvenStarted;
    public static event Action OnPizzaReady;
    
    
    // JUICA
    public static event Action OnJuicerStarted;
    public static event Action OnJuiceReady;
    

    
    
    
    
    public static void OrderTake() {
        Debug.Log("Вызвано событие OnOrderTake");
        OnOrderTake?.Invoke();
    }
    
    public static void IngredientAddOnPlate(KitchenObjectSO objectSO) {
        Debug.Log("Вызвано событие OnIngredientAddedOnPlate " + OnIngredientAddedOnPlate);
        OnIngredientAddedOnPlate?.Invoke(objectSO);
    }
    
    
    public static void OvenStarted() {
        Debug.Log("Вызвано событие OnOvenStarted");
        OnOvenStarted?.Invoke();
    }
    
    public static void OvenIngredientAdded(int count) {
        Debug.Log("Вызвано событие OnOvenIngredientAdded, count: " + count);
        OnOvenIngredientAdded?.Invoke(count);
    }
    
    public static void PizzaReady() {
        Debug.Log("Вызвано событие OnPizzaReady");
        OnPizzaReady?.Invoke();
    }
    
    public static void ObjectPutInTable(KitchenObject putedObject) {
        Debug.Log("Вызвано событие OnObjectPutInTable, обьект " + putedObject);
        OnObjectPutInTable?.Invoke(putedObject);
    }
    
    public static void OrderCompleted() {
        Debug.Log("Вызвано событие OnOrderCompleted");
        OnOrderCompleted?.Invoke();
    } 
    
    public static void JuicerStarted() {
        Debug.Log("Вызвано событие OnJuicerStarted");
        OnJuicerStarted?.Invoke();
    } 
    
    
    public static void JuiceReady() {
        Debug.Log("Вызвано событие OnJuiceReady");
        OnJuiceReady?.Invoke();
    } 
    

}
