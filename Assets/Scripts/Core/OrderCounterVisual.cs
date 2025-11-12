using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms;

public class OrderCounterVisual : MonoBehaviour {
    
    [Header("Канвас заказа")]
    [SerializeField] public GameObject _iconTemplate;
    [SerializeField] public GameObject _generalCanvas;
    [SerializeField] public GameObject _canvasPizza;
    [SerializeField] public GameObject _pizzaContainer;
    [SerializeField] public GameObject _canvasBurger;
    [SerializeField] public GameObject _burgerContainer;
    [SerializeField] public GameObject _canvasDrink;
    [SerializeField] public GameObject _drinkContainer;
    [SerializeField] public TMP_Text _timerText;
    [SerializeField] public TMP_Text _closeIn;
    [SerializeField] public GameObject _timerCanvas;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private GameObject _bigCloseButton;
    [SerializeField] private Image _level;
    [SerializeField] private Image _new;
    [SerializeField] private TMP_Text orderStatusText; // новый заказ или показ правильности
    [SerializeField] private TMP_Text _lateText;
    
    [SerializeField] public Transform _popupCanvas;

    [SerializeField] private TMP_Text _pizzaName;
    [SerializeField] private TMP_Text _burgerName;
    [SerializeField] private TMP_Text _drinkName;

    [SerializeField] private TMP_Text _orderNumberText; // сверху слева
    [SerializeField] public GameObject _orderNumberTextBackground; // сверху слева

    public bool _orderIsShowed;

    
    public void SetOrderDishesName(Order order) {
        if (order.dishStruct[0].dish != null) {
            _pizzaName.text = order.dishStruct[0].dish.dishName;
        }
        if (order.dishStruct[1].dish != null) {
            _burgerName.text = order.dishStruct[1].dish.dishName;
        }
        if (order.dishStruct[2].dish != null) {
            _drinkName.text = order.dishStruct[2].dish.dishName;
        }
    }
    
    
    [Header("Иконки канваса")]
    public List<GameObject> pizzaIcons = new List<GameObject>();
    public List<GameObject> burgerIcons = new List<GameObject>();
    public List<GameObject> drinkIcons = new List<GameObject>();

    
    [Header("UI повторного показа заказа")]
    [SerializeField] private Button _showOrderButton;
    [SerializeField] private TMP_Text _contToShowOrderText; // Сколько еще раз показать
    public int _countToShowOrder; // сколько еще раз можно подсмотреть в заказ

    
    private bool showOrder = true;

    [Header("Таймер выполнения заказа")]
    [SerializeField] private GameObject _orderTimerVisual;
    [SerializeField] private TMP_Text _orderCompleteTimerText;
    [SerializeField] private Image _timerToCompleteOrderImg;

    [Header("Вор")]
    [SerializeField] private ThiefCat _thief; // воришка
    

    private void Start() {
        _showOrderButton.gameObject.SetActive(false);
        _orderTimerVisual.SetActive(false);
        _orderNumberTextBackground.SetActive(false);
    }



    public void HideContainers() {
        _pizzaContainer.SetActive(false);
        _burgerContainer.SetActive(false);
        _drinkContainer.SetActive(false);
    }
    
    public void ShowCanvas(GameObject canvas) {
        if (canvas == _canvasPizza) {
            _pizzaContainer.SetActive(true);
        }
        if (canvas == _canvasBurger) {
            _burgerContainer.SetActive(true);
        }
        if (canvas == _canvasDrink) {
            _drinkContainer.SetActive(true);
        }
    }
    

    public void AddIcons() {
        Order order = OrderManager.Instance.CurrentOrder;
        if (order.dishStruct[0].dish != null) {
            _pizzaContainer.SetActive(true);
            foreach (var ingredient in order.dishStruct[0].dish.ingredients) {
                AddIcon(_canvasPizza, ingredient, pizzaIcons);
            }
        }
        if (order.dishStruct[1].dish != null) {
            _burgerContainer.SetActive(true);
            foreach (var ingredient in order.dishStruct[1].dish.ingredients) {
                AddIcon(_canvasBurger, ingredient, burgerIcons );
            }
        }
        if (order.dishStruct[2].dish != null) {
            _drinkContainer.SetActive(true);
            foreach (var ingredient in order.dishStruct[2].dish.ingredients) {
                AddIcon(_canvasDrink, ingredient, drinkIcons);
            }
        }
    }
    

