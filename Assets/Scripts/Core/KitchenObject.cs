using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Кухонный обьект по сути что может лежать на столах - еда и тп
public class KitchenObject : MonoBehaviour {

    [SerializeField] private KitchenObjectSO _kitchenObjectSO;
    [SerializeField] private GameObject _particles;
    private IKitchenObjectParent _kitchenObjectParent;
    public bool _isFresh = true;




    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {
        if (this._kitchenObjectParent != null) {
            this._kitchenObjectParent.ClearKitchenObject();
        }

        // Initialize
        this._kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject()) {
            Debug.Log("Шото не так, тут не должен быть обьект");
        }

        this._kitchenObjectParent.SetKitchenObject(this);
        // Visual
        transform.parent = kitchenObjectParent.GetKitchenObjectTransform(); // куда положить
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public IKitchenObjectParent GetKitchenObjectParent() => _kitchenObjectParent;
    public KitchenObjectSO GetKitchenObjectSO() => _kitchenObjectSO;

    public void DestroyMyself() {
        _kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static void CreateKitchenObject(KitchenObjectSO _kitchenObjectSO, IKitchenObjectParent parent) {
        Transform kitchenObjectTransform = Instantiate(_kitchenObjectSO.prefab);
        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(parent);
    }

    public void SetUnfresh() {
        _isFresh = false;
        _particles.SetActive(true);
    }

}
