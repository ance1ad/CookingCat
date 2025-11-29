using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkinItem : PurchaseObject {
    [SerializeField] private SkinObjectSO _objectSO;
    // Card Visual
    [SerializeField] private Image _grayBackground;
    [SerializeField] private Image _greenBackground;
    [SerializeField] private Image _lock; // замочек;

    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _dequipButton;

    [SerializeField] private TMP_Text _equipButtonText;
    [SerializeField] private TMP_Text _dequipButtonText;

    public override float CurrentPrice => _objectSO.Price;
    public override IPurchase PurchaseSO => _objectSO;
    public IEquipable EquipItem => _objectSO;
    
    public override ValuteType ValuteType => _objectSO.ValuteType; 
    public override PurchaseType PurchaseType => _objectSO.PurchaseType;
    
    
    
    private void Awake() {
        _buyButton.onClick.AddListener(() => OnSkinBought?.Invoke(this));
        _equipButton.onClick.AddListener(() => OnSkinEquipped?.Invoke(this));
        _dequipButton.onClick.AddListener(() => OnSkinDequipped?.Invoke(this));
        _greenBackground.enabled = false;
        _name.fontSize = 70;
        
        _equipButton.gameObject.SetActive(false);
        _dequipButton.gameObject.SetActive(false);
        _buyButton.gameObject.SetActive(false);
    }

    private void Start() {
        SetDefaultVisual();
    }
    

    
    public event Action<SkinItem> OnSkinBought;
    public event Action<SkinItem> OnSkinEquipped;
    public event Action<SkinItem> OnSkinDequipped;

    public SkinObjectSO ObjectSO => _objectSO;



    protected override void SetDefaultVisual() {
        _warningMessage.text = "";
        _name.text = _objectSO.GetLocalizationName();
        _price.text = FormatPrice(_objectSO.Price);
        _icon.sprite = _objectSO.Icon;
        if (_objectSO.ValuteType == ValuteType.Coins) {
            _gemsIcon.enabled = false;
            _coinsIcon.enabled = true;
        }
        else {
            _coinsIcon.enabled = false;
            _gemsIcon.enabled = true;
        }
    }

    public void SetUnbought() {
        _buyButton.gameObject.SetActive(true);
    }
       

    public override void SetTextLocalization() {
        if (_name.text != _objectSO.GetLocalizationName()) {
            _name.text = _objectSO.GetLocalizationName();
        }
        if (_buyButton.gameObject.activeSelf) {
            _buyButtonText.text = LocalizationManager.Get("BuyButtonText");
        }
    }

    
    public override void SetBought() {
        bool state = gameObject.activeSelf;
        if (!state) {
            gameObject.SetActive(true);
        }
        Debug.Log($"Куплен: {_objectSO.GetLocalizationName()}");
        _priceContainer.SetActive(false);
        _grayBackground.enabled = false;
        _lock.enabled = false;
        _buyButton.gameObject.SetActive(false);
        
        _equipButton.gameObject.SetActive(true);
        _equipButtonText.text = LocalizationManager.Get("EquipButtonText");
        _greenBackground.enabled = true;
        
        if (!state) {
            gameObject.SetActive(false);
        }
    }
    
    public void SetSkinEquipped() {
        _equipButton.gameObject.SetActive(false);
        _dequipButton.gameObject.SetActive(true);
        _dequipButtonText.text = LocalizationManager.Get("DequipButtonText");
    }
    
    public void SetSkinDequipped() {
        _dequipButton.gameObject.SetActive(false);
        _equipButton.gameObject.SetActive(true);
        _equipButtonText.text = LocalizationManager.Get("EquipButtonText");
    }
    
    
    private void OnDestroy() {
        _buyButton.onClick.RemoveListener(SetBought);
        _equipButton.onClick.RemoveListener(SetSkinEquipped);
        _dequipButton.onClick.RemoveListener(SetSkinDequipped);
    }
    
}
