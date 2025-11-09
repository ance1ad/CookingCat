using System;
using System.Collections;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour {   
    // Тут логика вознаграждений
    
    [SerializeField] private GameObject _rewardContainer;
    [SerializeField] private Button _getGiftButton;
    [SerializeField] private TMP_Text _rewardTimer;
    [SerializeField] private GameObject _grayBack;
    private float timer;
    private bool readyToGift = false;
    public static RewardManager Instance {get; private set; }


    private void Start() {
        _getGiftButton.onClick.AddListener(GetGiftButton);
        
        SettingsManager.Instance.OnSwipeLanguage += OnSwipeLanguage;
        OnSwipeLanguage();
        _rewardContainer.SetActive(false);
    }

    private void OnSwipeLanguage() {
        secMeasurement = LocalizationManager.Get("SecMeasurement");
    }

    public void StartRewardTimerRoutine() {
        _rewardContainer.SetActive(true);
        StartCoroutine(RewardTimerRoutine());
    }


    private string secMeasurement;
    private IEnumerator RewardTimerRoutine() {
        timer = 10f;
        _grayBack.SetActive(true);
        _rewardTimer.gameObject.SetActive(true);
        while (timer > 0) {
            timer -= Time.deltaTime;
            _rewardTimer.text = timer.ToString("0") + secMeasurement;
            yield return null;
        }
        _grayBack.SetActive(false);
        _rewardTimer.gameObject.SetActive(false);
        readyToGift = true;
    }

    private void GetGiftButton() {
        if(!readyToGift) return;
        readyToGift = false;
        // Логика рандома выдачи денег
        CurrencyManager.Instance.UpdateCash(4000f, 1);
        StartCoroutine(RewardTimerRoutine());
    }
    
    


    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("Instance already created");
            return;
        }
        Instance = this;
    }

    private float _allAccuracy;
    private int _newGemsCount;
    private int _comboCount = 0;
    private float _comboBonus;
    private int _comboCountMaxInOrder = 0;
    private string _comboStat;
    // Общая стата по заказу
    private int _sumActions = 0;
    private int _sumRightIngredients= 0;

    public void CalculateOrderStatistic(Order order) {
        int countDishes = 0;
        float allReward = 0f;

        foreach (var element in order.dishStruct) {
            if (element.dish != null) {
                allReward += CalculateRewardForDish(element, order.elapsedTime, order.maxTime);
                countDishes++;
            }
        }

        // Учитываем лишние ингредиенты вообще не из заказа
        _sumActions += order.foreignExtraCount;

        // --- Финальный расчёт точности ---
        _allAccuracy = _sumRightIngredients > 0
            ? Mathf.Clamp01((float)_sumRightIngredients / _sumActions) * 100f
            : 0f;

        // --- Штраф за плохую точность ---
        float penaltyMultiplier = 1f;
        if (_allAccuracy < 30f) {
            float penalty = Mathf.Lerp(0f, 0.6f, (30f - _allAccuracy) / 30f); // до -60%
            penaltyMultiplier -= penalty;
        }

        float finalReward = allReward * penaltyMultiplier;


        // --- Комментарии игроку ---
        string comment;
        if (_allAccuracy < 50f) comment = LocalizationManager.Get("ManyErrors");
        else if (_allAccuracy < 65f) comment = LocalizationManager.Get("ClientUnhappy");
        else if (_allAccuracy < 75f) comment = LocalizationManager.Get("NotBad");
        else comment = LocalizationManager.Get("GoodJob");
        
        string timeInfo = order.maxTime - order.elapsedTime >= 0
            ? LocalizationManager.Get("TimeNotLate", order.elapsedTime)
            : LocalizationManager.Get("TimeLate", order.elapsedTime-order.maxTime);

        
        string accuracyText = LocalizationManager.Get("Accuracy", _allAccuracy, comment);

        
        
        
        
        CurrencyManager.Instance.SetOrderResult(finalReward, _newGemsCount, timeInfo, accuracyText, _comboStat);
        if (OrderManager.Instance.Level < 5) {
            if (_allAccuracy >= 70) {
                OrderManager.Instance.Level++;
                OrderManager.Instance._tryCounts = 0;
            }
            else {
                OrderManager.Instance.SayLevelResult();
                OrderManager.Instance._tryCounts++;
            }
        }
        
        // --- Сброс ---
        _sumRightIngredients  = 0;
        _sumActions = 0;
        _allAccuracy = 0f;
        _comboCountMaxInOrder = 0;
        _newGemsCount = 0;
    }



    private float CalculateRewardForDish(Order.DishStruct oderInfo, float elapsedTime, float maxTime) {
        int correct = oderInfo.correct; // я правильно добавил
        int extra = oderInfo.extra;
        int missing = oderInfo.missing;
        int total = oderInfo.total; // всего ингредиентов в блюде т.е правильных типо
        int playerAddedOrMissing = correct + extra + missing;
        
        
        
        _sumActions += playerAddedOrMissing;
        _sumRightIngredients += correct;
        
        
        
        float baseReward = CalculateBaseReward(oderInfo.dish.ingredients);
        
        
        float accuracy = Mathf.Clamp01((float)correct / playerAddedOrMissing); // в пределах заказика чтоб начислить гем
        
        float timeBonus = 0.5f + 0.5f * Mathf.Clamp01(1f - elapsedTime / maxTime);
        _comboBonus = 1f + Mathf.Min(_comboCount * 0.05f, 0.65f);
        
        if (accuracy >= 1f) {
            _newGemsCount++; // За собранный правильно заказ + алмазик
            _comboCount++;
            // _comboStat = $"Комбо x{_comboCount} (+{_comboCount*5}%)";
            _comboStat = LocalizationManager.Get("ComboX", _comboCount, _comboCount*5);
        }
        else {
            if (_comboCount > _comboCountMaxInOrder) {
                _comboCountMaxInOrder = _comboCount;
            }
            _comboCount = 0;
            if (_comboCountMaxInOrder == 0) {
                _comboStat = LocalizationManager.Get("ComboNone");
            }
            else {
                _comboStat =LocalizationManager.Get("ComboBreak", _comboCountMaxInOrder);
            }
        }
        
        return baseReward * accuracy * (1 + timeBonus) * _comboBonus; // за блюдо
    }


    private float CalculateBaseReward(KitchenObjectSO[] ingredients) {
        float sum = 0f;
        
        foreach (var ingredient in ingredients) {
            sum += ingredient.price;
        }
        return sum;
    }
}
