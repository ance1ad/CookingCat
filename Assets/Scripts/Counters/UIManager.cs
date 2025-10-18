using PlasticGui.WorkspaceWindow.Locks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    
    // Сами кнопки
    [SerializeField] private Button _eButton;
    [SerializeField] private Button _fButton;
    
    // Уровень
    [SerializeField] private Image _levelEButton;
    [SerializeField] private Image _levelFButton;


    // Серый
    [SerializeField] private Image _eGray;
    [SerializeField] private Image _fGray;


    // E
    [SerializeField] private Image _takeImg;
    [SerializeField] private Image _putImg;
    [SerializeField] private Image _fightImg;
    //[SerializeField] private GameObject _takeOrderImg;
    //[SerializeField] private GameObject _putOrderImg;
    // мб еще с бургером но хз



    // F
    [SerializeField] private Image _slicedObjectImage;
    [SerializeField] private Image _slicedObjectShadowImage;
    [SerializeField] private Image _setOvenOnImage;



    //....



    public static UIManager Instance { get; private set; }


    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is no more 2 UIManager!");
        }
        Instance = this;
        _takeImg.enabled = true; // пусть 1 горит
        _putImg.enabled = false;
        _fightImg.enabled = false;
        _setOvenOnImage.enabled = false;
        ShowSlicingSprite(false);
    }



    private void Start() {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        Player.Instance.OnThiefInteract += PLayer_OnThiefInteract;
    }

    private void PLayer_OnThiefInteract(object sender, Player.OnThiefInteractEventArgs e) {
        if (e.thief != null) {
            SetGrayEButton(false);
            SetEButton(UIButtonState.Fight);
            return;
        }
        SetGrayEButton(true);
        if (Player.Instance.HasKitchenObject()) {
            SetEButton(UIButtonState.Put);
        }
        else {
            SetEButton(UIButtonState.Take);
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
        if (e.selectedCounter != null) {
            SetGrayEButton(false);
            // Порезать
            if (e.selectedCounter is CuttingCounter && e.selectedCounter.HasKitchenObject()) {
                CuttingCounter tempCC = e.selectedCounter as CuttingCounter;
                if (!tempCC.ObjectIsSliced()) {
                    SetFButton(UIButtonState.Slice, e.selectedCounter.GetKitchenObject().GetKitchenObjectSO().sprite);
                    SetGrayFButton(false);
                }
            }
            if (e.selectedCounter is OvenCounter) {
                SetFButton(UIButtonState.On, null);
                SetGrayFButton(false);
            }
            // Взять можно
            if (e.selectedCounter.HasKitchenObject()) {
                SetEButton(UIButtonState.Take);
            }
            // Можно положить
            if (Player.Instance.HasKitchenObject()) {
                SetEButton(UIButtonState.Put);
            }
        }
        else {
            SetGrayEButton(true);
            SetGrayFButton(true);
            ShowSlicingSprite(false);
        }
    }



    public enum UIButtonState {
        None,
        Take,
        Put,
        Fight,
        TakeOrder,
        PutOrder,

        Slice,
        On
    }

    private UIButtonState _eCurrentState = UIButtonState.None;
    private UIButtonState _fCurrentState = UIButtonState.None;

    public void SetEButton(UIButtonState state) {
        if (state == _eCurrentState) return; 
        Debug.Log("Установка " + state);

        _takeImg.enabled = false;
        _putImg.enabled = false;
        _fightImg.enabled = false;

        switch (state) {
            case UIButtonState.Take:
                _takeImg.enabled = true;
                break;
            case UIButtonState.Put:
                _putImg.enabled = true;
                break;
            case UIButtonState.Fight:
                _fightImg.enabled = true;
                break;
        }
        _eCurrentState = state;
    }


    public void SetFButton(UIButtonState state, Sprite sprite) {
        if (state == _fCurrentState) return; 
        Debug.Log("Установка " + state);
        ShowSlicingSprite(true);
        _setOvenOnImage.enabled = false;

        switch (state) {
            case UIButtonState.Slice:
                _slicedObjectImage.enabled = true;
                _slicedObjectShadowImage.enabled = true;
                _slicedObjectImage.sprite = sprite;
                _slicedObjectShadowImage.sprite = sprite;
                Debug.Log(sprite);
                break;
            case UIButtonState.On:
                _setOvenOnImage.enabled = true;
                break;
        }
        _eCurrentState = state;
    }


    public void ShowSlicingSprite(bool state) {
        _slicedObjectImage.enabled = state;
        _slicedObjectShadowImage.enabled = state;
    }



    public void SetGrayEButton(bool state) {
        _eGray.enabled = state;
    }

    public void SetGrayFButton(bool state) {
        _fGray.enabled = state;
    }



    // ШОБ МЕНЯТЬ ЛЕВЕЛ ----------------------------
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
}
