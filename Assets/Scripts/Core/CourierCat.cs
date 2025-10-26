using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static ThiefCat;

public class CourierCat : MonoBehaviour, IKitchenObjectParent {


    [Serializable]
    public class ProductToContainer {
        public KitchenObjectSO product;
        public BaseCounter container;
    }

    [SerializeField] private List<ProductToContainer> _productContainerPairs = new();
    [SerializeField] private Transform _exit;
    [SerializeField] private Animator _animator;

    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";
    private NavMeshAgent _agent;
    private bool _readyToNewTarget = true; // Дошел до контейнера
    private bool _readyToNewCycle = true;
    private KitchenObject _kitchenObject;

    private void Awake() {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start() {
        ProductManager.Instance.OnProductCardAdded += InstanceOnOnProductCardAdded;
    }

    private Queue<Dictionary<KitchenObjectSO, int>> _ordersQueue = new Queue<Dictionary<KitchenObjectSO, int>>();
    private void InstanceOnOnProductCardAdded(Dictionary<KitchenObjectSO, int> newOrder) {
        _ordersQueue.Enqueue(newOrder);
        if (_doingWorkCoroutine == null) {
            _doingWorkCoroutine = StartCoroutine(DoingQueue());
        }
        // int count = 1;
        // Debug.Log("______________Список всех заказов: ______________");
        // foreach (var order in _ordersQueue) {
        //     Debug.Log("Новый заказ " + count);
        //     foreach (var order2 in order) {
        //         if (order2.Value != 0) {
        //             Debug.Log(order2.Key.objectName + " - " + order2.Value);
        //         }
        //     }
        //     count++;
        // }
    }

    private Coroutine _doingWorkCoroutine;
    private IEnumerator DoingQueue() {
        while (_ordersQueue.Count > 0) {
            yield return new WaitUntil(() => _readyToNewCycle);
            FormingOrder();
        }
        _doingWorkCoroutine = null;
    }

    private void FormingOrder() {
        _readyToNewCycle = false;
        // Ищем соответствия ящику и кол-ву
        Dictionary<BaseCounter, int> newTargets = new();
        foreach (var product in _ordersQueue.Dequeue()) {
            // Нужно вытащить все продукты количество которых != 0 и засунуть в newTargets
            if (product.Value != 0) {
                newTargets.TryAdd(GetCounterFromObject(product.Key), product.Value);
            }
        }

        StartCoroutine(CourierCycle(newTargets));
    }

    private BaseCounter GetCounterFromObject(KitchenObjectSO obj) {
        foreach (var objectToContainer in _productContainerPairs) {
            if (objectToContainer.product == obj) {
                return objectToContainer.container;
            }
        }
        return null;
    }

    public int currentProductCount; // кол-во конкретного продукта к цели к которой он идёт
    
    public IEnumerator CourierCycle(Dictionary<BaseCounter, int> targets) {
        // Пробегаемся по всем контейнерам
        foreach (var target in targets) {
            DelieveProduct(target.Key);
            currentProductCount = target.Value;
            yield return new WaitUntil(() => _readyToNewTarget);
        }
        GetOut();
    }


    private void DelieveProduct(BaseCounter counter) {
        _readyToNewTarget = false;
        _agent.SetDestination(counter.transform.position);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
        StartCoroutine(TargetWentRoutine(counter));
    }



    public IEnumerator TargetWentRoutine(BaseCounter counter) {
        yield return new WaitUntil(() => !_agent.pathPending && _agent.hasPath);

        yield return new WaitUntil(() => _agent.remainingDistance < _agent.stoppingDistance);


        // Дошел, поворачиваем
        Vector3 targetDirection = counter.transform.position - transform.position;
        // Игнорируем высоту
        targetDirection.y = 0f; 
        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        float rotationSpeed = 7f;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
         }

        // Точно выравниваем
        transform.rotation = targetRotation;
        counter.CourierInteract(this);
        _readyToNewTarget = true;
    }


    public void GetOut() {
        // Проверка как в воре не нужна, его никто не собьёт
        StartCoroutine(GetOutRoutine());
    }



    private IEnumerator GetOutRoutine() {
        _readyToNewTarget = false;
        _agent.speed = 10f;
        _agent.SetDestination(_exit.position);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);

        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
            yield return null;


        _readyToNewTarget = true;
        _readyToNewCycle = true;
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);

        _agent.speed = 7f;
    }




    public Transform GetKitchenObjectTransform() => null;

    public KitchenObject GetKitchenObject() => _kitchenObject;

    public bool HasKitchenObject() => _kitchenObject != null;


    public void SetKitchenObject(KitchenObject kitchenObject) {
        _kitchenObject = kitchenObject;
    }

    public void ClearKitchenObject() {
        _kitchenObject = null;
    }
}
