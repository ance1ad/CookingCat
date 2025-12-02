using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private const string PLAYER_WALKING_STATE_VARIABLE = "IsWalking";
    private const string PLAYER_FIGHTING_STATE_VARIABLE = "IsFighting";
    [SerializeField] private Player _player;
    [SerializeField] private SoundData[] _stepSounds; // 4 типа

    private float _footstepBase = 0.35f;
    private float _footstepInterval = 0.35f;
    private float _footstepTimer;
    private float _walkingSpeed = 1f;
    private float _baseSpeed = 4.5f;
    
    void Start() {
        _animator = GetComponent<Animator>();
        PlayerUpgradeManager.Instance.OnUpgrade += OnUpgrade;
        ChangeStats();
    }

    private void OnUpgrade() {
        ChangeStats();
    }

    private void ChangeStats() {
        _animator.speed = PlayerUpgradeManager.Instance.PlayerSpeed / _baseSpeed;
        _walkingSpeed = _animator.speed;
        if (_walkingSpeed != 1f) {
            _footstepInterval = _footstepBase - (float)0.08 * _walkingSpeed;
        }
    }

    private void Update()
    {
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, _player._isMoving);
        if (_player._isMoving) {
            _animator.speed = _walkingSpeed;
            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer <= 0) {
                PlayRandomFootstep();
                _footstepTimer =  _footstepInterval;
            }
        }
        else {
            _animator.speed = 1f;
        }
        _animator.SetBool(PLAYER_FIGHTING_STATE_VARIABLE, _player._isFighting);
    }

    private int lastIndex = -1;
    private void PlayRandomFootstep() {
        if (_stepSounds.Length == 0) return;
        int index;
        do {
            index = Random.Range(0, _stepSounds.Length-1);
        } while (index == lastIndex);
        lastIndex = index;
        SoundManager.Instance.PlaySFX(_stepSounds[index].id);
    }
}
