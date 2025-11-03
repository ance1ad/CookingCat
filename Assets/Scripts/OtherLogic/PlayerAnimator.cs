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

    private const float _footstepInterval = 0.35f;
    private float _footstepTimer;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
        // StartCoroutine(PlaySoundRoutine());
    }

    private void Update()
    {
        _animator.SetBool(PLAYER_WALKING_STATE_VARIABLE, _player._isMoving);
        if (_player._isMoving) {
            _footstepTimer -= Time.deltaTime;
            if (_footstepTimer <= 0) {
                PlayRandomFootstep();
                _footstepTimer =  _footstepInterval;
            }
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
