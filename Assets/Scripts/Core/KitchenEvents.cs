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
    public static event Action OnJuicerIngredientAdded;
    public static event Action OnJuiceReady;
    
    public static event Action OnShopOpen;
    public static event Action OnShopClose;
    public static event Action OnProductThrow;

    

    
    
    public static void OrderTake() {
        OnOrderTake?.Invoke();
    }
    
    public static void IngredientAddOnPlate(KitchenObjectSO objectSO) {
        OnIngredientAddedOnPlate?.Invoke(objectSO);
    }
    
    
    public static void OvenStarted() {
        OnOvenStarted?.Invoke();
    }
    
    public static void OvenIngredientAdded(int count) {
        OnOvenIngredientAdded?.Invoke(count);
    }
    
    public static void PizzaReady() {
        OnPizzaReady?.Invoke();
    }
    
    public static void ObjectPutInTable(KitchenObject putedObject) {
        OnObjectPutInTable?.Invoke(putedObject);
    }
    
    public static void OrderCompleted() {
        OnOrderCompleted?.Invoke();
    } 
    
    public static void JuicerStarted() {
        OnJuicerStarted?.Invoke();
    } 
    
    
    public static void JuiceReady() {
        OnJuiceReady?.Invoke();
    }   
    
    public static void JuicerIngredientAdded() {
        OnJuicerIngredientAdded?.Invoke();
    } 
    
    
    public static void ShopOpen() {
        OnShopOpen?.Invoke();
    }
    
    public static void ShopClose() {
        OnShopClose?.Invoke();
    }
    
    public static void ProductThrow() {
        OnProductThrow?.Invoke();
    }

}
