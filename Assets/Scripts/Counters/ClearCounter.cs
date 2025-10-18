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
            // ����� �������
            if (!player.HasKitchenObject()) {
                if (GetKitchenObject() is Plate) {
                    // ��������� � ����������� ���������
                    _counterTopPoint.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    _counterTopPoint.localPosition = new Vector3(0, 1.3f, 0);
                }

                GetKitchenObject().SetKitchenObjectParent(player);

            }
            // � ������������ �������
            else if (player.GetKitchenObject() is Plate) {
                _plate = player.GetKitchenObject() as Plate;
                _plate.AddIngredient(GetKitchenObject());
            }
            // ������� �� �����
            else if (GetKitchenObject() is Plate) {
                _plate = GetKitchenObject() as Plate;
                _plate.AddIngredient(player.GetKitchenObject());
                
            }
            else {
                ShowPopupText("���� ��� ����� ������ ���������");
            }
        }

        else if (player.HasKitchenObject()) {
            // �������� �������
            if (player.GetKitchenObject() is Plate) {
                // �����������
                _counterTopPoint.localRotation = Quaternion.Euler(0f, 90f, 0f);
                _counterTopPoint.localPosition = new Vector3(-0.136f, 1.3f, 0.134f);
            }
            player.GetKitchenObject().SetKitchenObjectParent(this);
            // ������
            UIManager.Instance.SetEButton(UIManager.UIButtonState.Take);
        }
        else {
            ShowPopupText("� ��� ��� �������� ������� ����� ��������");
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
