using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class ClientCat : MonoBehaviour {
    [SerializeField] private Transform _exit;
    [SerializeField] private Transform _table;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private TMP_Text _popupText;

    
    
    
    private NavMeshAgent _agent;
    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";
    public static ClientCat Instance { get; private set; }
    
    private bool _clientIsGone = false;
    public bool ClientIsGone => _clientIsGone;
    
    private void Awake() {
        if (Instance != null) {
            Debug.Log("2 clients wth?");
            return;
        }
        Instance = this;
        _agent = GetComponent<NavMeshAgent>();
        _popupText.text = LocalizationManager.Get("ClientRole");
    }

    private void Start() {
        SettingsManager.Instance.OnSwipeLanguage += OnOnSwipeLanguage;
    }

    private void OnOnSwipeLanguage() {
        _popupText.text = LocalizationManager.Get("ClientRole");
    }


    public void GoSayOrder() {
        if (isActiveAndEnabled) {
            StartCoroutine(GoSayOrderRoutine());
        }
    }
    public void GoEatOrder() {
        StartCoroutine(GoEatOrderRoutine());
    }
    private IEnumerator GoSayOrderRoutine() {
        _agent.SetDestination(_table.position);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
        yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
        // Дошел
        _clientIsGone = false;
    }
    

    
    private IEnumerator GoEatOrderRoutine() {
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
        _agent.SetDestination(_exit.position);
        yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
        // Дошел
        _clientIsGone = true;
    }

    public void GivePlate(Plate plate) {
        plate.transform.SetParent(_holdPoint);
        plate.transform.localPosition = Vector3.zero;
        plate.transform.localRotation = Quaternion.identity;
    }
}
