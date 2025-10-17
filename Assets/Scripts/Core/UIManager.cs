using PlasticGui.WorkspaceWindow.Locks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Button _eButton;
    [SerializeField] private Button _fButton;
    
    // ”Ó‚ÂÌ¸
    [SerializeField] private Image _levelEButton;
    [SerializeField] private Image _levelFButton;

    // —Â˚È
    [SerializeField] private GameObject _eGray;
    [SerializeField] private GameObject _fGray;



    [SerializeField] private Image _slicedObjectImage;
    [SerializeField] private Image _slicedObjectShadowImage;
    private Sprite _slicedObjectSprite;
    private Sprite _slicedObjectShadowSprite;

    [SerializeField] private GameObject _takeObjectImg;
    [SerializeField] private GameObject _putObjectImg;



    public static UIManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is no more 2 UIManager!");
        }
        Instance = this;
    }



    private void Start() {
        _slicedObjectSprite = _slicedObjectImage.GetComponent<Sprite>();
        _slicedObjectShadowSprite = _slicedObjectShadowImage.GetComponent<Sprite>();
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
        if (e.selectedCounter != null && e.selectedCounter.HasKitchenObject()) {
            ShowTakeButton();
        }
        else {
            ShowPutButton();
        }
    }


    //public void SetGrayEButton(bool state) {
    //}

    //public void SetGrayFButton(bool state) {
    //}



    // ÿŒ¡ Ã≈Õﬂ“‹ À≈¬≈À ----------------------------
    public void StartProgressChangeEButton(float time) {
        ProgressChangeRoutine(_levelEButton, time);
    }

    public void StartProgressChangeFButton(float time) {
        ProgressChangeRoutine(_levelFButton, time);
    }

    private IEnumerator ProgressChangeRoutine(Image img, float time) {
        float localTime = 0f;
        while (localTime < time) {
            img.fillAmount = localTime / time;
            localTime += Time.deltaTime;
            yield return null;
        }
        img.fillAmount = 0f;
    }


    public void ShowPutButton() {
        _takeObjectImg.gameObject.SetActive(false);
        _putObjectImg.gameObject.SetActive(true);
        

    }

    public void ShowTakeButton() {
        _takeObjectImg.gameObject.SetActive(true);
        _putObjectImg.gameObject.SetActive(false);
    }




    // -------------------------------------------
    public void SetSliceButton(Sprite img) {
        // Ò‰ÂÎ‡Ú¸ ÒÂ˚Ï
        _slicedObjectSprite = img;
        _slicedObjectShadowSprite = img;
    }






    public void ShowButtons(bool state) {
        _eButton.gameObject.SetActive(state);
        _fButton.gameObject.SetActive(state);
    }
}
