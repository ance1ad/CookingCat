using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour {   
    // Тут логика вознаграждения за заказ
    public static RewardManager Instance {get; private set; }

    
    
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
        _allAccuracy = (float)_sumRightIngredients  / _sumActions;
        _allAccuracy = Mathf.Clamp01(_allAccuracy) * 100f;

        Debug.Log($"Точность: {_allAccuracy}% ({_sumRightIngredients }/{_sumActions})");

        // --- Штраф за плохую точность ---
        float penaltyMultiplier = 1f;
        if (_allAccuracy < 40f) {
            float penalty = Mathf.Lerp(0f, 0.3f, (40f - _allAccuracy) / 40f); // до -30%
            penaltyMultiplier -= penalty;
        }
        float finalReward = allReward * penaltyMultiplier;

        
        // --- Комментарии игроку ---
        string comment;
        if (_allAccuracy < 50f) comment = "Много ошибок";
        else if (_allAccuracy < 65f) comment = "Клиент недоволен...";
        else if (_allAccuracy < 75f) comment = "Неплохо!";
        else comment = "Отличная работа!";

        string timeInfo = order.maxTime - order.elapsedTime >= 0
            ? $"Время: {order.elapsedTime:0} сек. успел!"
            : $"Время: {(order.elapsedTime - order.maxTime):0} сек. опоздание";

        string accuracyText = $"Точность: {_allAccuracy:0}%\n{comment}";

        CurrencyManager.Instance.SetOrderResult(finalReward, _newGemsCount, timeInfo, accuracyText, _comboStat);

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
        float baseReward = oderInfo.dish.baseReward;
        
        
        float accuracy = Mathf.Clamp01((float)correct / playerAddedOrMissing); // в пределах заказика чтоб начислить гем
        
        _allAccuracy += accuracy;
        
        float timeBonus = 0.5f + 0.5f * Mathf.Clamp01(1f - elapsedTime / maxTime);
        _comboBonus = 1f + Mathf.Min(_comboCount * 0.05f, 0.65f);
        
        if (accuracy >= 1f) {
            _newGemsCount++; // За собранный правильно заказ + алмазик
            _comboCount++;
            _comboStat = $"Комбо x{_comboCount} (+{_comboCount*5}%)";
        }
        else {
            if (_comboCount > _comboCountMaxInOrder) {
                _comboCountMaxInOrder = _comboCount;
            }
            _comboCount = 0;
            if (_comboCountMaxInOrder == 0) {
                _comboStat = $"Комбо: нет";
            }
            else {
                _comboStat = $"Комбо прервано на x{_comboCountMaxInOrder}";
            }
        }
        

        
        Debug.Log("baseReward "+ baseReward);
        Debug.Log("accuracy "+ accuracy);
        Debug.Log("timeBonus "+ timeBonus);
        Debug.Log("_comboBonus "+ _comboBonus);

        
        return baseReward * accuracy * (1 + timeBonus) * _comboBonus; // за блюдо
    }
}