    public void DeleteAllIcons() {
        foreach (var element in pizzaIcons) {
            Destroy(element);
        }
        pizzaIcons.Clear();

        foreach (var element in burgerIcons) {
            Destroy(element);
        }
        burgerIcons.Clear();

        foreach (var element in drinkIcons) {
            Destroy(element);
        }
        drinkIcons.Clear();
    }


    public GameObject AddIcon(GameObject canvas, KitchenObjectSO ingredient, List<GameObject> list) {
        GameObject newIcon = Instantiate(_iconTemplate);
        newIcon.transform.GetChild(2).GetComponent<Image>().sprite = ingredient.sprite;
        newIcon.transform.SetParent(canvas.transform, false);
        newIcon.SetActive(true);
        list.Add(newIcon);
        SetText(newIcon, ingredient.objectName);
        return newIcon;
    }

    
    public void SetGrayColor(GameObject icon) {
        icon.transform.GetChild(2).GetComponent<Image>().color = Color.gray;
    }



    public void SetGood(GameObject icon) {
        icon.transform.GetChild(3).gameObject.SetActive(true);
    }

    public void SetBad(GameObject icon) {
        icon.transform.GetChild(4).gameObject.SetActive(true);
    }

    public void SetText(GameObject icon, string text) {
        icon.transform.GetChild(5).GetComponent<TMP_Text>().text = text;
    }


