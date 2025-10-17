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
        iconImage.sprite = icon;
        newIcon.SetActive(true);
    }

    public void HideIcon() {
        newIcon.SetActive(false);
    }
}
