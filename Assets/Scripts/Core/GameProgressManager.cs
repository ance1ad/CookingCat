using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using YG;
using YG.Utils.LB;

public class GameProgressManager : MonoBehaviour {
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _statisticTitle;
    [Header("Player Stat")]
    [SerializeField] private TMP_Text _ordersCount;
    [SerializeField] private TMP_Text _burgersCount;
    [SerializeField] private TMP_Text _pizzasCount;
    [SerializeField] private TMP_Text _drinksCount;
    
    [Header("Player Stat UI Text")]
    [SerializeField] private TMP_Text _ordersCountText;
    [SerializeField] private TMP_Text _burgersCountText;
    [SerializeField] private TMP_Text _pizzasCountText;
    [SerializeField] private TMP_Text _drinksCountText;
    
    
    [Header("Player Info")]
    [SerializeField] private Image _avatarImg;
    [SerializeField] private Sprite _defaultAvatar;
    // XP ETC
    [SerializeField] private TMP_Text _levelVisual;
    [SerializeField] private TMP_Text _xpVisual;
    [SerializeField] private Image _xpVisualFill;
    [SerializeField] private TMP_Text _status;
    [SerializeField] private TMP_Text _worldRating;
    [SerializeField] private TMP_Text _nextStatusText;

    [SerializeField] private CongratulationManager _congratManager;
    
    
    private int _level;
    private int _rating;
    private int _xp;



    public int CountOrders;
    public int CountBurgers;
    public int CountPizzas;
    public int CountDrinks;



    public static GameProgressManager Instantiate {get; private set; }

    private void Awake() {
       if (Instantiate != null) {
           // Debug.LogWarning("GameProgressManagerGameProgressManager is already exist, error");
           return;
       }
       Instantiate = this;
       YG2.onGetSDKData += OnGetSDKData;
       YG2.onGetLeaderboard += OnGetLeaderboard;
       CloseWindow();
    }


    private void Start() {
        GetTextLocalization();
        SettingsManager.Instance.OnSwipeLanguage += GetTextLocalization;
        YG2.GetLeaderboard("MeowLeaderboard");
    }

    private void GetTextLocalization() {
        _title.text = LocalizationManager.Get("ProgressTitle");
        _statisticTitle.text = LocalizationManager.Get("StatisticTitle");
       
        _ordersCountText.text =  LocalizationManager.Get("AllOrdersCount");
        _burgersCountText.text =  LocalizationManager.Get("AllBurgersCount");
        _pizzasCountText.text =  LocalizationManager.Get("AllPizzasCount");
        _drinksCountText.text =  LocalizationManager.Get("AllDrinksCount");
        _levelVisual.text = LocalizationManager.Get("Level", _level);
        
        GetStatusByLevel();
        _levelVisual.text = LocalizationManager.Get("Level", _level);
        _worldRating.text = LocalizationManager.Get("WorldRating", _rating);

    }
    
    
    private void OnGetSDKData() { 
        _level = YG2.saves.level;
        if (_level == 0) {
            UpdateLevel();
        }
        _levelVisual.text = LocalizationManager.Get("Level", _level);
        
        GetStatusByLevel();
        GetXPOnSDK();

        CountOrders = YG2.saves.CountCompleteOrders;
        CountBurgers = YG2.saves.countBurgers;
        CountPizzas =  YG2.saves.countPizza;
        CountDrinks =  YG2.saves.countDrinks;
        
        
        _ordersCount.text = CountOrders.ToString();
        _burgersCount.text = CountBurgers.ToString();
        _pizzasCount.text = CountPizzas.ToString();
        _drinksCount.text = CountDrinks.ToString();
    }

    private void UpdateLevel() {
        _level++;
        _levelVisual.text = LocalizationManager.Get("Level", _level);
        YG2.saves.level = _level;
    }
    
    

    private void OnGetLeaderboard(LBData obj) {
       SetAvatar();
       SetRating(obj);
    }

    private void SetRating(LBData obj) {
        if (obj.currentPlayer.rank >= 0) {
            SetRatingVisual(obj.currentPlayer.rank);
            _rating = obj.currentPlayer.rank;
        }
    }

    public void SetRatingVisual(int rank) {
        _worldRating.text = LocalizationManager.Get("WorldRating", rank);
    }

    private void SetAvatar() {
       if (YG2.player != null && !string.IsNullOrEmpty(YG2.player.photo)) { 
           StartCoroutine(LoadAvatarByUrl(YG2.player.photo));
       }
       else {
           SetDefaultImg();           
       }
    }

