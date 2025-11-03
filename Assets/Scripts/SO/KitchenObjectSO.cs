using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject {
    public Transform prefab;
    public Sprite sprite;
    public float price;
    
    
    [Header("Русский текст")]
    [SerializeField] private string _objectName;
    [SerializeField] private string _declension;
    [SerializeField] private string _justification;

    [Header("Английский текст")]
    [SerializeField] private string _englishName;
    [SerializeField] private string _englishDeclension;
    [SerializeField] private string _englishJustification;
    
    
    public string objectName =>
        LocalizationManager.CurrentLanguage == Language.RU ? _objectName : _englishName;

    public string declension =>
        LocalizationManager.CurrentLanguage == Language.RU ? _declension : _englishDeclension;

    public string justification =>
        LocalizationManager.CurrentLanguage == Language.RU ? _justification : _englishJustification;
    
}


