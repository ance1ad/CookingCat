using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Order {
    public DishSO first;
    public DishSO second;
    public DishSO drink;
    public int orderNum;


    public Order(DishSO first,DishSO second, DishSO drink, int orderNum) {
        this.first = first;
        this.second = second;
        this.drink = drink;
        this.orderNum = orderNum;
    }

    public List<DishSO> GetAllDishes() {
        List<DishSO> dishes = new List<DishSO>();
        if (first) dishes.Add(first);
        if (second) dishes.Add(second);
        if (drink) dishes.Add(drink);
        Debug.Log(first?.dishName + " " +  second?.dishName + " " + drink?.dishName + " ¹" + orderNum);
        return dishes;
    }
}


