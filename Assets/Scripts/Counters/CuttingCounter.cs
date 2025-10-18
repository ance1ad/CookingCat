using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UIManager;

public class CuttingCounter : BaseCounter, IHasProgress {

    [SerializeField] CuttedObjectSO[] _cuttedObjectsSO;




    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float Progress;
    }

    public event Action OnKitchenObjectTake;



    private CuttedObjectSO output;
    private int cutCount = 0;


    public override void Interact(Player player) {
        if (player.HasKitchenObject() && !HasKitchenObject()) {
            if (!player.GetKitchenObject()._isFresh) {
                ShowPopupText("Этот предмет пропал, выкинь его");
                return;
            }
            output = GetOutputForInput(player.GetKitchenObject().GetKitchenObjectSO());
            if (output != null) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _objectIsSliced = false;
                UIManager.Instance.SetFButton(UIManager.UIButtonState.Slice, GetKitchenObject().GetKitchenObjectSO().sprite);
                UIManager.Instance.SetGrayFButton(false);
                UIManager.Instance.SetEButton(UIManager.UIButtonState.Take);
                cutCount = 0;
            }
            else {
                ShowPopupText("Этот предмет нельзя нарезать");
            }
        }
        else if (HasKitchenObject()) {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
            else if (player.HasKitchenObject() && player.GetKitchenObject() is Plate) {
                Plate plate = player.GetKitchenObject() as Plate;
                plate.AddIngredient(GetKitchenObject());
            }
            else {
                ShowPopupText("Стол занят другим предметом");
            }
        }
        else {
            ShowPopupText("У вас нет предмета для нарезки");
        }
        if (!HasKitchenObject()) {
            OnKitchenObjectTake?.Invoke();
            UIManager.Instance.SetGrayFButton(true);
        }

    }

    public override void AlternativeInteract(Player player) {
        if (_objectIsSliced) {
            ShowPopupText("Обьект уже нарезан, заберите его");
            return;
        }
        if (output != null && HasKitchenObject() ) {
            cutCount++;

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                Progress = (float)cutCount / output.maxSliceCount
            });

            
            if (cutCount == output.maxSliceCount) { 
                GetKitchenObject().DestroyMyself();
                KitchenObject.CreateKitchenObject(output.output, this);
                output = null;
                UIManager.Instance.SetGrayFButton(true);
                UIManager.Instance.SetEButton(UIButtonState.Take);
                _objectIsSliced = true;
            }
        }
        
    }

    private bool _objectIsSliced = false;
    public bool ObjectIsSliced() => _objectIsSliced;

    public override bool CanTakeObject() => HasKitchenObject() && GetKitchenObject()._isFresh;

    public override bool ThiefInteract(ThiefCat thief) {
        if (HasKitchenObject()) {
            GetKitchenObject().SetKitchenObjectParent(thief);
            OnKitchenObjectTake?.Invoke();
            return true;
        }
        return false;
    }


    private CuttedObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO) {
        foreach (CuttedObjectSO item in _cuttedObjectsSO) {
            if (item.input == kitchenObjectSO) {
                return item;
            }
        }
        return null;
    }




}
