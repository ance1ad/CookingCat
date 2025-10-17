using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


// Тут внутри можно рандомно взять элемент из массива вариантов 1 2 3 блюда и напитка
public class DishVisual : MonoBehaviour {


    [SerializeField] public Plate _plate;
    [SerializeField] public DishSO info;
    [SerializeField] public List<IngredientVisual> ingredientVisuals;



    [System.Serializable]
    public struct IngredientVisual {
        public KitchenObjectSO ingredient;
        public GameObject visual;
    }


    public List<KitchenObjectSO> Ingredients;
    public Dictionary<KitchenObjectSO, GameObject> visuals = new Dictionary<KitchenObjectSO, GameObject>();

    private void Awake() {
        foreach (var iv in ingredientVisuals) {
            Debug.Log("Добавление ингредиента " + iv.ingredient);
            visuals.Add(iv.ingredient, iv.visual);
            Ingredients.Add(iv.ingredient);
            iv.visual.SetActive(false); // выключаем всё по умолчанию
        }
    }



    public bool ShowIngredient(KitchenObjectSO ingredient) {
        if (visuals.TryGetValue(ingredient, out var go)) {
            if (go.activeSelf) return false;
            go.SetActive(true);
            return true;
        }
        return false;
    }
}
