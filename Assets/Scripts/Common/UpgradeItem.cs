using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



// Визуал карточек
public class UpgradeItem : PurchaseObject  {
    [SerializeField] private UpgradeObjectSO _objectSO;
    [SerializeField] private PlayerData _playerData;
    // Прогресс
    [SerializeField] private Image _level;
    [SerializeField] private TMP_Text _count;

    private float _newPrice;
    public override float CurrentPrice => _newPrice;
    public UpgradeObjectSO ObjectSO => _objectSO;

    public override IPurchase PurchaseSO => _objectSO;
    public override ValuteType ValuteType => _objectSO.ValuteType; 
    public override PurchaseType PurchaseType => _objectSO.PurchaseType;
    
    public event Action<UpgradeItem> OnUpgradeBought;
    
    private void Awake() {
        _buyButton.onClick.AddListener(() => OnUpgradeBought?.Invoke(this));
        SetObject();
        _newPrice = _objectSO.Price;
    }

    private void SetObject() {
        _warningMessage.text = "";
        _name.text = _objectSO.GetLocalizationName();
        _icon.sprite = _objectSO.Icon;
        _price.text = FormatPrice(_objectSO.Price);
        if (_objectSO.ValuteType == ValuteType.Coins) {
            _gemsIcon.enabled = false;
            _coinsIcon.enabled = true;
        }
        else {
            _coinsIcon.enabled = false;
            _gemsIcon.enabled = true;
        }

        _level.fillAmount = 0;
        _count.text = ""; 
    }

    public override void SetBought() {
        int currentCountUpgrades = _playerData.GetUpgradeCount(ObjectSO);
        _newPrice = _objectSO.Price * Mathf.Pow(_objectSO.PriceMultiply, currentCountUpgrades) ;
        _price.text = FormatPrice(CurrentPrice);
        _count.text = $"{currentCountUpgrades}/{ObjectSO.MaxCountUpgrades} ({((float) currentCountUpgrades / ObjectSO.MaxCountUpgrades * 100):F0}%)";
        Debug.Log(currentCountUpgrades);
        Debug.Log($"Куплен: {_objectSO.GetLocalizationName()}");
        if (_priceContainer.activeSelf && ObjectSO.MaxCountUpgrades == currentCountUpgrades) {
            _priceContainer.SetActive(false);
            _level.fillAmount = 1;
            _buyButton.gameObject.SetActive(false);
            return;
        }

        if (ObjectSO.MaxCountUpgrades != 0) {
            _level.fillAmount = (float) currentCountUpgrades / ObjectSO.MaxCountUpgrades;
        }
    }
    
    
}
