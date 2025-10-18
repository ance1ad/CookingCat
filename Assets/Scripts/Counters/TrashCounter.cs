using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCounter : BaseCounter,IHasProgress {
    [SerializeField] private Transform _dropPoint;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float Progress;
    }

    public event Action OnKitchenObjectTake;
    private float count = 0;




    public override void Interact(Player player) {
        if (player.HasKitchenObject()) {

            if (player.GetKitchenObject() is Plate) {
                ShowPopupText("ѕоднос нельз€ выкинуть, соберите заказ");
                return;
            }
            StartCoroutine(DropObject(player));
            return;
        }
        ShowPopupText("” вас в руках ничего нет чтобы выкинуть");
    }

    private IEnumerator DropObject(Player player) {
        KitchenObject objectKO = player.GetKitchenObject();
        player.StopWalking();
        player.MoveToPoint(player._plateEdge, 2f);
        StartCoroutine(Progress());
        yield return new WaitUntil(() => player._moveCoroutine == null);
        objectKO.transform.localScale = objectKO.transform.localScale * 0.7f;
        player.MoveToPoint(_dropPoint, 7f);
        // игрок не может ходить
        ShowPopupText("¬ы выкинули " + objectKO.GetKitchenObjectSO().objectName.ToLower());
        yield return new WaitUntil(() => player._moveCoroutine == null);
        objectKO.DestroyMyself();
        UIManager.Instance.SetEButton(UIManager.UIButtonState.Take);
        player._stopWalking = false;
    }

    private IEnumerator Progress() {
        count = 0;
        while (count < .5f) {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                Progress = count / .5f
            });
            count += Time.deltaTime;
            yield return null;
        }
        OnKitchenObjectTake.Invoke();
    }
}
