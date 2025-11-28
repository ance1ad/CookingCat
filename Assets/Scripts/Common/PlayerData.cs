using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;


public class PlayerData: MonoBehaviour {
    // Будут конкретные SO на Upgrades
    // Будут 1 SO на Skins настроенный

    public HashSet<string> OwnedPurchase { get; private set; } = new(); // Что есть уже у игрока
    
    public Dictionary<PurchaseType, string> AppliedSkins { get; private set; } = new();
    
    public Dictionary<UpgradeObjectSO, int> Upgrades { get; private set; } = new();
    
    public event Action<SkinObjectSO> OnSkinActivate;
    public event Action<SkinObjectSO> OnSkinDeactivate;
    public event Action<Dictionary<UpgradeObjectSO, int>> OnUpgradeBought;

    
    public void AddPurchase(IPurchase purchaseSO) {
        OwnedPurchase.Add(purchaseSO.Id);
    }
    
    
    public void ActivateSkin(SkinObjectSO skinSO) {
        if (!AppliedSkins.ContainsKey(skinSO.PurchaseType)) {
            AppliedSkins.Add(skinSO.PurchaseType,skinSO.Id);
            OnSkinActivate?.Invoke(skinSO);
        }
    }
    
    public void DeactivateSkin(SkinObjectSO skinSO) {
        if (AppliedSkins.Remove(skinSO.PurchaseType)) {
            OnSkinDeactivate?.Invoke(skinSO);
        }
    }

    public void UpdateUpgrade(UpgradeObjectSO upgradeObjectSo) {
        Upgrades.TryAdd(upgradeObjectSo, 0);
        Upgrades[upgradeObjectSo] += 1;
        OnUpgradeBought?.Invoke(Upgrades);
    }

    public int GetUpgradeCount(UpgradeObjectSO upgradeObjectSo) => 
        Upgrades.ContainsKey(upgradeObjectSo) ? Upgrades[upgradeObjectSo] : 0;
    


    public bool HasItem(string key) => OwnedPurchase.Contains(key);
    public bool IsWearItem(string id) => AppliedSkins.ContainsValue(id);


    private void EnableGra() {
        // Запуск после загрузки данных пользователя
        YG2.GameReadyAPI();
    }
    
    
    
}
