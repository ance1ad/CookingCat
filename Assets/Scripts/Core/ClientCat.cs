using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClientCat : MonoBehaviour {
    [SerializeField] private Transform _exit;
    [SerializeField] private Transform _table;
    [SerializeField] private Animator _animator;

    private NavMeshAgent _agent;
    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";



    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        StartCoroutine(Timer());
    }

    private IEnumerator Timer() {
        while (true) {
            _agent.SetDestination(_table.position);
            _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
            yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
            _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);

            yield return new WaitForSeconds(1f);
            _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, true);
            _agent.SetDestination(_exit.position);
            yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
            _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, false);
            yield return new WaitForSeconds(1f);
        }
    }
}
