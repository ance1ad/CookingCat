using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OvenVisual : MonoBehaviour
{
    [SerializeField] private OvenCounter _oven;
    [SerializeField] private GameObject _iconTemplate;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Sprite _pizzaIsReady;
    private List<GameObject> icons = new List<GameObject>();
    private Animator animator;
    private const string STATE_NAME = "OpenDoor";



    private void Start() {
        _oven.OnIndridientAddedInOven += Oven_OnIndridientAddedInOven;
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    private void Oven_OnIndridientAddedInOven(OvenCounter.IngredientAddedArgs obj) {
        GameObject newIcon = Instantiate(_iconTemplate);
        newIcon.transform.GetChild(1).GetComponent<Image>().sprite = obj.icon;
        newIcon.transform.SetParent(_canvas.transform, false);
        newIcon.SetActive(true);
        icons.Add(newIcon);
    }


    public void PlayAnimation() {
        animator.enabled = true;
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        bool isPlayingOpenDoor = info.IsName("OpenDoor") && info.normalizedTime < 1f;
        if (isPlayingOpenDoor) return; // не запускаем пока не закончила
        animator.Play(STATE_NAME, 0, 0f);
    }


    public void ClearIcons() {
        foreach (var icon in icons) {
            Destroy(icon);
        }
    }

    public void SetPizzaReady() {
        ClearIcons();
        GameObject newIcon = Instantiate(_iconTemplate);
        newIcon.transform.GetChild(1).GetComponent<Image>().sprite = _pizzaIsReady;
        newIcon.transform.SetParent(_canvas.transform, false);
        newIcon.SetActive(true);
        icons.Add(newIcon);

    }
}
