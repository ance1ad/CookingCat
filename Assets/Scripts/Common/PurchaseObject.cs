using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class PurchaseObject : MonoBehaviour {
    
    // Базовый класс для скинов и улучшалок
    // [SerializeField] private Text _text;
    // [SerializeField] private GameObject _storeCanvas;
    [SerializeField] protected TMP_Text _name;
    [SerializeField] protected Image _icon;
    
    
    [SerializeField] protected GameObject _priceContainer;
    [SerializeField] protected TMP_Text _price;
    [SerializeField] protected Image _coinsIcon;
    [SerializeField] protected Image _gemsIcon;
    [SerializeField] protected TMP_Text _warningMessage;
    [SerializeField] protected Button _buyButton;
    [SerializeField] protected TMP_Text _buyButtonText;


    
    public abstract float CurrentPrice { get; }
    public abstract IPurchase PurchaseSO { get; }
    public abstract ValuteType ValuteType { get; }
    public abstract PurchaseType PurchaseType{ get; }

    
    protected virtual void UpdateVisuals() {
        Debug.Log("UpdateVisuals");
    }

    public virtual void SetTextLocalization() {
        Debug.Log("SetTextLocalization");
    }

    public abstract void SetBought();
    
    
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

    protected string FormatPrice(float price) {
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
