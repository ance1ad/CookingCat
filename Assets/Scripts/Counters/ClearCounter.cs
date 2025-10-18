using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClearCounter : BaseCounter, IKitchenObjectParent {
    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
    [SerializeField] private KitchenObjectSO _overcookedMeat;
    

    private Plate _plate;


    public override void Interact(Player player) {

        if (HasKitchenObject()) {
            // Хотим забрать
            if (!player.HasKitchenObject()) {
                if (GetKitchenObject() is Plate) {
                    // Возвращаю в изначальное положение
                    _counterTopPoint.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    _counterTopPoint.localPosition = new Vector3(0, 1.3f, 0);
                }

                GetKitchenObject().SetKitchenObjectParent(player);

            }
            // У пользователя тарелка
            else if (player.GetKitchenObject() is Plate) {
                _plate = player.GetKitchenObject() as Plate;
                _plate.AddIngredient(GetKitchenObject());
            }
            // Тарелка на столе
            else if (GetKitchenObject() is Plate) {
                _plate = GetKitchenObject() as Plate;
                _plate.AddIngredient(player.GetKitchenObject());
                
            }
            else {
                ShowPopupText("Стол уже занят другим предметом");
            }
        }

        else if (player.HasKitchenObject()) {
            // положить тарелко
            if (player.GetKitchenObject() is Plate) {
                // Выравниваем
                _counterTopPoint.localRotation = Quaternion.Euler(0f, 90f, 0f);
                _counterTopPoint.localPosition = new Vector3(-0.136f, 1.3f, 0.134f);
            }
            player.GetKitchenObject().SetKitchenObjectParent(this);
            // кладет
            UIManager.Instance.SetEButton(UIManager.UIButtonState.Take);
        }
        else {
            ShowPopupText("У вас нет предмета который можно положить");
        }
    }

    public override bool ThiefInteract(ThiefCat thief) {
        if (HasKitchenObject()) {
            GetKitchenObject().SetKitchenObjectParent(thief);
            return true;
        }
        return false;
    }

    public override bool CanTakeObject() => (HasKitchenObject() &&
        !(GetKitchenObject() is Plate) && 
        GetKitchenObject()._isFresh &&
        GetKitchenObject().GetKitchenObjectSO() != _overcookedMeat);





}
