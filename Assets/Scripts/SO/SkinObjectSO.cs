using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SkinObjectSO : ScriptableObject {
    
    [Header("Русский текст")]
    public string nameRus;
    [Header("Английский текст")]
    public string nameEngl;
    
    
    
    public string _name => 
        LocalizationManager.CurrentLanguage == Language.RU ? nameRus : nameEngl;
    
    
    
    public float _price;
    public Sprite _icon;
    public BuyType _skinType;
    public ValuteType _valuteType;
    public GameObject _prefab; // настроенный заранее спаунится правильно
    public Texture _texture; // настроенный заранее спаунится правильно
}
