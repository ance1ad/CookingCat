using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private List<EmotionsAccordance> _emotions;
    [SerializeField] private PlayerVisual _playerVisual;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _focus;
    
    

    public event Action OnButtonClick;
    
    
    private Coroutine _currentRoutine;

    public static MessageUI Instance;

    private Coroutine _nextButtonPulsing;
    public void HideNextButton() {
        _nextButton.gameObject.SetActive(false);
        StopPulseCoroutine();
    }

    private void StopPulseCoroutine() {
        if (_nextButtonPulsing != null) {
            StopCoroutine(_nextButtonPulsing);
            _nextButtonPulsing = null;
        }
    }
    
    public void ShowNextButton() {
        _nextButton.gameObject.SetActive(true);
        if (_nextButtonPulsing == null) {
            _nextButtonPulsing = StartCoroutine(NextButtonPulsingRoutine());
        }
    }

    private IEnumerator NextButtonPulsingRoutine() {
        RectTransform rect = _nextButton.GetComponent<RectTransform>();
        float pulseSpeed = 7f;      // скорость пульса
        float pulseAmount = 0.1f;   // ±10% масштаб

        
        Vector3 baseScale = rect.localScale;

        while (true) {
            float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            rect.localScale = baseScale + Vector3.one * scaleOffset;
            yield return null;
        }
    }



    private IEnumerator ShakeArrow2(GameObject arrow, bool arrow2d) {
        arrow.GetComponent<ArrowData>().ResetPosition();
        RectTransform rect = arrow.GetComponent<RectTransform>();


        float up = 1f;
        float speed = 1.4f;
        if (arrow2d) {
            up *= 100f;
            speed *= 100f;
        }

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPose = startPos + up * Vector2.up;

        for (int i = 0; i < 4; i++) {
            float t = 0f;
            while (t < 1) {
                t += Time.deltaTime * speed / up;
                rect.anchoredPosition = Vector2.Lerp(startPos, endPose, t);
                yield return null;
            }

            t = 0f;
            while (t < 1) {
                t += Time.deltaTime * speed / up;
                rect.anchoredPosition = Vector2.Lerp(endPose, startPos, t);
                yield return null;
            }
        }
    }


    public bool PopupIsActive() => _canvas.gameObject.activeSelf;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is no more 2 MessageUI!");
        }
        Instance = this;
        Show(false);
        HideArrows();
    }


    private void Start() {
        ShowNextButton();
        _nextButton.onClick.AddListener(ButtonClick);
    }

    private void ButtonClick() {
        OnButtonClick?.Invoke();
        if (TutorialManager.Instance.TutorialStarted) {
            return;
        }
        Show(false);
    }

    public void Show(bool state) {
        if (_canvas.gameObject.activeSelf != state) {
            _canvas.gameObject.SetActive(state);
        }

        if (!state) {
            StopPulseCoroutine();
        }
        else if(_nextButton.gameObject.activeSelf) {
            ShowNextButton();
        }
    }
    
    
    public void SetTextInfinity(string text, Emotions emotion) {
        if (string.IsNullOrEmpty(text)) {
            return;
        }
        Show(true);
        _text.text = text;
        SetEmotion(emotion, "NextTutorStep");
        // + показать кнопку
    }
    
    
    public void SetTextTemporary(string text, Emotions emotion, float time) {
        
        if (string.IsNullOrEmpty(text)) {
            return;
        }
        if (_currentRoutine != null) {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }

        _currentRoutine = StartCoroutine(TextShowRoutine(time));
        Show(true);
        _text.text = text;
        SetEmotion(emotion, "NextTutorStep");
        // + показать кнопку
    }

    public void HideInfinityText() {
        if (_currentRoutine == null) {
            Show(false);
        }
    }


    private IEnumerator TextShowRoutine(float timeToShow) {
        yield return new WaitForSeconds(timeToShow);
        Show(false);
        _currentRoutine = null;
    }

    public enum Emotions {
        defaultFace,
        sad,
        angry,
        happy,
        shocked,
        eated,
        bonk
    }

    [Serializable]
    public struct EmotionsAccordance {
        public Image face;
        public Emotions emotion;
    }

    public void SetEmotion(Emotions emotion, string saySound) {
        foreach (var item in _emotions) {
            if (item.emotion == emotion) {
                item.face.gameObject.SetActive(true);
                if (emotion == Emotions.angry) {
                    SoundManager.Instance.PlaySFX("Angry");
                }
                else {
                    SoundManager.Instance.PlaySFX(saySound);
                }
            }
            else {
                item.face.gameObject.SetActive(false);
            }
        }
    }
    

    public void ShowPlayerPopup(string text) {
        _playerVisual.ShowPopupText(text);
        SoundManager.Instance.PlaySFX("Warning");
    } 

    
    public IEnumerator HideFocusRoutine() {
        if (!_focus.gameObject.activeSelf) {
            _focus.gameObject.SetActive(true);
        }
        _focus.fillAmount = 1;
        float time = 0;
        float timeToClose = 2f;
        while (time <= timeToClose) {
            time+=Time.deltaTime;
            _focus.fillAmount = 1 - time/timeToClose;
            yield return null;
        }
        _focus.gameObject.SetActive(false);
        HideInfinityText();
        
    }
    
    public void HideArrows() {
        if (_platesArrow.activeSelf) {
            _platesArrow.SetActive(false);
        }
        if (_stoveArrow.activeSelf) {
            _stoveArrow.SetActive(false);
        }
        if (_ovenArrow.activeSelf) {
            _ovenArrow.SetActive(false);
        }
        if (_juicerArrow.activeSelf) {
            _juicerArrow.SetActive(false);
        }
        if (_bunArrow.activeSelf) {
            _bunArrow.SetActive(false);
        }
        if (_tomatoArrow.activeSelf) {
            _tomatoArrow.SetActive(false);
        }
        if (_testoArrow.activeSelf) {
            _testoArrow.SetActive(false);
        }
        if (_cheeseArrow.activeSelf) {
            _cheeseArrow.SetActive(false);
        }
        if (_orangeArrow.activeSelf) {
            _orangeArrow.SetActive(false);
        }
        if (_appleArrow.activeSelf) {
            _appleArrow.SetActive(false);
        }
        if (_meatArrow.activeSelf) {
            _meatArrow.SetActive(false);
        }
        if (_cuttingArrow.activeSelf) {
            _cuttingArrow.SetActive(false);
        }
        if (_getOrderArrow.activeSelf) {
            _getOrderArrow.SetActive(false);
        }
        if (_upClearCounterArrow.activeSelf) {
            _upClearCounterArrow.SetActive(false);
        }
        if (_rightClearCounterArrow.activeSelf) {
            _rightClearCounterArrow.SetActive(false);
        }
        if (_trashArrow.activeSelf) {
            _trashArrow.SetActive(false);
        }
        
    }
    
        
    [SerializeField] private GameObject _platesArrow;
    [SerializeField] private GameObject _ovenArrow;
    [SerializeField] private GameObject _stoveArrow;
    [SerializeField] private GameObject _juicerArrow;
    [SerializeField] private GameObject _bunArrow;
    [SerializeField] private GameObject _tomatoArrow;
    [SerializeField] private GameObject _meatArrow;
    [SerializeField] private GameObject _testoArrow;
    [SerializeField] private GameObject _cheeseArrow;
    [SerializeField] private GameObject _orangeArrow;
    [SerializeField] private GameObject _appleArrow;
    [SerializeField] private GameObject _cuttingArrow;
    [SerializeField] private GameObject _getOrderArrow;
    [SerializeField] private GameObject _upClearCounterArrow;
    [SerializeField] private GameObject _rightClearCounterArrow;
    [SerializeField] private GameObject _trashArrow;
    [SerializeField] private BaseCounter _upClearCounter;
    [SerializeField] private BaseCounter _rightClearCounter;
    
    

    public void ShowPlatesArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _platesArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_platesArrow, false));
    }

    public void ShowOvenArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _ovenArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_ovenArrow, false));
    }
    
    public void ShowStoveArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _stoveArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_stoveArrow, false));
    }
    
    
    public void ShowJuicerArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _juicerArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_juicerArrow, false));
    }
    
    
    public void ShowAppleArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _appleArrow.SetActive(true);
    }
    
    public void ShowOrangeArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _orangeArrow.SetActive(true);
    }
    
    public void ShowBunArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _bunArrow.SetActive(true);
        // StartCoroutine(ShakeArrow(_bunArrow));
    }
    
    public void ShowTomatoArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _tomatoArrow.SetActive(true);
        // StartCoroutine(ShakeArrow(_tomatoArrow));
    }
    
    
    public void ShowMeatArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _meatArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_meatArrow,false));
    }
    
    
    public void ShowCuttingArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _cuttingArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_cuttingArrow, false));
    }
    
    public void ShowGetOrderArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _getOrderArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_getOrderArrow, false));
    } 
    
    
    
    public void ShowClearCountersArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _upClearCounterArrow.SetActive(true);
        _rightClearCounterArrow.SetActive(true);
    }
    
    public void ShowTrashArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _trashArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_trashArrow, false));
    }
  
    
    // 2d arrows
    public void ShakeTargetArrow(GameObject arrow) {
            StartCoroutine(ShakeArrow(arrow, true));
    } 
    
    public void ShowActiveClearCounterArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        // Или или
        if (_upClearCounter.GetKitchenObject() is Plate) {
            _upClearCounterArrow.SetActive(true);
        }
        else if(_rightClearCounter.GetKitchenObject() is Plate) {
            _rightClearCounterArrow.SetActive(true);
        }
    }
    
    public void ShowTestoArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _testoArrow.SetActive(true);
        // StartCoroutine(ShakeArrow(_testoArrow));
    } 
    
    public void ShowCheeseArrow(bool hidePrevious) {
        if (hidePrevious) {
            HideArrows();
        }
        _cheeseArrow.SetActive(true);
        // StartCoroutine(ShakeArrow(_cheeseArrow));
    } 
    
    

    private IEnumerator ShakeArrow(GameObject arrow, bool arrow2d) {
        arrow.GetComponent<ArrowData>().ResetPosition();
        RectTransform rect = arrow.GetComponent<RectTransform>();
        
        
        float up = 1f;
        float speed = 1.4f;
        if (arrow2d) {
            up *= 100f;
            speed *= 100f;
        }
   
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPose = startPos + up * Vector2.up;
        
        for (int i = 0; i < 4; i++) {
            float t = 0f;
            while (t < 1) {
                t += Time.deltaTime * speed / up;
                rect.anchoredPosition = Vector2.Lerp(startPos, endPose, t);
                yield return null;
            }
            t = 0f;
            while (t < 1) {
                t += Time.deltaTime * speed / up;
                rect.anchoredPosition = Vector2.Lerp(endPose, startPos, t);
                yield return null;
            }
        }
    }
    
    

}
