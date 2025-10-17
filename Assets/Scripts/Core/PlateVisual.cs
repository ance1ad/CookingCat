using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateVisual : MonoBehaviour {
    [SerializeField] private Plate _plate;
    [SerializeField] private GameObject iconTemplate;
    [SerializeField] private GameObject canvas;



    private void Start() {
        _plate.OnIndridientAdded += Plate_OnIndridientAdded;
    }

    private void Plate_OnIndridientAdded(Plate.IngredientAddedArgs obj) {
        GameObject newIcon = Instantiate(iconTemplate);
        newIcon.transform.GetChild(1).GetComponent<Image>().sprite = obj.icon;
        newIcon.transform.SetParent(canvas.transform, false);
        newIcon.SetActive(true);
    }
}
