using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThiefCat : MonoBehaviour, IKitchenObjectParent {
    [SerializeField] private List<BaseCounter> _containers = new List<BaseCounter>(); // холодосы и тп где точно есть
    [SerializeField] private List<BaseCounter> _counters = new List<BaseCounter>(); // Это уже где пользователь
    [SerializeField] private Transform _exit;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<KitchenObjectSO> _pizzes;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private Transform _objectHoldPoint;

    public bool _readyToFight = true;

    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";
    private const string PLAYER_FIGHTING_STATE_VARIABLE = "IsFighting";

    private BaseCounter _chooseContainer;



    private NavMeshAgent _agent;
    private bool _hasReachedTarget = false;
    private Transform _targetContainer;
    private Coroutine _thiefCycle;



    public static bool catRunCicleGoing = true;
    public static bool catRunNew;

    public enum CatState { 
        Idle, 
        Stealing, 
        Fighting, 
        Leaving 
    }
    public CatState _state = CatState.Idle;

    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        _thiefCycle = StartCoroutine(ThiefLoop());
    }


    private void Update() {
        // Проверяем флаг остановки
        if (_state == CatState.Fighting) {
            GetComponent<NavMeshAgent>().updateRotation = false;
            _agent.updateRotation = false;
        }
        else {
            _agent.updateRotation = true;
        }


        if (_targetContainer == null || _state == CatState.Fighting) return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance) {
            if (!_hasReachedTarget) {
                _hasReachedTarget = true;
                StartCoroutine(RotateToTarget());
            }
        }
    }

    public void StopThiefCycle() {
        if(_thiefCycle != null) {
            Debug.Log("Цикл остановился");
            GetOut();
            StopCoroutine(_thiefCycle);
            _thiefCycle = null;
        }
    }
    public void StartThiefCycle() {
        Debug.Log("Цикл возобновился");
        _thiefCycle = StartCoroutine(ThiefLoop());
    }





    private IEnumerator ThiefLoop() {
        while (catRunCicleGoing) {
            yield return new WaitForSeconds(Random.Range(1f,2f));

            Debug.Log(_state);
            catRunNew = Random.value > 0f;
            if (_state == CatState.Leaving || _state == CatState.Stealing) {
                continue;
            }
            if (catRunNew) {
                if (HasKitchenObject()) {
                    GetKitchenObject().DestroyMyself();
                }
                RunToNewCounter();
                Debug.Log("Cat run!");
            }
            else {
                _agent.ResetPath();
                _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
                Debug.Log("Cat lazy!");
            }
            
        }
    }


    private void RunToNewCounter() {
        _state = CatState.Stealing;
        _hasReachedTarget = false;
        _chooseContainer = null;
        // Выбираем из грилей духовок или столов
        for (int i = 0; i < _counters.Count; i++) {
            if (_counters[i].CanTakeObject()) {
                _chooseContainer = _counters[i];
                break;
            }
        }
        // Выбираем из холодосов
        if (_chooseContainer == null) {
            _chooseContainer = _containers[Random.Range(0, _containers.Count)];
        }

        _targetContainer = _chooseContainer.transform;
        Debug.Log("Выбран новый контейнер " + _chooseContainer);
        _agent.SetDestination(_targetContainer.position);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
        MessageUI.Instance.SetText("Кот-вор собирается что-то украсть!", MessageUI.Emotions.shocked);
    }


   

    public IEnumerator RotateToTarget() {
        if (_state == CatState.Fighting || _state == CatState.Leaving) {
            yield break;
        }
        // Игнорируем высоту
        Vector3 targetDirection = _targetContainer.position - transform.position;
        targetDirection.y = 0f; // не учитываем высоту
        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        float rotationSpeed = 7f;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f ) {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            if(_state == CatState.Fighting || _state == CatState.Leaving) {
                yield break;
            }
            yield return null;
        }

        // Последний шаг: точно выравниваем
        transform.rotation = targetRotation;

        if (_chooseContainer != null && !_chooseContainer.ThiefInteract(this)) {
            _hasReachedTarget = false;
            _targetContainer = null;
            RunToNewCounter();
        }
        else {
            GetOut();
        }
    }


    public void ForceStopCompletely() {
        if (_agent == null) return;

        // Полностью отключаем перемещение агента
        _agent.ResetPath();
        _agent.isStopped = true;
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.enabled = false;

        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
        _animator.SetBool(PLAYER_FIGHTING_STATE_VARIABLE, true);
    }


    public void EnableAgentAgain() {
        if (_agent == null) return;

        _agent.enabled = true;
        _agent.updatePosition = true;
        _agent.updateRotation = true;
        _agent.isStopped = false;
    }





    public void GetOut() {
        if (_state == CatState.Leaving) return;
        _animator.SetBool(PLAYER_FIGHTING_STATE_VARIABLE, false);
        _state = CatState.Leaving;

        StartCoroutine(GetOutRoutine());
    }



    private IEnumerator GetOutRoutine() {
        _targetContainer = null;
        _chooseContainer = null;
        _hasReachedTarget = false;

        _agent.speed = 10f;
        // Останавливаем текущие корутины взаимодействия
        _agent.SetDestination(_exit.position);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);

        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
            yield return null;

        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
        _readyToFight = true;

        _agent.speed = 7f;

        ThiefSuccess();
        _state = CatState.Leaving;
        yield return new WaitForSeconds(4f);
        // Добежал
        _state = CatState.Idle;
        _thiefCycle = StartCoroutine(ThiefLoop());
    }


    private void ThiefSuccess() {
        if (HasKitchenObject()) {
            if (_pizzes.Contains(GetKitchenObject().GetKitchenObjectSO())) {
                MessageUI.Instance.SetText("Кот-вор украл " + GetKitchenObject().GetKitchenObjectSO().declension, MessageUI.Emotions.angry);
            }
            else {
                MessageUI.Instance.SetText("Кот-вор украл " + GetKitchenObject().GetKitchenObjectSO().declension, MessageUI.Emotions.sad);
            }
        }
    }

    public void PlayCatFightParticle() {
        _particles.Play();
    }



    public Transform GetKitchenObjectTransform() => _objectHoldPoint;

    public KitchenObject GetKitchenObject() => _kitchenObject;

    public bool HasKitchenObject() => _kitchenObject != null;


    public void SetKitchenObject(KitchenObject kitchenObject) {
        _kitchenObject = kitchenObject;
        if (Player.Instance.HasKitchenObject()) {
            HighlightManager.Instance.OnObjectTake(Player.Instance.GetKitchenObject().GetKitchenObjectSO());
        }
    }

    public void ClearKitchenObject() {
        _kitchenObject = null;
    }
}
