using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";
    [SerializeField] private Player _player;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, _player._isMoving);
    }
}
