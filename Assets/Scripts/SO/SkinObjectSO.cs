using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SkinObjectSO : ScriptableObject {
    public string _name;
    public float _price;
    public Sprite _icon;
    public BuyType _skinType;
    public ValuteType _valuteType;
    public GameObject _prefab; // настроенный заранее спаунится правильно
}
