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
    [SerializeField] private GameObject _nextButton;
    [SerializeField] private Image _focus;



    
    
    private Coroutine _currentRoutine;

    public static MessageUI Instance;
    private float timeToShow = 5f;

    public bool PopupIsActive() => _canvas.gameObject.activeSelf;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is no more 2 MessageUI!");
        }
        Instance = this;
        Show(false);
    }

    public void Show(bool state) {
        if (_canvas.gameObject.activeSelf != state) {
            _canvas.gameObject.SetActive(state);
        }
    }

    public void SetText(string text, Emotions emotion) {
        Show(true);
        _text.text = text;
        SetEmotion(emotion, "CatSay");
        if (_currentRoutine != null) {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }

        _currentRoutine = StartCoroutine(TextShowRoutine());
    }
    
    public void SetTextInfinity(string text, Emotions emotion) {
        // В этом случае будет меняться только текст
        if (_currentRoutine != null) {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }
        Show(true);
        _text.text = text;
        SetEmotion(emotion, "NextTutorStep");
        // + показать кнопку
    }

    public void HideInfinityText() {
        if (_currentRoutine == null) {
            Show(false);
            _nextButton.gameObject.SetActive(false);
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
    
    
    

}
