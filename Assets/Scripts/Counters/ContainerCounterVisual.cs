using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerCounterVisual : MonoBehaviour {


    [SerializeField] private ContainerCounter _containerCounter;
    // ����� ����������� 
    [SerializeField] private Image _background;
    [SerializeField] private TMP_Text _text;
    // +-
    [SerializeField] private TMP_Text _textPM;
    [SerializeField] private Image _minusBackground;
    [SerializeField] private Image _plusBackground;




    private const string STATE_NAME = "OpenDoor";
    private Animator animator;
    private float _timeToShow = 2f;


    private void Start() {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        _containerCounter.OnContainerCounterInteract += OnInteractAnimation;
        HideElements();
    }

    private void OnInteractAnimation() {
        animator.enabled = true;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingOpenDoor = info.IsName("OpenDoor") && info.normalizedTime < 1f;
        SoundManager.Instance.PlaySFX("Holodos");


        if (isPlayingOpenDoor) return; // �� ��������� ���� �� ���������
        animator.Play(STATE_NAME, 0, 0f);
    }


    private Coroutine iconsCoroutine;
    public void ShowCountIcon(float time, string text) {
        if (iconsCoroutine != null) {
            StopCoroutine(iconsCoroutine);
        }
        iconsCoroutine = StartCoroutine(ShowIconRoutine(time, text));
    }

    private IEnumerator ShowIconRoutine(float time, string text) {
        _text.text = text;
        _background.enabled = true;
        _text.enabled = true;
        yield return new WaitForSeconds(time);
        HideElements();
    }

    private void HideElements() {
        // All
        _background.enabled = false;
        _text.enabled = false;
        // ������� ��������
        _textPM.enabled = false;
        _minusBackground.enabled = false;
        _plusBackground.enabled = false;
    }

    public void ShowMinusProduct(int allCount) {
        _textPM.enabled = true;
        _textPM.text = "-1";
        _minusBackground.enabled = true;
        _plusBackground.enabled = false;
        ShowCountIcon(_timeToShow, allCount.ToString());
    }

    public void ShowPlusProducts(int count, int allCount) {
        _textPM.enabled = true;
        _textPM.text = "+ " + count.ToString();
        _plusBackground.enabled = true;
        _minusBackground.enabled = false;
        ShowCountIcon(_timeToShow, allCount.ToString());
    }

}
