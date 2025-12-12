using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductManager : MonoBehaviour {
    [SerializeField] private GameObject _storeCanvas;
    [SerializeField] private List<ProductCard> _productCards;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _allPriceText;
    [SerializeField] private TMP_Text _successMessage;
    
    [SerializeField] private TMP_Text _buyButtonText;

    

    private Dictionary<KitchenObjectSO, int> _basketDictionary = new Dictionary<KitchenObjectSO, int>();
    public event Action<Dictionary<KitchenObjectSO, int>> OnProductCardAdded;
    private float _allPrice;
    
    public static ProductManager Instance { get; private set; }



    private void Awake() {
        if (Instance != null) {
            Debug.Log("Ошибко, 2 манажара");
            return;
        }
        Instance = this;
        _storeCanvas.SetActive(false);
    }



    private void Start() {
        foreach (var product in _productCards) {
             product.OnCardChanged += ProductOnOnCardChanged;
            _basketDictionary.TryAdd(product.GetProductSO(), 0);
        }
        _successMessage.enabled = false;
        KitchenEvents.OnSettingsCanvasOpen += HideCanvas;
    }

    private void HideCanvas() {
        if (_storeCanvas.activeSelf) {
            _storeCanvas.SetActive(false);
        }
    }

    private void ProductOnOnCardChanged(ProductCard productCard) {
        _basketDictionary[productCard.GetProductSO()] = productCard.GetCount();
        _allPrice = 0f;
        foreach (var product in _basketDictionary) {
            _allPrice+=product.Key.price * product.Value;
        }

        
        _allPriceText.text = LocalizationManager.Get("AllPrice", _allPrice.ToString("F2"));
    }

    private Coroutine _textCoroutine;
    public void OnCallCourierClicked() {
        if (_textCoroutine != null) {
            StopCoroutine(_textCoroutine);
        }
        // Проверить мани
        if (_allPrice == 0) {
            string message = LocalizationManager.Get("NothingSet");
            _textCoroutine = StartCoroutine(ShowWarningMessageRoutine(false, message));
            return;
        }
        if (CurrencyManager.Instance.Coins < _allPrice) {
            string message = LocalizationManager.Get("LackCoins");
            _textCoroutine = StartCoroutine(ShowWarningMessageRoutine(false, message));
            return;
        }
        CurrencyManager.Instance.UpdateCash(-1 * _allPrice, 0);
        _textCoroutine = StartCoroutine(ShowWarningMessageRoutine(true, LocalizationManager.Get("CourierFound")));
        
        // Шоб не удалился
        var copyDict = new Dictionary<KitchenObjectSO, int>(_basketDictionary);
        OnProductCardAdded?.Invoke(copyDict);
        SoundManager.Instance.PlaySFX("Yandex");
        foreach (var card in _productCards) {
            _basketDictionary[card.GetProductSO()] = 0;
            card.SetZero();
        }
    }

    private IEnumerator ShowWarningMessageRoutine(bool success, string message) {
        _successMessage.enabled = true;
        _successMessage.text = message;
        if (!success) {
            _successMessage.color = Color.red;
        }
        else {
            _successMessage.text = message;
            _successMessage.color = Color.green;
            _allPrice = 0f;
            _allPriceText.text = LocalizationManager.Get("AllPrice", 0);
            SoundManager.Instance.PlaySFX("Warning");
            
        }
        yield return new WaitForSeconds(2f);
        _successMessage.enabled = false;
    }

    public void ShowHideStoreWindow() {
        _storeCanvas.SetActive(!_storeCanvas.activeSelf);
        if (_storeCanvas.activeSelf) {
            PlayerBankVisual.Instance.ShowBank();
            KitchenEvents.ShopOpen();
        }
        else {
            PlayerBankVisual.Instance.HideBank();
            KitchenEvents.ShopClose();
        }
        UpdateTextLocalization();
    }

    private void UpdateTextLocalization() {
        if (_storeCanvas.activeSelf) {
            _title.text = LocalizationManager.Get("ProductStoreTitle");
            _allPriceText.text = LocalizationManager.Get("AllPrice", _allPrice.ToString("F2"));
            _buyButtonText.text = LocalizationManager.Get("BuyButtonText");
            foreach (var card in _productCards) {
                card.UpdateText();
            }
        }
    }
    
}
