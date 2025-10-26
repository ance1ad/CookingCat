using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {

    [SerializeField] public Transform _counterTopPoint;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private GameObject[] _outlineObjects;
    public bool outlineIsActive = false;

    private float intensity = 0.2f;

    private KitchenObject _kitchenObject;

    public Transform CounterTopPoint => _counterTopPoint;


    private void Awake() {
        ShowOutline(false);
    }


    public virtual void Interact(Player player) {
        Debug.Log("Interact");
    }


    public virtual bool ThiefInteract(ThiefCat thief) {
        Debug.Log("ThiefInteract");
        return false;
    }


    public virtual bool CourierInteract(CourierCat thief) {
        Debug.Log("CourierInteract");
        return false;
    }

    public virtual void AlternativeInteract(Player player) {
        Debug.Log("AlternativeInteract");
    }

    // Получить точку спауна
    public Transform GetKitchenObjectTransform() => _counterTopPoint;

    public virtual bool CanTakeObject() {
        return true;
    }

    public KitchenObject GetKitchenObject() => _kitchenObject;

    public bool HasKitchenObject() => _kitchenObject != null;


    public void SetKitchenObject(KitchenObject kitchenObject) {
        _kitchenObject = kitchenObject;
    }

    public void ClearKitchenObject() {
        _kitchenObject = null;
    }


    private Coroutine pulseRoutine;

    public void SetHighlight(bool active) {
        if (active && !HasKitchenObject()) {
            if (pulseRoutine == null)
                pulseRoutine = StartCoroutine(PulseHighlight());
            ShowOutline(true);


            outlineIsActive = true;
        }
        else {
            if (pulseRoutine != null) StopCoroutine(pulseRoutine);
            pulseRoutine = null;
            foreach (var renderer in renderers)
                renderer.material.DisableKeyword("_EMISSION");
            ShowOutline(false);
            outlineIsActive = false;
        }
    }


    public void ShowOutline(bool state) {
        foreach (var elements in _outlineObjects) {
            elements.gameObject.SetActive(state);
        }
    }



    private IEnumerator PulseHighlight() {
        float t = 0f;
        while (true) {
            t += Time.deltaTime * 2f;
            float pulse = (Mathf.Sin(t) + 1f) / 2f;
            foreach (var renderer in renderers) {
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", highlightColor * (pulse * intensity));
            }
            yield return null;
        }
    }


}
