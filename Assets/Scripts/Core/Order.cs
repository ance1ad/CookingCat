using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Order {
    // Общая инфа по заказу
    public int orderNum;
    public float elapsedTime;
    public float maxTime;
    public int foreignExtraCount = 0; // лишние ингредиенты блюда которого нэд
    public int allIngredientsAdded = 0; // лишние ингредиенты блюда которого нэд
    
    // Структура заказа и его выполнение
    public class DishStruct {
        // baseReward берем из DishSO
        public DishSO dish;
        public int correct;
        public int extra;
        public int missing;
        public int total;
    }
    
    public DishStruct[] dishStruct = new DishStruct[3];

    public Order(DishSO first,DishSO second, DishSO drink, int orderNum) {
        dishStruct[0] = new DishStruct();
        dishStruct[1] = new DishStruct();
        dishStruct[2] = new DishStruct();
        
        if (first!=null) {
            dishStruct[0].dish = first;
        }
        
        if (second != null) {
            dishStruct[1].dish = second;
        }

        if (drink != null) {
            dishStruct[2].dish = drink;
        }
        this.orderNum = orderNum;
    }

    public List<DishSO> GetAllDishes() {
        List<DishSO> dishes = new List<DishSO>();
        if (dishStruct[0].dish) dishes.Add(dishStruct[0].dish);
        if (dishStruct[1].dish) dishes.Add(dishStruct[1].dish);
        if (dishStruct[2].dish) dishes.Add(dishStruct[2].dish);
        Debug.Log(dishStruct[0].dish?.dishName + " " +  dishStruct[1].dish?.dishName + " " + dishStruct[2].dish?.dishName + " №" + orderNum);
        return dishes;
    }
}


