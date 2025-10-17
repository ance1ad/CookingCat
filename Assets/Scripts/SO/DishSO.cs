using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���������� ������ ����������� �����
[CreateAssetMenu()]
public class DishSO : ScriptableObject {
    public enum dishType {
        First, // ������: �������, �����
        Second, // �������� ���, ��������
        Third, // ��
        Drink // �������
    }

     
    public string dishName;
    public dishType type;
    public Sprite dishIcon;
    public KitchenObjectSO[] ingredients;
    public GameObject prefab; // ������
}
