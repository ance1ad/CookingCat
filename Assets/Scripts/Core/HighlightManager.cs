using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

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
            Debug.Log("There is no more 2 highlight managers!");
        }
        // ������ �� ������� ����� ������ ����������� � Instance
        Instance = this;
    }

    // ������������


    public List<BaseCounter> counters = new List<BaseCounter>();

    public void OnObjectTake(KitchenObjectSO takenObject) {
        // ������� ����� ���, ����� ������� � ��������� �����
        OnObjectDrop();
        // ��������� �����

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
            Debug.Log("������ ��������");
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
