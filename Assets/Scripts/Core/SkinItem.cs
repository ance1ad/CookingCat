using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour {
    [SerializeField] private SkinObjectSO _skinObject;
    
    
    // Инфа про скин
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Image _icon;
    
    
    [SerializeField] private GameObject _priceContainer;
    [SerializeField] private TMP_Text _price;
    [SerializeField] private Image _coinsIcon;
    [SerializeField] private Image _gemsIcon;
    
    // Визуал карточки
    [SerializeField] private Image _grayBackground;
    [SerializeField] private Image _greenBackground;
    [SerializeField] private Image _lock; // замочек;

    
    
    
    [SerializeField] private Button _buyButton;
    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _dequipButton;

    [SerializeField] private TMP_Text _warningMessage;
    
    
    
    public event Action<SkinItem> OnSkinBought;
    public event Action<SkinItem> OnSkinEquipped;
    public event Action<SkinItem> OnSkinDequipped;

    private void Start() {
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        _warningMessage.text = "";
        _name.text = _skinObject._name;
        _price.text = FormatPrice(_skinObject._price);
        _icon.sprite = _skinObject._icon;
        if (_skinObject._valuteType == ValuteType.Coins) {
            _gemsIcon.enabled = false;
            _coinsIcon.enabled = true;
        }
        else {
            _coinsIcon.enabled = false;
            _gemsIcon.enabled = true;
        }
        _buyButton.gameObject.SetActive(true);
        _equipButton.gameObject.SetActive(false);
        _dequipButton.gameObject.SetActive(false);
        
    }

    private void Awake() {
        _buyButton.onClick.AddListener(() => OnSkinBought?.Invoke(this));
        _equipButton.onClick.AddListener(() => OnSkinEquipped?.Invoke(this));
        _dequipButton.onClick.AddListener(() => OnSkinDequipped?.Invoke(this));
        _greenBackground.enabled = false;
    }
    

    public SkinObjectSO SkinObject => _skinObject;
    public ValuteType ValuteType => _skinObject._valuteType;
    public BuyType SkinType => _skinObject._skinType;
    public float Price => _skinObject._price;
    
    
    public void SetSkinBought() {
        Debug.Log($"Куплен: {_skinObject._name}");
        _priceContainer.SetActive(false);
        _grayBackground.enabled = false;
        _lock.enabled = false;
        _buyButton.gameObject.SetActive(false);
        _equipButton.gameObject.SetActive(true);
        _greenBackground.enabled = true;
    }
    
    public void SetSkinEquipped() {
        Debug.Log($"Надет: {_skinObject._name}");
        _equipButton.gameObject.SetActive(false);
        _dequipButton.gameObject.SetActive(true);
    }
    
    public void SetSkinDequipped() {
        Debug.Log($"Снят: {_skinObject._name}");
        _dequipButton.gameObject.SetActive(false);
        _equipButton.gameObject.SetActive(true);
    }
    
    
    private void OnDestroy() {
        _buyButton.onClick.RemoveListener(SetSkinBought);
        _equipButton.onClick.RemoveListener(SetSkinEquipped);
        _dequipButton.onClick.RemoveListener(SetSkinDequipped);
    }


    private Coroutine _showWarningTextCoroutine;
    public void SetMessageResultText(string text, bool success) {
        _warningMessage.text = text;
        if (_showWarningTextCoroutine != null) {
            StopCoroutine(_showWarningTextCoroutine);
        }
        _showWarningTextCoroutine = StartCoroutine(ShowWarningTextRoutine());
        if (success) {
            _warningMessage.color = Color.green;
            return;
        }
        _warningMessage.color = Color.red;
    }

    private string FormatPrice(float price) {
        if (ValuteType == ValuteType.Coins && price >= 1000)
            return $"{price / 1000f:0.#}k";
        
        return price.ToString();
    }


    private IEnumerator ShowWarningTextRoutine() {
        yield return new WaitForSeconds(2f);
        _warningMessage.text = "";
    }

    public void ResetWarningText() {
        if (!string.IsNullOrEmpty(_warningMessage.text)) {
            _warningMessage.text = "";
        }
    }
}
