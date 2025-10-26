using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatesCounter : BaseCounter {

    [SerializeField] private KitchenObjectSO _plate;
    [SerializeField] private Transform _fakePlateForVisual;


    private Vector3 stackOffset = new Vector3(0f, 0.1f, 0f);
    private Vector3 nextLocalOffset = new Vector3(0f, 0.1f, 0f);
    private Transform topPoint;
    private float timer;
    private float timeToCreatePlate = 3f;
    private int maxPlates = 5;

    private bool _mayGivePlate = true;



    private void Start() {
        topPoint = CounterTopPoint;
        OrderManager.Instance.OnOrderCompleted += InstanceOnOnOrderCompleted;
    }

    private void InstanceOnOnOrderCompleted() {
        _mayGivePlate = true;
    }


    private void Update() {
        timer += Time.deltaTime;

        // добиваем визуалом
        if (timer >= timeToCreatePlate && topPoint.childCount < maxPlates) {
            CreatePlateVisual();
            timer = 0;
        }
    }

    public override void Interact(Player player) {
        // Выдаем тарелку если создана
        if (OrderManager.Instance.orderIsAppointed) {
            MessageUI.Instance.ShowPlayerPopup("Сначала выполните взятый заказ");
            return;
        }
        if (!player.HasKitchenObject()  && _mayGivePlate && topPoint.childCount > 0) {
           
            KitchenObject.CreateKitchenObject(_plate, player);
            _mayGivePlate = false;
            HighlightManager.Instance.OnObjectTake(_plate);

             // самый верхний
            Destroy(topPoint.GetChild(topPoint.childCount-1).gameObject); // просто визуально удалить сверху
            nextLocalOffset -= stackOffset;
            return;
        }

        if (!_mayGivePlate) {
            MessageUI.Instance.ShowPlayerPopup("Вы уже взяли поднос, выполните заказ");
        }
    }



    public void CreatePlateVisual() { 
        Transform plate = Instantiate(_fakePlateForVisual, topPoint);
        plate.transform.localPosition = nextLocalOffset;
        plate.transform.localRotation = Quaternion.identity;
        nextLocalOffset += stackOffset; }
}
