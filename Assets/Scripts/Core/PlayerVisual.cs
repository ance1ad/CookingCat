using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviour {
    [SerializeField] private GameObject iconTemplate;
    [SerializeField] private GameObject canvas;
    [SerializeField] public GameObject _popupCanvas;
    
    
    private GameObject newIcon;
    private Image iconImage;
    private TMP_Text _popupText;
    private float showPopupTime = 2.5f;

    private void Start() {
        _popupText = _popupCanvas.transform.GetChild(1).GetComponent<TMP_Text>();
        iconImage = newIcon.transform.GetChild(1).GetComponent<Image>();
    }


    private void Awake() {
        newIcon = Instantiate(iconTemplate);
        newIcon.transform.SetParent(canvas.transform, false);
        canvas.SetActive(true);
    }

    public void ShowIcon(Sprite icon) {
        if (!newIcon.activeSelf) {
            newIcon.SetActive(true);
        }

        if (iconImage.sprite != icon) {
            iconImage.sprite = icon;
        }
    }

    public void HideIcon() {
        if (newIcon.activeSelf) {
            newIcon.SetActive(false);
        }
    }
    
    private Coroutine corutine;
    public void ShowPopupText(string text) {
        _popupCanvas.SetActive(true);
        _popupText.text = text;
        if (corutine != null) {
            StopCoroutine(corutine);
        }
        corutine = StartCoroutine(Timer(showPopupTime));
    }

    public IEnumerator Timer(float time) {
        yield return new WaitForSeconds(time);
        _popupCanvas.SetActive(false);
        corutine = null;
    }
}
