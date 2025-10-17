using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectParent {

    Transform GetKitchenObjectTransform();

    KitchenObject GetKitchenObject();

    bool HasKitchenObject();

    void SetKitchenObject(KitchenObject kitchenObject);

    void ClearKitchenObject();
}
