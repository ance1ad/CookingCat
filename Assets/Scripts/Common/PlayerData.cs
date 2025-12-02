using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;


public class PlayerData : MonoBehaviour {
    public event Action<SkinObjectSO> OnSkinActivate;
    public event Action<SkinObjectSO> OnSkinDeactivate;
    public event Action<string, int> OnUpgradeBought;
    
    
    private void Start() {
        YG2.onGetSDKData += OnGetSDKData;
    }

    private void OnGetSDKData() {
        Debug.Log(YG2.saves.OwnedPurchase.Count);
        foreach (var item in YG2.saves.OwnedPurchase) {
            Debug.Log(item);
        }
    }


    public void AddPurchase(IPurchase purchaseSO) {
        if (!SkinListContainsKey(YG2.saves.OwnedPurchase, purchaseSO.Id)) {
            YG2.saves.OwnedPurchase.Add(purchaseSO.Id);
            YG2.SaveProgress();    
        }
    }


    public void ActivateSkin(SkinObjectSO skinSO) {
        // + добавить логику удаления с этого места старого скина 
        
        if (SkinAppliedContainsKey(YG2.saves.AppliedSkins, skinSO.Id)) {
            OnSkinActivate?.Invoke(skinSO);
        }
        else if (SkinListContainsKey(YG2.saves.OwnedPurchase, skinSO.Id)) {
            YG2.saves.AppliedSkins.Add(new SkinObject {
                type = skinSO.PurchaseType,
                id = skinSO.Id
            });
            OnSkinActivate?.Invoke(skinSO);
        }
        else {
            Debug.LogWarning("Скин " + skinSO.Id + " еще не куплен");
        }

        YG2.SaveProgress();
    }

    public void DeactivateSkin(SkinObjectSO skinSO) {
        if (RemoveSkinInList(YG2.saves.AppliedSkins, skinSO.PurchaseType)) {
            OnSkinDeactivate?.Invoke(skinSO);
        }
        YG2.SaveProgress();
    }

    public void UpdateUpgrade(UpgradeObjectSO upgradeObjectSo) {
        bool upgradeIsContains = false;
        int count;
        var index = YG2.saves.Upgrades.FindIndex(i => i.id == upgradeObjectSo.Id);
        // Уже есть
        if (index >= 0) {
            var item = YG2.saves.Upgrades[index];
            item.count += 1;
            count = item.count;
            YG2.saves.Upgrades[index] = item;
        }
        else {
            YG2.saves.Upgrades.Add(new UpgradeObject {
                id = upgradeObjectSo.Id,
                count = 1,
            });
            count = 1;
        }
        YG2.SaveProgress();
        OnUpgradeBought?.Invoke(upgradeObjectSo.Id, count);
    }


    public void DownloadUpgrades() {
        Debug.Log("Вызов подгрузки всех апгрейдов в PlayerData");
        foreach (var upgrade in YG2.saves.Upgrades) {
            OnUpgradeBought?.Invoke(upgrade.id, upgrade.count);
        }
    }


    private bool SkinListContainsKey(List<string> list, string key) {
        var index = list.FindIndex(i => i == key);
        return index >= 0;
    }
    
    private bool SkinAppliedContainsKey(List<SkinObject> list, string id) {
        var index = list.FindIndex(i => i.id == id);
        return index >= 0;
    }

    private bool RemoveSkinInList(List<SkinObject> list, PurchaseType type) {
        int index = list.FindIndex(i => i.type == type);
        if (index >= 0) {
            list.RemoveAt(index);
            return true;
        }
        return false;
            
    }

    public int GetUpgradeCount(UpgradeObjectSO ObjectSO) 
        => YG2.saves.Upgrades.Find(i => i.id == ObjectSO.Id).count;
    
    

}
