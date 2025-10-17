using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderCounterVisual : MonoBehaviour {
    [SerializeField] public GameObject _iconTemplate;
    [SerializeField] public GameObject _generalCanvas;
    [SerializeField] public GameObject _canvasPizza;
    [SerializeField] public GameObject _canvasBurger;
    [SerializeField] public GameObject _canvasDrink;

    [SerializeField] public GameObject _timerText;
    [SerializeField] public GameObject _timerCanvas;
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _bigCloseButton;
    [SerializeField] private Image _level;
    [SerializeField] private Image _new;

    [SerializeField] private ThiefCat _thief; // воришка



    [SerializeField] private TMP_Text orderStatusText;
    private bool showOrder = true;


    public List<GameObject> pizzaIcons = new List<GameObject>();
    public List<GameObject> burgerIcons = new List<GameObject>();
    public List<GameObject> drinkIcons = new List<GameObject>();


    public void AddIcons() {
        Order order = OrderManager.Instance.CurrentOrder;
        if (order.first != null) {
            foreach (var ingredient in order.first.ingredients) {
                AddIcon(_canvasPizza, ingredient, pizzaIcons);
            }
        }
        if (order.second != null) {
            foreach (var ingredient in order.second.ingredients) {
                AddIcon(_canvasBurger, ingredient, burgerIcons );
            }
        }
        if (order.drink != null) {
            foreach (var ingredient in order.drink.ingredients) {
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



    public IEnumerator TimerToCloseOrderInfo(float time) {
        _thief.StopThiefCycle();
        float timeNow = 0f;
        while (timeNow < time) {
            _timerText.GetComponent<TMP_Text>().text = ((int)((time - timeNow)+1) + " сек");
            _level.fillAmount = (1f - timeNow / time);

            timeNow += Time.deltaTime;
            // обновлять потом поле с временем до закрытия
            yield return null;
        }
        _generalCanvas.gameObject.SetActive(false);
        _thief.StartThiefCycle();
    }


    public void ShowCanvas() {
        Debug.Log("Вызов канваса");
        if (showOrder) {
            orderStatusText.text = "Новый заказ";
            showOrder = false;
            _new.gameObject.SetActive(true);
            _timerCanvas.gameObject.SetActive(true);
            _closeButton.gameObject.SetActive(false);
            _bigCloseButton.SetActive(false);
        }
        else {
            orderStatusText.text = "Выполнение заказа";
            _thief.StopThiefCycle();
            showOrder = true;
            _new.gameObject.SetActive(false);
            _timerCanvas.gameObject.SetActive(false);
            _closeButton.gameObject.SetActive(true);
            _bigCloseButton.SetActive(true);
        }
        _generalCanvas.gameObject.SetActive(true);

    }

    public void HideCanvas() {
        _generalCanvas.gameObject.SetActive(false);
        _thief.StartThiefCycle();
    }

}
