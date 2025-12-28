using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class ClientCat : MonoBehaviour {
    [SerializeField] private Transform _exit;
    [SerializeField] private Transform _table;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private TMP_Text _popupText;
    [SerializeField] private List<Texture> _textures;
    [SerializeField] private Renderer _catRenderer;

    
    
    
    private NavMeshAgent _agent;
    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";
    public static ClientCat Instance { get; private set; }
    
    
    private void Awake() {
        if (Instance != null) {
            Debug.Log("2 clients wth?");
            return;
        }
        Instance = this;
        _agent = GetComponent<NavMeshAgent>();
    }



    private void Start() {
        SettingsManager.Instance.OnSwipeLanguage += OnSwipeLanguage;
        OnSwipeLanguage();
    }

    private void OnSwipeLanguage() {
        _popupText.text = LocalizationManager.Get("ClientRole");
    }


    private void SetRandomColor() {
        _catRenderer.material.mainTexture = _textures[Random.Range(0, _textures.Count)];
    }
    
    
    public void GoSayOrder() {
        if (isActiveAndEnabled) {
            StartCoroutine(GoSayOrderRoutine());
            SetRandomColor();
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
    }
    

    
    private IEnumerator GoEatOrderRoutine() {
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
        _agent.SetDestination(_exit.position);
        yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
    }

    public void GivePlate(Plate plate) {
        plate.transform.SetParent(_holdPoint);
        plate.transform.localPosition = Vector3.zero;
        plate.transform.localRotation = Quaternion.identity;
    }
}
