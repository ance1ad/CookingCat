using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour {

    [SerializeField] CuttingCounter _cuttingCounter;
    private const string CUT = "Cut";
    private Animator animator;
    [SerializeField] private SoundData[] _sliceSounds; // 4 типа


    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        _cuttingCounter.OnProgressChanged += CuttingCounterCut;
    }


    private void CuttingCounterCut(object sender, System.EventArgs e) {
        animator.SetTrigger(CUT);
        GetRandomCutSound();
    }

    
    private int lastIndex = -1;
    private void GetRandomCutSound() {
        if (_sliceSounds.Length == 0) return;
        int index;
        do {
            index = Random.Range(0, _sliceSounds.Length-1);
        } while (index == lastIndex);
        lastIndex = index;
        SoundManager.Instance.PlaySFX(_sliceSounds[index].id);
    }
}
