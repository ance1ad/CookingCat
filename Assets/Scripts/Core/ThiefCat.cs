using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ThiefCat : MonoBehaviour, IKitchenObjectParent {
    [SerializeField] private List<BaseCounter> _containers = new List<BaseCounter>(); // �������� � �� ��� ����� ����
    [SerializeField] private List<BaseCounter> _counters = new List<BaseCounter>(); // ��� ��� ��� ������������
    [SerializeField] private Transform _exit;
    [SerializeField] private Animator _animator;
    [SerializeField] private List<KitchenObjectSO> _pizzes;
    [SerializeField] private ParticleSystem _particles;

    public bool stopWalking = false;
    public bool _readyToFight = true;

    [SerializeField] private Transform _objectHoldPoint;
    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";
    private BaseCounter _chooseContainer;



    private NavMeshAgent _agent;
    private bool _hasReachedTarget = false;
    private bool _catGetOut = true;
    private Transform _targetContainer;
    private Coroutine _thiefCycle;



    public static bool catRunCicleGoing = true;
    public static bool catRunNew;




    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        _thiefCycle = StartCoroutine(ThiefLoop());
    }


    private void Update() {
        // ��������� ���� ���������
        if (stopWalking) {
            GetComponent<NavMeshAgent>().updateRotation = false;
            StartCoroutine(StopWalkingTemporarily());
            _agent.updateRotation = false;
        }
        else {
            _agent.updateRotation = true;
        }


        if (_targetContainer == null || stopWalking) return;

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance) {
            if (!_hasReachedTarget) {
                _hasReachedTarget = true;
                StartCoroutine(RotateToTarget());
            }
        }
    }

    public void StopThiefCycle() {
        if(_thiefCycle != null) {
            Debug.Log("���� �����������");
            GetOut();
            StopCoroutine(_thiefCycle);
            _thiefCycle = null;
        }
    }
    public void StartThiefCycle() {
        Debug.Log("���� ������������");
        _thiefCycle = StartCoroutine(ThiefLoop());
    }


    private IEnumerator StopWalkingTemporarily() {

        _agent.isStopped = true;
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);

        // ���� ���� stopWalking �� ������ false
        while (stopWalking) {
            yield return null;
        }
        GetComponent<NavMeshAgent>().updateRotation = true;
        // ���������� ��������
        _agent.isStopped = false;
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
        stopWalking = false;
    }



    private IEnumerator ThiefLoop() {
        while (catRunCicleGoing) {
            yield return new WaitForSeconds(Random.Range(1f,2f));


            catRunNew = Random.value > .1f;
            if (!_catGetOut) {
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
        _catGetOut = false;
        _hasReachedTarget = false;
        _chooseContainer = null;
        // �������� �� ������ ������� ��� ������
        for (int i = 0; i < _counters.Count; i++) {
            if (_counters[i].CanTakeObject()) {
                _chooseContainer = _counters[i];
                break;
            }
        }
        // �������� �� ���������
        if (_chooseContainer == null) {
            _chooseContainer = _containers[Random.Range(0, _containers.Count)];
        }

        _targetContainer = _chooseContainer.transform;
        _agent.SetDestination(_targetContainer.position);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
        MessageUI.Instance.SetText("���-��� ���������� ���-�� �������!", MessageUI.Emotions.shocked);
    }


   

    public IEnumerator RotateToTarget() {
        // ���������� ������
        Vector3 targetDirection = _targetContainer.position - transform.position;
        targetDirection.y = 0f; // �� ��������� ������
        targetDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        float rotationSpeed = 7f;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.5f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        // ��������� ���: ����� �����������
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

    public void GetOut() {
        stopWalking = false;
        if (isLeaving) return;
        StartCoroutine(GetOutRoutine());
    }

    private bool isLeaving = false;

    private IEnumerator GetOutRoutine() {
        isLeaving = true;
        _targetContainer = null;
        _chooseContainer = null;
        _hasReachedTarget = false;

        _agent.speed = 17f;
        // ������������� ������� �������� ��������������
        StopCoroutine(nameof(RotateToTarget));
        _agent.SetDestination(_exit.position);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);

        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
            yield return null;

        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
        _readyToFight = true;

        _agent.speed = 7f;

        if (HasKitchenObject()) {
            if (_pizzes.Contains(GetKitchenObject().GetKitchenObjectSO())) {
                MessageUI.Instance.SetText("���-��� ����� " + GetKitchenObject().GetKitchenObjectSO().declension, MessageUI.Emotions.angry);
            }
            else {
                MessageUI.Instance.SetText("���-��� ����� " + GetKitchenObject().GetKitchenObjectSO().declension, MessageUI.Emotions.sad);
            }
            
        }
        yield return new WaitForSeconds(4f);

        isLeaving = false;
        _catGetOut = true;
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
