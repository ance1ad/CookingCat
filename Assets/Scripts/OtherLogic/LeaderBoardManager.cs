using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using YG.Utils.LB;

public class LeaderBoardManager : MonoBehaviour {
    [SerializeField] private GameObject _leaderBoardCanvas;
    [SerializeField] private TMP_Text _boardName;
    
    // LeaderBoardName: MeowLeaderboard


    private void Start() {
        SettingsManager.Instance.OnSwipeLanguage += OnSwipeLanguage; 
        _boardName.text = LocalizationManager.Get("LeaderBoardName");
        SetLeaderBoardState(false);
        
    }


    private bool needToChangeLanguage = false;
    private void OnSwipeLanguage() {
        // // Debug.Log("OnSwipeLanguage");
        needToChangeLanguage = true;
    }


    public void SetLeaderBoardState(bool state) {
        _leaderBoardCanvas.SetActive(state);
        if (state && needToChangeLanguage) {
            needToChangeLanguage = false;
            _boardName.text = LocalizationManager.Get("LeaderBoardName");
        }
    }
    

    
}
