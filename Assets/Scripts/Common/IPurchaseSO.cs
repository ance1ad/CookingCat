using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchase {
    
    string Id { get; }
    string NameRus { get;}
    string NameEngl{ get; }
    string GetLocalizationName();
    float Price { get; }
    Sprite Icon { get; }
    PurchaseType PurchaseType { get; }
    ValuteType ValuteType { get; }
    
    public void Buy(PlayerData data);

}
