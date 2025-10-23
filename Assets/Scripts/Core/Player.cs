using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using static ThiefCat;

public class Player : MonoBehaviour, IKitchenObjectParent {
    [SerializeField] float _velocicy = 4f;
    [SerializeField] float _rotateSpeed = 8f;
    [SerializeField] GameInput _gameInput;
    [SerializeField] LayerMask _countersLayerMask;
    [SerializeField] private Transform _objectHoldPoint;
    [SerializeField] private float _interactDistance = 2f;
    [SerializeField] private Plate _plate;
    [SerializeField] private float _playerRadius = 0.7f;
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private PlayerVisual _visual;
    [SerializeField] public GameObject _holding;
    [SerializeField] private Transform _mouthPoint;
    [SerializeField] public Transform _plateEdge;
    [SerializeField] private ParticleSystem _particles;


    public GameObject visualPlate;

    private Vector3 lastDirection;
    public bool _isMoving;
    public bool _stopWalking = false;
    private BaseCounter selectedCounter;
    private ThiefCat selectedThief;
    private KitchenObject _kitchenObject;
    public bool _isFighting = false;



    // ��� ���������
    public static Player Instance { get; private set; }


    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    public event EventHandler<OnThiefInteractEventArgs> OnThiefInteract;
    public class OnThiefInteractEventArgs : EventArgs {
        public ThiefCat thief;
    }


    