    private IEnumerator LoadAvatarByUrl(string url) {
        // Честно сказать код спиздил, потом разберусь
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url)) {
           yield return uwr.SendWebRequest();

           if (uwr.result != UnityWebRequest.Result.Success) {
               // // Debug.LogError("Avatar load error: " + uwr.error);
               SetDefaultImg();
           }
           else {
               Texture2D tex = DownloadHandlerTexture.GetContent(uwr);
               Sprite sprite = Sprite.Create(
                   tex,
                   new Rect(0, 0, tex.width, tex.height),
                   new Vector2(0.5f, 0.5f)
               );

               _avatarImg.sprite = sprite;
           }
        }
    }



    private void SetDefaultImg() {
       _avatarImg.sprite = _defaultAvatar;
    }


    private void GetXPOnSDK() {
        _xp = YG2.saves.xp;
        SetXPVisual();
    }



    private void SetXPVisual() {
        int needXP = GetXPByNextLevel(_level);
        if (_xp >= needXP) {
            // // Debug.LogWarning("Проблема с установкой уровня");
            // Вообще такое не должно произойти т.к загружается сначала левел потом ХР но вдруг
            UpdateXP(_xp-needXP);
            return;
        }
        _xpVisual.text = $"XP: {_xp}/{needXP}";
        _xpVisualFill.fillAmount = (float)_xp / needXP;
    }
    
    
    private void GetStatusByLevel() {
        if (_level < 5) {
            _status.text = LocalizationManager.Get("rank1");
            _nextStatusText.text = LocalizationManager.Get("NextNewStatusNum", 5, LocalizationManager.Get("rank2"));
        }
        else if (_level < 10) {
            _status.text = LocalizationManager.Get("rank2");
            _nextStatusText.text = LocalizationManager.Get("NextNewStatusNum", 10, LocalizationManager.Get("rank3"));
            
        }
        else if (_level < 15) {
            _status.text = LocalizationManager.Get("rank3");
            _nextStatusText.text = LocalizationManager.Get("NextNewStatusNum", 15, LocalizationManager.Get("rank4"));
        }
        else if (_level < 20) {
            _status.text = LocalizationManager.Get("rank4");
            _nextStatusText.text = LocalizationManager.Get("NextNewStatusNum", 20, LocalizationManager.Get("rank5"));
        }
        else{
            _status.text = LocalizationManager.Get("rank5");
            _nextStatusText.text = LocalizationManager.Get("YouAreLegend");
        }
    }
    

    public void CloseWindow() {
       _canvas.SetActive(false);
    }

    public void OpenWindow() {
       _canvas.SetActive(true);
    }

    
    private int GetXPByNextLevel(int level) {
        switch(level) {
            case 1:
                return 350;
            case 2:
                return 500;
            case 3:
                return 650;
            case 4:
                return 900;
            case 5:
                return 1200;
            case 6:
                return 1500;
            case 7:
                return 1900;
            case 8:
                return 2300;
            case 9:
                return 2800;
            case 10:
                return 3400;
            case 11:
                return 4100;
            case 12:
                return 4900;
            case 13:
                return 5800;
            case 14:
                return 6800;
            case 15:
                return 80200;
            case 16:
                return 9400;
            case 17:
                return 11000;
            case 18:
                return 12800;
            case 19:
                return 15000;
            default:
                return 15000 + 1500 * (level-19);
        }
    }

    private void UpdateXP(int newXp) {
        if (newXp <= 0) {
            return;
        }
        int nextLevelXP = GetXPByNextLevel(_level);
        if (_xp + newXp >= nextLevelXP) {
            // Обновляемся
            _xp += newXp - nextLevelXP;
            UpdateLevel();
            GetStatusByLevel();
            if (_level % 5 == 0) {
                _congratManager.CongratWithNewStatus(_level, _status.text);
            }
            else {
                _congratManager.CongratWithNewLevel(_level);
            }
        }
        else {
            _xp += newXp;
        }
        SetXPVisual();
        YG2.saves.xp = _xp;
        YG2.SaveProgress();
    }

    public int CalculateXpByAccuracy(float accuracy, int count) {
        if (accuracy <= 0) {
            return 0;
        }

        accuracy /= 100;
        float xp = count * 100 * accuracy;
        
        UpdateXP((int)xp);
        return (int)xp;
    }
    
    
    
    // TEST
    private void UpdateXPByButton() {
        // // Debug.Log("Обновление xp");
        UpdateXP(500);
    }
    
    

    public void CalculateNewDishesCount(Plate plate) {
        if (plate.burgerIngredientsAdded.Count > 0) {
            // Debug.Log("Есть бургэр");
            CountBurgers++;
            YG2.saves.countBurgers = CountBurgers;
            _burgersCount.text = CountBurgers.ToString();
        }
        if (plate.pizzaIngredientsAdded.Count > 0) {
            // Debug.Log("Есть пицца");
            CountPizzas++;
            YG2.saves.countPizza = CountPizzas;
            _pizzasCount.text = CountPizzas.ToString();
        }
        if (plate.drinkIngredientsAdded.Count > 0) {
            // Debug.Log("Есть напиток");
            CountDrinks++;
            YG2.saves.countDrinks = CountDrinks;
            _drinksCount.text = CountDrinks.ToString();
        }
        
        // Он там сам сохранил в OrderManager
        _ordersCount.text = OrderManager.Instance.CountCompleteOrders.ToString();
        YG2.SaveProgress();
    }
    
}
