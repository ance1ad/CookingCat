using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class SkinObjectSO : ScriptableObject, IPurchase, IEquipable {
    
    
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public string NameRus { get; private set; }
    [field: SerializeField] public string NameEngl { get; private set; }
    [field: SerializeField] public float Price { get; private set; }
    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public PurchaseType PurchaseType { get; private set; }
    [field: SerializeField] public ValuteType ValuteType { get; private set; }

    // Unique to Skin
    [field: SerializeField] public GameObject Prefab { get; private set; }
    [field: SerializeField] public Texture Texture { get; private set; }
    
    public string GetLocalizationName() => 
        LocalizationManager.CurrentLanguage == Language.RU ? NameRus : NameEngl;


    public void Buy(PlayerData data) {
        data.AddPurchase(this);
    }

    public void Apply(PlayerData data) {
        data.ActivateSkin(this);
    }

    public void Remove(PlayerData data) {
        data.DeactivateSkin(this);
    }

    
}
