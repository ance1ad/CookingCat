using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVisual : MonoBehaviour {
    [SerializeField] private GameObject iconTemplate;
    [SerializeField] private GameObject canvas;
    private GameObject newIcon;
    private Image iconImage;

    private void Awake() {
        newIcon = Instantiate(iconTemplate);
        newIcon.transform.SetParent(canvas.transform, false);
        canvas.SetActive(true);
        iconImage = newIcon.transform.GetChild(1).GetComponent<Image>();
    }

    public void ShowIcon(Sprite icon) {
        if (!newIcon.activeSelf) {
            newIcon.SetActive(true);
            Debug.Log("Смена иконки");
        }

        if (iconImage.sprite != icon) {
            iconImage.sprite = icon;
        }
    }

    public void HideIcon() {
        if (newIcon.activeSelf) {
            newIcon.SetActive(false);
            Debug.Log("Крыто сшито иконки");
        }
    }
}
