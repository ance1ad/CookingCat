using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu()]
public class UpgradeObjectSO: ScriptableObject, IPurchase {
    
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string NameRus { get; private set; }
    [field: SerializeField] public string NameEngl { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public PurchaseType PurchaseType { get; private set; }
    [field: SerializeField] public ValuteType ValuteType { get; private set; }
    [field: SerializeField] public UpgradeType UpgradeType { get; private set; }
    // Unique to Upgrade
    // Upgrade multiply: slice: +1, speed: +0.5 mb
    [field: SerializeField] public float Bonus { get; private set; }
    [field: SerializeField] public int MaxCountUpgrades { get; private set; }
    [field: SerializeField] public float PriceMultiply { get; private set; }
    
    
    
    public string GetLocalizationName() => 
        LocalizationManager.CurrentLanguage == Language.RU ? NameRus : NameEngl;


    // Тока покупает, применять не нужно удалять не нужно
    public void Buy(PlayerData data) {
        data.UpdateUpgrade(this);
    }
    
}
