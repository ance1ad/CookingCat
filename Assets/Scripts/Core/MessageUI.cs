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


    private Coroutine _currentRoutine;

    public static MessageUI Instance;
    private float timeToShow = 3f;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is no more 2 MessageUI!");
        }
        Instance = this;
        Show(false);
    }

    public void Show(bool state) {
        _canvas.gameObject.SetActive(state);
    }

    public void SetText(string text, Emotions emotion) {
        Show(true);
        _text.text = text;
        SetEmotion(emotion);
        if (_currentRoutine != null) {
            StopCoroutine(_currentRoutine);
        }

        _currentRoutine = StartCoroutine(TextShowRoutine());
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
        eated
    }

    [Serializable]
    public struct EmotionsAccordance {
        public Image face;
        public Emotions emotion;
    }

    public void SetEmotion(Emotions emotion) {
        foreach (var item in _emotions) {
            if (item.emotion == emotion) {
                item.face.gameObject.SetActive(true);
            }
            else {
                item.face.gameObject.SetActive(false);
            }
        }
    }


}