    private float timeToShowIngredients;
    public bool _firstTimeShowResourceArrow = true;
    public IEnumerator TimerToCloseOrderInfo(float time, int orderNumber) {
        _orderIsShowed = false;
        timeToShowIngredients = time;
        _thief.StopThiefCycle();

        ShowButtonToOpenOrder(false);
        
        float timeNow = 0f;
        while (timeNow < time) {
            _timerText.text = LocalizationManager.Get("OrderShowTimerText", (int)((time - timeNow)+1));
            _level.fillAmount = (1f - timeNow / time);

            timeNow += Time.deltaTime;
            // обновлять потом поле с временем до закрытия
            yield return null;
        }

        _level.fillAmount = 0;
        ShowButtonToOpenOrder(true);


        // Показ кнопки и таймера
        _orderTimerVisual.SetActive(true);
        _showOrderButton.gameObject.SetActive(true);
        
        if (orderTimerCoroutine == null) {
            orderTimerCoroutine = StartCoroutine(StartNewTimerRoutine());
            _lateText.text = "";
        }



        if (OrderManager.Instance.Level > 1) {
            _thief.StartThiefCycle();
        }
        _generalCanvas.SetActive(false);
        if (OrderManager.Instance.Level == 1 && _firstTimeShowResourceArrow) {
            _firstTimeShowResourceArrow = false;
            TutorialManager.Instance.ShowOrderResource();
            MessageUI.Instance.SetText(LocalizationManager.Get("LastStep"), MessageUI.Emotions.defaultFace);
        }
        _orderIsShowed = true;
        if (orderNumber != -1) {
            MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("OrderTaken", orderNumber));
        }
    }

    
    
    private Coroutine orderTimerCoroutine;
    public float _completeTimer { get; private set; }
    public float _timeToCompleteOrder; // устанавливается из OrderManager


    public void StopCompleteTimer() {
        if (orderTimerCoroutine != null) {
            StopCoroutine(orderTimerCoroutine);
            orderTimerCoroutine = null;
        }
    }
    
    private IEnumerator StartNewTimerRoutine() {
        _orderCompleteTimerText.text = _timeToCompleteOrder.ToString("0");
        yield return new WaitForSeconds(0.5f);
        Debug.Log(orderTimerCoroutine);
        _completeTimer = 0f;
        while (_completeTimer <= _timeToCompleteOrder) {
            _timerToCompleteOrderImg.fillAmount = (1 - _completeTimer/_timeToCompleteOrder);
            _orderCompleteTimerText.text = (_timeToCompleteOrder - _completeTimer).ToString("0");
            _completeTimer += Time.deltaTime;
            yield return null;
        }

        _timerToCompleteOrderImg.fillAmount = 0;
        _lateText.text = LocalizationManager.Get("LateText");
        
        while (true) {
            _completeTimer += Time.deltaTime;
            _orderCompleteTimerText.text = ((_completeTimer - _timeToCompleteOrder)).ToString("0");
            yield return null;
        }
    }
    

    public void ShowCanvas() {
        if (showOrder) {
            _orderTimerVisual.SetActive(false);
            _contToShowOrderText.text = _countToShowOrder.ToString();
            orderStatusText.text = LocalizationManager.Get("NewOrderCanvas");
            showOrder = false;
            _new.gameObject.SetActive(true);
            _timerCanvas.gameObject.SetActive(true);
            // Тут текст поменять закроется через
            _closeIn.text = LocalizationManager.Get("CloseIn");
            _bigCloseButton.SetActive(false);
            _closeButton.SetActive(false);
        }
        else {
            _showOrderButton.gameObject.SetActive(false);
            _orderTimerVisual.SetActive(false);
            orderStatusText.text = LocalizationManager.Get("CompleteOrderCanvas");
            SoundManager.Instance.PlaySFX("GiveOrder");
            _thief.StopThiefCycle();
            showOrder = true;
            _new.gameObject.SetActive(false);
            _timerCanvas.SetActive(false);
            _bigCloseButton.SetActive(true);
            _closeButton.SetActive(true);
            
        }
        _generalCanvas.SetActive(true);
    }

        
        
    public void HideCanvas() {
        _generalCanvas.SetActive(false);
    }

    public void ShowButtonToOpenOrder(bool state) {
        _showOrderButton.enabled = state;
    }
    
    
    
    public void ShowOrderByButton() {
        if (_countToShowOrder == 0) {
            ShowButtonToOpenOrder(false);
            return;
        }
        
        _bigCloseButton.SetActive(true);
        _closeButton.SetActive(true);
        orderStatusText.text = LocalizationManager.Get("OrderStatusText");
        _countToShowOrder--;
        _generalCanvas.SetActive(true);
        StartCoroutine(TimerToCloseOrderInfo(timeToShowIngredients, -1));
        _contToShowOrderText.text = _countToShowOrder.ToString();
        _new.gameObject.SetActive(false);
    }

        
    private float showPopupTime = 2f;
    private Coroutine _showTextCoroutine;
    
    
    public void ShowPopupText(string text) {
        _popupCanvas.gameObject.SetActive(true);
        Text.text = text;
        if (_showTextCoroutine != null) {
            StopCoroutine(_showTextCoroutine);
        }
        _showTextCoroutine = StartCoroutine(Timer(showPopupTime));
    }

    public void ShowInfinityPopupText(string text) {
        if (!_popupCanvas.gameObject.activeSelf) {
            _popupCanvas.gameObject.SetActive(true);
        }
        Text.text = text;
    }

    
    
    
    private TMP_Text _text;
    private TMP_Text Text {
        get {
            if (_text == null) {
                _text = _popupCanvas.GetChild(1).GetComponent<TMP_Text>();
            }
            return _text;
        }
    }
    
    public void HideInfinityPopupText() {
        _popupCanvas.gameObject.SetActive(false);
    }

    public IEnumerator Timer(float time) {
        yield return new WaitForSeconds(time);
        _popupCanvas.gameObject.SetActive(false);
    }

    public void ChangeTextLanguage(string text) {
        bool lastStateIsActive = _orderNumberTextBackground.activeSelf;
        if (!lastStateIsActive) {
            _orderNumberTextBackground.SetActive(true);
        }
        _orderNumberText.text = text;
        if (!lastStateIsActive) {
            _orderNumberTextBackground.SetActive(false);
        }
    }
    public void SetOrderNumVisual(string text) {
        _orderNumberTextBackground.SetActive(true);
        _orderNumberTextBackground.SetActive(true);
        _orderNumberText.text = text;
    }
    
    

    public void HideOrderNumVisual() {
        _orderNumberTextBackground.SetActive(false);
    }


}
