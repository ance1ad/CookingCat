using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductCard : MonoBehaviour {
    [SerializeField] private KitchenObjectSO _product;
    [SerializeField] private Button _minusButton;
    [SerializeField] private Button _plusButton;
    // Данные визуала
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _price;
    [SerializeField] private TMP_Text _countText;
    private int _count;
    public event Action<ProductCard> OnCardChanged;
    
    
    
    public KitchenObjectSO GetProductSO() => _product;
    public int GetCount() => _count;
    private string pieces = "шт.";
    
    // Подсчет денег
    private void Awake() {
        // Добавить обновление цены, названия и иконки
        _icon.sprite = _product.sprite;
        _name.text = _product.objectName;
        _price.text = _product.price.ToString() + " $";
        pieces = LocalizationManager.Get("Pieces");
        _countText.text = (_count.ToString() + pieces);
    }

    private bool _colorIsGreen;
    public void OnClickPlus() {
        _count++;
        _countText.text = (_count.ToString() + pieces);
        OnCardChanged?.Invoke(this);
        if (!_colorIsGreen) {
            _countText.color = Color.green;
        }
    }
    
    public void OnClickMinus() {
        if (_count == 0) {
            return;
        }
        _count-=1;
        if (_count == 0) {
            _countText.color = Color.white;
            _colorIsGreen = false;
        }
        _countText.text = (_count.ToString() + pieces);
        OnCardChanged?.Invoke(this);
    }

    public void SetZero() {
        _count = 0;
        _countText.text = (0 + pieces);
        _countText.color = Color.white;
        
    }

    public void UpdateText() {
        // Если сменился язык
        if (_name.text != _product.objectName) {
            _name.text = _product.objectName;
        }

        if (pieces != LocalizationManager.Get("Pieces")) {
            pieces = LocalizationManager.Get("Pieces");
            _countText.text = (_count.ToString() + pieces);
        }
            
    }
    
}
