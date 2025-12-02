using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour {
    [SerializeField] private PlayerData _data;
    [SerializeField] private List<UpgradeObjectSO> _upgrades = new List<UpgradeObjectSO>();
    
    public float PlayerSpeed { get; private set; } = 4.5f;
    public int SliceCount { get; private set; } = 1;
    public float OvenSpeed { get; private set; } 
    public float JuicerSpeed { get; private set; }
    public float MeatFryingSpeed { get; private set; }
    public float MeatOvercookedSpeed { get; private set; }
    public int OrderPeekCount { get; private set; }
    public static PlayerUpgradeManager Instance { get; private set; }

    public event Action OnUpgrade;
    

    private void Awake() {
        if (Instance != null) {
            Debug.Log("WTH, error in Instance");
            return;
        }
        Instance = this;
        _data.OnUpgradeBought += DataOnUpgradesReload;
        
    }
    
    
    private void DataOnUpgradesReload(string id, int count) {
        UpgradeObjectSO upgradeObj = FindUpgradeObjectById(id);
        float bonus = upgradeObj.Bonus;
        
        switch (upgradeObj.UpgradeType) {
            case UpgradeType.PlayerSpeed:
                PlayerSpeed = 4.5f;
                PlayerSpeed += count * bonus;
                break;
            case UpgradeType.SliceCount:
                SliceCount = 1;
                SliceCount += (int)(count * bonus);
                break;
            case UpgradeType.OvenSpeed:
                OvenSpeed = count * bonus;
                break;
            case UpgradeType.JuicerSpeed:
                JuicerSpeed =  count * bonus;
                break;
            case UpgradeType.MeatFryingSpeed:
                MeatFryingSpeed = count * bonus;
                break;
            case UpgradeType.MeatOvercookedSpeed:
                MeatOvercookedSpeed = count * bonus;
                break;
            case UpgradeType.OrderPeek:
                OrderPeekCount = (int)(count * bonus);
                break;
            default:
                Debug.Log("Update not found");
                break;
        }
        OnUpgrade?.Invoke();
    }

    private UpgradeObjectSO FindUpgradeObjectById(string id) => _upgrades.Find(i => i.Id == id);
}