    // ��� ���������
    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is no more 2 players!");
        }
        // ������ �� ������� ����� ������ ����������� � Instance
        Instance = this;
    }


    private void Update() {
        HandleMovement();
    }

    private void FixedUpdate() {
        HandleInteractions();
    }


    private void Start() {
        _gameInput.OnInteractAction += GameInput_OnInteractAction;
        _gameInput.OnAlternativeInteractAction += GameInput_OnAlternativeInteractAction;
    }

    private void GameInput_OnAlternativeInteractAction(object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.AlternativeInteract(this);
        }
    }


    private void GameInput_OnInteractAction(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
        // ���
        if (selectedThief != null && selectedThief.transform.position.x < 7f && selectedThief._readyToFight) {
            StartCoroutine(FightWithCat(selectedThief));
        }
    }


    private IEnumerator FightWithCat(ThiefCat newSelected) {
        StopWalking();
        _isFighting = true;
        newSelected.ForceStopCompletely();
        newSelected.StopAllCoroutines();
        newSelected._readyToFight = false;
        newSelected._state = CatState.Fighting;
        // ����������� ����� ������ 
        Vector3 dir = (newSelected.transform.position - transform.position).normalized;
        dir.y = 0f;

        // ������������� ����� ���� � ����� 
        transform.rotation = Quaternion.LookRotation(dir);
        newSelected.transform.rotation = Quaternion.LookRotation(-dir);

        // --- ����� ������� ---
        // ��� ��� ������� ���� �����
        Vector3 myTarget = transform.position + dir * 0.3f;
        Vector3 thiefTarget = newSelected.transform.position - dir * 0.4f;

        // ���������� ����� ����� ������
        float moveTime = 0.05f;
        float elapsed = 0f;
        Vector3 myStart = transform.position;
        Vector3 thiefStart = newSelected.transform.position;

        while (elapsed < moveTime) {
            elapsed += Time.deltaTime;
            float t = elapsed / moveTime;

            transform.position = Vector3.Lerp(myStart, myTarget, t);
            newSelected.transform.position = Vector3.Lerp(thiefStart, thiefTarget, t);

            yield return null;
        }

        // --- ������� ---
        newSelected._readyToFight = false;
        StartCoroutine(ThiefSuccessInfo(newSelected));
        newSelected.PlayCatFightParticle();
        _particles.Play();
        CameraShake.Instance.Shake(0.5f, 0.8f);

        yield return new WaitForSeconds(1.5f);
        _isFighting = false;

        newSelected.EnableAgentAgain();
        newSelected.GetOut();
        _stopWalking = false;

        // �������� ��� ��� �� �� ������
        Vector3 pos = transform.position;
        if (transform.position.x > 5.55f) {
            pos.x = 5.55f;
            StartCoroutine(MoveToPoint(1f, pos));
        }
    }




    private IEnumerator ThiefSuccessInfo(ThiefCat newSelected) {
        yield return new WaitForSeconds(1f);
        if (newSelected.HasKitchenObject() && !HasKitchenObject()) {
            newSelected.GetKitchenObject().SetKitchenObjectParent(this);
            MessageUI.Instance.SetText("�� �������� ����-����!", MessageUI.Emotions.happy);
        }
        else if (HasKitchenObject() && newSelected.HasKitchenObject()) {
            MessageUI.Instance.SetText("� ��� ������ ����", MessageUI.Emotions.sad);
            newSelected._readyToFight = true;
        }
        else {
            MessageUI.Instance.SetText("�� �������� ����-����!", MessageUI.Emotions.happy);
        }
    }

    private IEnumerator MoveToPoint(float moveDuration, Vector3 pos) {
        float elapsed = 0f;
        while (elapsed < moveDuration) {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, pos, elapsed / moveDuration);
            yield return null;
        }
    }


    private BaseCounter _lastCounter;
    private void HandleInteractions() {
        Vector2 inputVector = _gameInput.GetMovementVectorNormalized();
        Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y);

        if (direction != Vector3.zero)
            lastDirection = direction;

        if (Physics.Raycast(transform.position, lastDirection, out RaycastHit hit, _interactDistance, _countersLayerMask)) {
            ShowHolding(false);
            // ---- �������� ���� ----
            if (hit.transform.TryGetComponent(out ThiefCat thief)) {
                SetSelectedCounter(null);
                SetSelectedThief(thief);
                if (selectedThief != thief) {
                    selectedThief = thief;
                    _lastCounter = null;
                }
                return;
            }

            // ---- �������� ������ ----
            if (hit.transform.TryGetComponent(out BaseCounter counter)) {
                selectedThief = null; // ���� ����� ���

                if (counter != _lastCounter) {
                    SetSelectedCounter(counter);
                    _lastCounter = counter;
                }

                ShowIcon(true);
                return;
            }
        }
        else {
            ShowHolding(true);
        }

        // ---- ���� ������ �� ����� ----
        if (_lastCounter != null) {
            SetSelectedCounter(null);
            _lastCounter = null;
        }

        selectedThief = null;
        SetSelectedThief(null);
        ShowIcon(false);
    }


    public bool _stopHidingHold = false;
    private void ShowHolding(bool state) {
        if (_holding.activeSelf == state || _stopHidingHold) return;
        _holding.SetActive(state);
    }


    public void ShowIcon(bool state) {
        if (HasKitchenObject() && state) {
            _visual.ShowIcon(GetKitchenObject().GetKitchenObjectSO().sprite);
            return;
        }
        _visual.HideIcon();
    }


    public void StopWalking() {
        _stopWalking = true;
        _isMoving = false;
    }


    private void HandleMovement() {
        if (_stopWalking) {
            return;
        }
        Vector2 _inputVector = _gameInput.GetMovementVectorNormalized();
        Vector3 directoryVector = new Vector3(_inputVector.x, 0f, _inputVector.y);

        float moveDistance = _velocicy * Time.deltaTime;
        Vector3 startPosition = transform.position;

        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * _playerHeight,
            _playerRadius,
            directoryVector,
            moveDistance
        );

        if (!canMove) {
            // ������� X
            Vector3 dirX = new Vector3(directoryVector.x, 0, 0).normalized;
            canMove = directoryVector.x != 0 &&
                      !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _playerHeight,
                          _playerRadius, dirX, moveDistance);
            if (canMove) directoryVector = dirX;

            // ������� Z
            if (!canMove) {
                Vector3 dirZ = new Vector3(0, 0, directoryVector.z).normalized;
                canMove = directoryVector.z != 0 &&
                          !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _playerHeight,
                              _playerRadius, dirZ, moveDistance);
                if (canMove) directoryVector = dirZ;
            }
        }

        if (canMove) {
            transform.position += directoryVector * moveDistance;
        }

        // ������ _isMoving = true ������ ���� ������� ����������
        _isMoving = (transform.position != startPosition);

        // �� ������������ ������, ���� ���� ���� (����� �� ����� ��������� � �����)
        if (directoryVector != Vector3.zero) {
            transform.forward = Vector3.Slerp(transform.forward, directoryVector, Time.deltaTime * _rotateSpeed);
        }
    }





    private void SetSelectedCounter(BaseCounter counter) {
        selectedCounter = counter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    private void SetSelectedThief(ThiefCat thief) {
        OnThiefInteract?.Invoke(this, new OnThiefInteractEventArgs {
            thief = thief
        });
    }

    public Transform GetKitchenObjectTransform() => _objectHoldPoint;

    public KitchenObject GetKitchenObject() => _kitchenObject;

    public bool HasKitchenObject() => _kitchenObject != null;




    public void ClearKitchenObject() {
        _kitchenObject = null;
        HighlightManager.Instance.OnObjectDrop();

        if (!visualPlate.activeSelf) {
            visualPlate.SetActive(true);
        }
    }

    private Coroutine _coroutine;

    public void SetKitchenObject(KitchenObject kitchenObject) {
        if(_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _kitchenObject = kitchenObject;
        if(kitchenObject is Plate) {
            visualPlate.SetActive(false); // ���� ������
        }
        HighlightManager.Instance.OnObjectTake(_kitchenObject.GetKitchenObjectSO());
        // ������� �����
        if (UnityEngine.Random.value < .1 &&
            !(_kitchenObject is Plate) &&
            !string.IsNullOrEmpty(_kitchenObject.GetKitchenObjectSO().justification)) {
            _coroutine = StartCoroutine(EatProductRoutine());
        }
    }

    private IEnumerator EatProductRoutine() {
        yield return new WaitForSeconds(3f);
        if (HasKitchenObject() && _kitchenObject._isFresh) {
            MessageUI.Instance.SetText(_kitchenObject.GetKitchenObjectSO().justification, MessageUI.Emotions.eated);
            MoveToPoint(_mouthPoint, 1f);
            yield return new WaitUntil(() => _moveCoroutine == null);
            GetKitchenObject().DestroyMyself();
        }
    }

    public Coroutine _moveCoroutine { get; private set; }
    public void MoveToPoint(Transform point, float speed) {
        if(_moveCoroutine != null) {
            StopCoroutine(_moveCoroutine);
        }

        _moveCoroutine = StartCoroutine(ObjectMoveToPoint(point, speed));
    }



    private IEnumerator ObjectMoveToPoint(Transform point, float speed) {
        
        Transform obj = GetKitchenObject().transform;
        while (Vector3.Distance(obj.position, point.position) > 0.2f) {
            if (!HasKitchenObject()) {
                StopCoroutine(_moveCoroutine);
                _moveCoroutine = null;
            }
            obj.position = Vector3.MoveTowards(
                obj.position,
                point.position,
                speed * Time.deltaTime
            );
            yield return null;
        }
        obj.position = point.position;
        _moveCoroutine = null;
    }
}
