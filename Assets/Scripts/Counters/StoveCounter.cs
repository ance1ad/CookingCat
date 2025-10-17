using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseCounter;

public class StoveCounter : BaseCounter, IHasProgress {

    [SerializeField] private FryingObjectSO[] _fryingObjectSOArray;
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject switcher;
    private Plate _plate;


    private FryingObjectSO transitionToCooked;
    private FryingObjectSO transitionToOvercooked;

    private float timer = 0f;
    private float timeForCooked;
    private bool meatIsCooked = false;
    private bool meatIsOvercooked = false;


    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float Progress;
    }

    public event Action OnKitchenObjectTake;


    private void Update() {
        if (HasKitchenObject() && !meatIsOvercooked) {
            timer += Time.deltaTime;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                Progress = timer / timeForCooked
            });
            FryMeat();
        }
    }


    private void FryMeat() {
        // ������������ ������� �������������� ���� � ��� ������ �.� � ������������� ���� ��� ������ output
        if (transitionToOvercooked == null) {
            transitionToOvercooked = transitionToCooked;
            timeForCooked = transitionToCooked.fryingTimerMax;
            meatIsCooked = true;
        }

        if (timer >= transitionToCooked.fryingTimerMax && !meatIsCooked) {
            timer = 0f;
            ShowPopupText("���� ������!");
            GetKitchenObject().DestroyMyself();
            KitchenObject.CreateKitchenObject(transitionToCooked.output, this);
            timeForCooked = transitionToOvercooked.fryingTimerMax;
            meatIsCooked = true;
        }
        else if (timer >= transitionToOvercooked.fryingTimerMax) {
            ShowPopupText("���� ������������");
            GetKitchenObject().DestroyMyself();
            KitchenObject.CreateKitchenObject(transitionToOvercooked.output, this);
            OnKitchenObjectTake?.Invoke();
            timer = 0f;
            meatIsOvercooked = true;
        }
    }



    public override void Interact(Player player) {
        if (player.HasKitchenObject() && !HasKitchenObject()) {
            if (!player.GetKitchenObject()._isFresh) {
                ShowPopupText("���� ������� ������, ������ ���");
                return;
            }
            transitionToCooked = GetOutputForInput(player.GetKitchenObject().GetKitchenObjectSO());

            // ������������
            if (transitionToCooked != null) {
                transitionToOvercooked = GetOutputForInput(transitionToCooked.output);
                timeForCooked = transitionToCooked.fryingTimerMax;
                meatIsCooked = false;
                meatIsOvercooked = false;
                player.GetKitchenObject().SetKitchenObjectParent(this);
                particles.SetActive(true);
                switcher.SetActive(true);
            }
            else {
                ShowPopupText("������� " + player.GetKitchenObject().GetKitchenObjectSO().objectName + " ������ ��������");
            }

        }
        else if (HasKitchenObject()) {

            // � ����
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                player.visualPlate.SetActive(true);
                ClearData();
            }
            // �� �������
            if (player.GetKitchenObject() is Plate && HasKitchenObject()) {
                _plate = player.GetKitchenObject() as Plate;
                _plate.AddIngredient(GetKitchenObject());
                ClearData();
            }
        }
        else {
            ShowPopupText("�������� ���� � ���� ����� �������� ���");
        }
    }



    private FryingObjectSO GetOutputForInput(KitchenObjectSO fryingObjectSO) {
        foreach (FryingObjectSO item in _fryingObjectSOArray) {
            if (item.input == fryingObjectSO) {
                return item;
            }
        }
        return null;
    }

    private void ClearData() {
        particles.SetActive(false);
        switcher.SetActive(false);
        OnKitchenObjectTake?.Invoke();
        timer = 0f;
    }


    public override bool ThiefInteract(ThiefCat thief) {
        if (HasKitchenObject()) {
            GetKitchenObject().SetKitchenObjectParent(thief);
            ClearData();
            return true;
        }
        return false;
    }

    public override bool CanTakeObject() => HasKitchenObject() &&
        !meatIsOvercooked;

}
