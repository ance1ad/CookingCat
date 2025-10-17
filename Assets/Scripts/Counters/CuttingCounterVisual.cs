using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour {

    [SerializeField] CuttingCounter _cuttingCounter;
    private const string CUT = "Cut";
    private Animator animator;


    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        _cuttingCounter.OnProgressChanged += CuttingCounterCut;
    }

    private void CuttingCounterCut(object sender, System.EventArgs e) {
        animator.SetTrigger(CUT);
    }
}
