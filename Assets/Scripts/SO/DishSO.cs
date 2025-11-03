using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���������� ������ ����������� �����
[CreateAssetMenu()]
public class DishSO : ScriptableObject {
    [Header("Русский текст")]
    [SerializeField] private string _dishNameRus;
    [Header("Английский текст")]
    [SerializeField] private string _dishNameEng;
    
    
    public string dishName =>
        LocalizationManager.CurrentLanguage == Language.RU ? _dishNameRus : _dishNameEng;
    
    
    public Sprite dishIcon;
    public KitchenObjectSO[] ingredients;
    public GameObject prefab;
    public float baseReward; // награда за правильное приготовление
}
