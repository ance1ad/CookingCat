using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{

    [SerializeField] private List<BaseCounter> _rottenProductCounters;
    [SerializeField] private List<HiglightRules> rules;
    [Serializable]
    public class HiglightRules {
        public KitchenObjectSO obj;
        public List<BaseCounter> counters;
    }

    
    public static HighlightManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            // Debug.Log("There is no more 2 highlight managers!");
        }
        // Обьект на котором висит скрипт назначается в Instance
        Instance = this;
    }

    // Соответствие


    public List<BaseCounter> counters = new List<BaseCounter>();

    public void OnObjectTake(KitchenObjectSO takenObject) {
        // Сначала офаем все, потом вынести в отдельный метод
        OnObjectDrop();
        // Подсветка новых

        if (ProductIsRotten(takenObject)) {
            return;
        }
        
        foreach (var rule in rules) {
            if (rule.obj == takenObject) {
                foreach (var counter in rule.counters) {
                    counter.SetHighlight(true);
                }
                return;
            }
        }

    }

    private bool ProductIsRotten(KitchenObjectSO takenObject) {
        if (!takenObject.prefab.GetComponent<KitchenObject>()._isFresh) {
            foreach (var counters in _rottenProductCounters) {
                counters.SetHighlight(true);
            }
            // Debug.Log("Только выкинуть");
            return true;
        }
        return false;
    }

    public void OnObjectDrop() {
        foreach (var counter in counters) {
            counter.SetHighlight(false);
        }
    }


}
