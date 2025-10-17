using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour {


    
    [SerializeField] ContainerCounter _containerCounter;
    private const string STATE_NAME = "OpenDoor";
    private Animator animator;



    private void Start() {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        _containerCounter.OnContainerCounterInteract += ContainerCounterInteract;
    }

    private void ContainerCounterInteract(object sender, System.EventArgs e) {
        animator.enabled = true;


        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingOpenDoor = info.IsName("OpenDoor") && info.normalizedTime < 1f;

        if (isPlayingOpenDoor) return; // не запускаем пока не закончила
        animator.Play(STATE_NAME, 0, 0f);
    }

}
