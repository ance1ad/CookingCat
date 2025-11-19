using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class ContainerCounter : BaseCounter {

    [SerializeField] public KitchenObjectSO _kitchenObjectSO;
    [SerializeField] public ContainerCounterVisual _visual;


    public event Action OnContainerCounterInteract;


    public int _productCount { get; private set; } = 7;


    private void Start() {
        _productCount = 7;
        _visual.ShowCountIcon(2f, _productCount.ToString());
    }


   

    public void AddProduct(int count) {
        if (count <= 0) {
            return;
        }
        _productCount += count;
        StartCoroutine(CloseDoorRoutine());
        _visual.ShowPlusProducts(count, _productCount);
        OnContainerCounterInteract?.Invoke();
    }

    public bool TryUseProduct() {
        if(_productCount == 0) {
            Debug.Log("У вас 0 продуктов");
            return false;
        }
        StartCoroutine(CloseDoorRoutine());
        _productCount -= 1;
        _visual.ShowMinusProduct(_productCount);
        OnContainerCounterInteract?.Invoke();
        return true;
    }


    private bool _mayUse = true;
    private IEnumerator CloseDoorRoutine() {
        _mayUse = false;
        yield return new WaitForSeconds(.5f);
        _mayUse = true;
    }


    public override void Interact(Player player) {
        ScaleInteract();
        if (!_mayUse) {
            return;
        }
        // Оддат игроку
        if (!player.HasKitchenObject()) {
            
            if (!TryUseProduct()) {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("NeedCourier"));
                return;
            }


            KitchenObject.CreateKitchenObject(_kitchenObjectSO, player);
            HighlightManager.Instance.OnObjectTake(_kitchenObjectSO);
            if (UnityEngine.Random.value < 0.07f && !TutorialManager.Instance.TutorialStarted) {
                player.GetKitchenObject().SetUnfresh();
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("ProductRotten"));
                SoundManager.Instance.PlaySFX("ProductRotten");
                return;
            }

        }
        // Положить в холодос
        else if (player.GetKitchenObject().GetKitchenObjectSO() == _kitchenObjectSO && player.GetKitchenObject()._isFresh) {
            // Destroy(player.GetKitchenObject().gameObject);
            player.GetKitchenObject()?.DestroyMyself();
            AddProduct(1);
            HighlightManager.Instance.OnObjectDrop();
        }
        else if (!player.GetKitchenObject()._isFresh) {
            MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("ProductRotten"));
            SoundManager.Instance.PlaySFX("ProductRotten");
        }
        else {
            MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("DontPutObject"));
        }
    }

    public override bool ThiefInteract(ThiefCat thief) {
        OnContainerCounterInteract?.Invoke();
        if (TryUseProduct()) {
            KitchenObject.CreateKitchenObject(_kitchenObjectSO, thief);
            return true;
        }
        return false;
    }


    public override bool CourierInteract(CourierCat courier) {
        // Принес скокато
        //AddProduct(courier.AddProduct(_kitchenObjectSO));
        AddProduct(courier.currentProductCount);
        return true;
    }

    public override bool CanTakeObject() => _productCount > 0;

}
