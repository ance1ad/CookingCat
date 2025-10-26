using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���������� ������ ����������� �����
[CreateAssetMenu()]
public class DishSO : ScriptableObject {
    public string dishName;
    public Sprite dishIcon;
    public KitchenObjectSO[] ingredients;
    public GameObject prefab;
    public float baseReward; // награда за правильное приготовление
}
