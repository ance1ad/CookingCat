using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Скриптовой обьект конкретного блюда
[CreateAssetMenu()]
public class DishSO : ScriptableObject {
    public enum dishType {
        First, // первое: бургеры, пицца
        Second, // картошка фри, наггэтсы
        Third, // хз
        Drink // напиток
    }

     
    public string dishName;
    public dishType type;
    public Sprite dishIcon;
    public KitchenObjectSO[] ingredients;
    public GameObject prefab; // визуал
}
