using System.Collections;
using UnityEngine;
using System;
using static ThiefCat;

public class Player : MonoBehaviour, IKitchenObjectParent {
    [SerializeField] float _velocity = 4f;
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

    [SerializeField] private GameObject visualPlate;

    private Vector3 lastDirection;
    public bool _isMoving;
    public bool _stopWalking = false;
    private BaseCounter selectedCounter;
    private ThiefCat selectedThief;
    private KitchenObject _kitchenObject;
    public bool _isFighting = false;



    // Для синглтона
    public static Player Instance { get; private set; }


    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    public event EventHandler<OnThiefInteractEventArgs> OnThiefInteract;
    public class OnThiefInteractEventArgs : EventArgs {
        public ThiefCat thief;
    }


    

    // Для синглтона
    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is no more 2 players!");
        }
        // Обьект на котором висит скрипт назначается в Instance
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
        PlayerUpgradeManager.Instance.OnUpgrade += UpdatePlayerStats;
    }

    private void UpdatePlayerStats() {
        _velocity = PlayerUpgradeManager.Instance.PlayerSpeed;
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
        // Вор
        if (selectedThief != null && selectedThief.transform.position.x < 7f && selectedThief._readyToFight) {
            StartCoroutine(FightWithCat(selectedThief));
        }
    }


    private IEnumerator FightWithCat(ThiefCat newSelected) {
        StopWalking();
        SoundManager.Instance.PlaySFX("CatFight");
        _isFighting = true;
        newSelected.ForceStopCompletely();
        newSelected.StopAllCoroutines();
        newSelected._readyToFight = false;
        newSelected._state = CatState.Fighting;
        // Направление между котами 
        Vector3 dir = (newSelected.transform.position - transform.position).normalized;
        dir.y = 0f;

        // Разворачиваем лицом друг к другу 
        transform.rotation = Quaternion.LookRotation(dir);
        newSelected.transform.rotation = Quaternion.LookRotation(-dir);

        // --- точки встречи ---
        // Мой кот подойдёт чуть ближе
        Vector3 myTarget = transform.position + dir * 0.3f;
        Vector3 thiefTarget = newSelected.transform.position - dir * 0.4f;

        // Перемещаем обоих котов плавно
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

        // --- эффекты ---
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

        // Проверка что кот не за картой
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
            MessageUI.Instance.SetText(LocalizationManager.Get("ThiefGetOut"), MessageUI.Emotions.happy);
        }
        else if (HasKitchenObject() && newSelected.HasKitchenObject()) {
            MessageUI.Instance.SetText(LocalizationManager.Get("HandsNotFreeForThief"), MessageUI.Emotions.sad);
            newSelected._readyToFight = true;
        }
        else {
            MessageUI.Instance.SetText(LocalizationManager.Get("ThiefGetOut"), MessageUI.Emotions.happy);
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
        Vector2 inputVector = _gameInput.GetMovementVector().normalized;
        Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y);

        if (direction != Vector3.zero)
            lastDirection = direction;

        if (Physics.Raycast(transform.position, lastDirection, out RaycastHit hit, _interactDistance, _countersLayerMask)) {
            ShowHolding(false);
            // ---- Проверка кота ----
            if (hit.transform.TryGetComponent(out ThiefCat thief)) {
                SetSelectedCounter(null);
                SetSelectedThief(thief);
                if (selectedThief != thief) {
                    selectedThief = thief;
                    _lastCounter = null;
                }
                return;
            }

            // ---- Проверка столов ----
            if (hit.transform.TryGetComponent(out BaseCounter counter)) {
                selectedThief = null; // кота рядом нет

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

        // ---- Если никого не нашли ----
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
    
    public void StartWalking() {
        _stopWalking = false;
    }


    private float joystickDeadZone = 0f; 
    private float rotateThreshold = 0f;

    private void HandleMovement() {
        if (_stopWalking) return;

        Vector2 inputVector = _gameInput.GetMovementVector();
        float inputStrength = inputVector.magnitude;

        if (inputStrength < joystickDeadZone) {
            _isMoving = false;
            return;
        }

        Vector3 desiredDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveDistance = _velocity * Time.deltaTime;
        Vector3 startPos = transform.position;

        bool canMove = !Physics.CapsuleCast(
            transform.position,
            transform.position + Vector3.up * _playerHeight,
            _playerRadius,
            desiredDir,
            moveDistance
        );

        Vector3 moveDir = desiredDir;

        if (!canMove) {
            // Проверяем X и Z
            Vector3 dirX = new Vector3(desiredDir.x, 0, 0).normalized;
            bool canMoveX = desiredDir.x != 0 &&
                            !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _playerHeight,
                                _playerRadius, dirX, moveDistance);

            Vector3 dirZ = new Vector3(0, 0, desiredDir.z).normalized;
            bool canMoveZ = desiredDir.z != 0 &&
                            !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * _playerHeight,
                                _playerRadius, dirZ, moveDistance);

            if (canMoveX) moveDir = dirX;
            else if (canMoveZ) moveDir = dirZ;
            else moveDir = Vector3.zero;
        }

        // 💡 Если движение частично заблокировано — снижаем скорость (эффект "скольжения" по стене)
        float effectiveSpeed = _velocity;
        if (!canMove && moveDir != Vector3.zero) {
            effectiveSpeed *= 0.4f; // двигается в 2.5 раза медленнее вдоль препятствия
        }

        if (moveDir != Vector3.zero)
            transform.position += moveDir * (effectiveSpeed * Time.deltaTime);

        // --- Поворот ---
        if (canMove && inputStrength > rotateThreshold) {
            transform.forward = Vector3.Slerp(transform.forward, desiredDir, Time.deltaTime * _rotateSpeed);
        }
        else if (!canMove && inputStrength > 0.9f) {
            transform.forward = Vector3.Slerp(transform.forward, desiredDir, Time.deltaTime * (_rotateSpeed * 0.25f));
        }

        _isMoving = moveDir.magnitude > 0.1f && transform.position != startPos;
        
        // --- Поворот к выбранному контейнеру, если стоим на месте ---
        if (!_isMoving && selectedCounter != null) {
            Vector3 lookDir = (selectedCounter.transform.position - transform.position);
            lookDir.y = 0f; // чтобы не тянул вверх/вниз
            if (lookDir.sqrMagnitude > 0.001f) {
                // плавный поворот к объекту
                transform.forward = Vector3.Slerp(
                    transform.forward,
                    lookDir.normalized,
                    Time.deltaTime * (_rotateSpeed * 0.8f)
                );
            }
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
        visualPlate.SetActive(false);
    }

    private Coroutine _coroutine;

    public void SetKitchenObject(KitchenObject kitchenObject) {
        if(_coroutine != null) {
            StopCoroutine(_coroutine);
        }
        _kitchenObject = kitchenObject;
        if(kitchenObject is Plate) {
            visualPlate.SetActive(false); // Взял поднос
        }
        else {
            visualPlate.SetActive(true); 
        }
        HighlightManager.Instance.OnObjectTake(_kitchenObject.GetKitchenObjectSO());
        // Сжирает хавку
        
        
        
        // СМЕНИТЬ ПОТОМ
        if (UnityEngine.Random.value < 1 &&
            !(_kitchenObject is Plate) &&
            !string.IsNullOrEmpty(_kitchenObject.GetKitchenObjectSO().justification)
            && !TutorialManager.Instance.TutorialStarted ) {
            
            
            MessageUI.Instance.SetText(LocalizationManager.Get("CatWantEat", _kitchenObject.GetKitchenObjectSO().declension), MessageUI.Emotions.happy);
            _coroutine = StartCoroutine(EatProductRoutine());
        }
    }

    private IEnumerator EatProductRoutine() {
        yield return new WaitForSeconds(5f);
        
        if (HasKitchenObject() && _kitchenObject._isFresh) {
            MessageUI.Instance.SetText(_kitchenObject.GetKitchenObjectSO().justification, MessageUI.Emotions.eated);
            SoundManager.Instance.PlaySFX("Happy");
            MoveToPoint(_mouthPoint, 1f);
            yield return new WaitUntil(() => _objectMoveCoroutine == null);
            if (HasKitchenObject()) {
                GetKitchenObject().DestroyMyself();
            }
        }
    }

    public Coroutine _objectMoveCoroutine { get; private set; }
    public void MoveToPoint(Transform point, float speed) {
        if(_objectMoveCoroutine != null) {
            StopCoroutine(_objectMoveCoroutine);
        }

        _objectMoveCoroutine = StartCoroutine(ObjectMoveToPoint(point, speed));
    }



    private IEnumerator ObjectMoveToPoint(Transform point, float speed) {
        Transform obj = GetKitchenObject()?.transform;
    
        // Если уже нечего двигать — сразу выйти
        if (obj == null || point == null) {
            _objectMoveCoroutine = null;
            yield break;
        }

        while (true) {
            // Проверяем, не уничтожен ли объект или точка
            if (obj == null || point == null) {
                _objectMoveCoroutine = null;
                yield break;
            }

            // Проверяем дистанцию
            if (Vector3.Distance(obj.position, point.position) <= 0.2f)
                break;

            // Проверяем, что у нас ещё есть объект в руках
            if (!HasKitchenObject()) {
                _objectMoveCoroutine = null;
                yield break;
            }

            obj.position = Vector3.MoveTowards(
                obj.position,
                point.position,
                speed * Time.deltaTime
            );

            yield return null;
        }
        SoundManager.Instance.PlaySFX("TrashDrop");
        

        if (obj != null && point != null)
            obj.position = point.position;

        _objectMoveCoroutine = null;
    }

}
