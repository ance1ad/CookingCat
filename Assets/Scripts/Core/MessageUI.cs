using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private List<EmotionsAccordance> _emotions;
    [SerializeField] private PlayerVisual _playerVisual;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Image _focus;
    
    
    
    [SerializeField] private GameObject _platesArrow;
    [SerializeField] private GameObject _ovenArrow;
    [SerializeField] private GameObject _stoveArrow;
    [SerializeField] private GameObject _juicerArrow;


    public event Action OnButtonClick;
    
    
    private Coroutine _currentRoutine;

    public static MessageUI Instance;
    private float timeToShow = 5f;

    public void HideNextButton() {
        _nextButton.gameObject.SetActive(false);
    }
    
    public void ShowNextButton() {
        _nextButton.gameObject.SetActive(true);
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
        _nextButton.gameObject.SetActive(true);
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
    }

    public void SetText(string text, Emotions emotion) {
        if (string.IsNullOrEmpty(text)) {
            return;
        }
        Show(true);
        _text.text = text;
        SetEmotion(emotion, "CatSay");
        // Попробую со скипом чисто
        // if (_currentRoutine != null) {
        //     StopCoroutine(_currentRoutine);
        //     _currentRoutine = null;
        // }

        // _currentRoutine = StartCoroutine(TextShowRoutine());
    }
    
    public void SetTextInfinity(string text, Emotions emotion) {
        // В этом случае будет меняться только текст
        // if (_currentRoutine != null) {
        //     StopCoroutine(_currentRoutine);
        //     _currentRoutine = null;
        // }
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


    private IEnumerator TextShowRoutine() {
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
        
        
    }

    public void ShowPlatesArrow() {
        HideArrows();
        _platesArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_platesArrow));
    }

    public void ShowStoveArrow() {
        HideArrows();
        _stoveArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_stoveArrow));
    }
    
    public void ShowOvenArrow() {
        HideArrows();
        _ovenArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_ovenArrow));
    }
    
    
    public void ShowJuicerArrow() {
        HideArrows();
        _juicerArrow.SetActive(true);
        StartCoroutine(ShakeArrow(_juicerArrow));
    }
    
    
    

    private IEnumerator ShakeArrow(GameObject arrow) {
        float upScale = 1f;
        Vector3 startPos = arrow.transform.position;
        Vector3 scaleVector = new Vector3(startPos.x, startPos.y + upScale, startPos.z);
        float currentScale = 0f;
        for (int i = 0; i < 4; i++) {
            while (currentScale < upScale) {
                currentScale += Time.deltaTime * 1.4f / upScale;
                arrow.transform.position = Vector3.Lerp(startPos, scaleVector, currentScale);
                yield return null;
            }

            currentScale = 0f;
            while (currentScale < upScale) {
                currentScale += Time.deltaTime * 1.4f / upScale;
                arrow.transform.position = Vector3.Lerp(scaleVector, startPos, currentScale);
                yield return null;
            }
            currentScale = 0f;
        }
    }
    
    

}
