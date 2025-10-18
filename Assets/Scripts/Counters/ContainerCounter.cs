using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter {

    [SerializeField] public KitchenObjectSO _kitchenObjectSO;

    public event EventHandler OnContainerCounterInteract;


    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            KitchenObject.CreateKitchenObject(_kitchenObjectSO, player);
            if(UnityEngine.Random.value < 0.1f) {
                player.GetKitchenObject().SetUnfresh();
                ShowPopupText("Этот продукт пропал, его только выбросить");
            }
            HighlightManager.Instance.OnObjectTake(_kitchenObjectSO);
            OnContainerCounterInteract?.Invoke(this, EventArgs.Empty);
        }
        else if (player.GetKitchenObject().GetKitchenObjectSO() == _kitchenObjectSO && player.GetKitchenObject()._isFresh) {
            Destroy(player.GetKitchenObject().gameObject);
            HighlightManager.Instance.OnObjectDrop();
            OnContainerCounterInteract?.Invoke(this, EventArgs.Empty);
            
        }
        else if (!player.GetKitchenObject()._isFresh) {
            ShowPopupText("Этот продукт пропал, его только выбросить");
        }
        else {
            ShowPopupText("Этот обьект нельзя положить сюда");
        }
    }

    public override bool ThiefInteract(ThiefCat thief) {
        KitchenObject.CreateKitchenObject(_kitchenObjectSO, thief);
        OnContainerCounterInteract?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public override bool CanTakeObject() {
        return true;
    }

    // Вонизаврия
}
