using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {

    [SerializeField] private Image _image;
    // т.к юнити не поддерживает перетаскивание интерфейсов мы обойдем это
    [SerializeField] private GameObject hasProgressGO;
    [SerializeField] private GameObject[] visualCanvas;


    private IHasProgress hasProgress;
    private void Awake() {
        ProgressBarVisible(false);
    }

    private void Start() {
        hasProgress = hasProgressGO.GetComponent<IHasProgress>();
        if(hasProgress == null) {
            Debug.LogError("Обьект "+ hasProgress + " не имплементирует интерфейс IHasProgress");
            return;
        }
        hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;
        hasProgress.OnKitchenObjectTake += OnKitchenObjectTake;
        _image.fillAmount = 0f;
    }

    private void OnKitchenObjectTake() {
        ProgressBarVisible(false);
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        ProgressBarVisible(true);

        _image.fillAmount = e.Progress;
        if (_image.fillAmount == 1f) {
            ProgressBarVisible(false);
        }
    }



    private void ProgressBarVisible(bool visible) {
        foreach (var item in visualCanvas) {
            item.SetActive(visible);
        }
    }
}
