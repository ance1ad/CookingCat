using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class ProductSaveManager : MonoBehaviour {
    [Header("Первородные продукты")] 
    [SerializeField] private KitchenObjectSO _bun;
    [SerializeField] private KitchenObjectSO _tomato;
    [SerializeField] private KitchenObjectSO _cheese;
    [SerializeField] private KitchenObjectSO _greenery;
    [SerializeField] private KitchenObjectSO _meat;
    [SerializeField] private KitchenObjectSO _onion;
    [SerializeField] private KitchenObjectSO _mushroom;
    [SerializeField] private KitchenObjectSO _testo;
    [SerializeField] private KitchenObjectSO _apple;
    [SerializeField] private KitchenObjectSO _tangerine;
    // Кол-во товаров
    private int bunCount;
    private int tomatoCount;
    private int cheeseCount;
    private int greeneryCount;
    private int meatCount;
    private int onionCount;
    private int mushroomCount;
    private int testoCount;
    private int appleCount;
    private int tangerineCount;


    public static ProductSaveManager Instance { get; private set; }
    private int _initCount = 5;


    private void Awake() {
        if (Instance!=null) {
            Debug.Log("Error, > 1 ProductSaveManager");
            return;
        }
        Instance = this;
        YG2.onGetSDKData += OnGetSDKData;
    }

    
    private void OnGetSDKData() {
        // Туториал пройден продукты инициализированы
        if (YG2.saves.TutorialPassed) {
            LoadProductsCount();
        }
        else {
            SetFirstTimeValues();
        }
    }
    
    private void LoadProductsCount() {
        bunCount = YG2.saves.bunCount;
        tomatoCount=  YG2.saves.tomatoCount;
        cheeseCount = YG2.saves.cheeseCount;
        greeneryCount = YG2.saves.greeneryCount;
        meatCount= YG2.saves.meatCount;
        onionCount = YG2.saves.onionCount;
        mushroomCount = YG2.saves.mushroomCount;
        testoCount = YG2.saves.testoCount;
        appleCount = YG2.saves.appleCount;
        tangerineCount = YG2.saves.tangerineCount;
    }



    private void SetFirstTimeValues() {
        YG2.saves.bunCount = _initCount;
        YG2.saves.tomatoCount = _initCount;
        YG2.saves.cheeseCount = _initCount;
        YG2.saves.greeneryCount = _initCount;
        YG2.saves.meatCount = _initCount;
        YG2.saves.onionCount = _initCount;
        YG2.saves.mushroomCount = _initCount;
        YG2.saves.testoCount = _initCount;
        YG2.saves.appleCount = _initCount;
        YG2.saves.tangerineCount = _initCount;
        YG2.SaveProgress();
        LoadProductsCount();
    }


    
    
    public int GetProductCount(KitchenObjectSO product) {
        
        if (product == _bun) {
            return bunCount;
        }
        if (product == _tomato) {
            return tomatoCount;
        }
        if (product == _cheese) {
            return cheeseCount;
        }
        if (product == _greenery) {
            return greeneryCount;
        }
        if (product == _meat) {
            return meatCount;
        }
        if (product == _onion) {
            return onionCount;
        }
        if (product == _mushroom) {
            return mushroomCount;
        }
        if (product == _testo) {
            return testoCount;
        }
        if (product == _apple) {
            return appleCount;
        }
        if (product == _tangerine) {
            return tangerineCount;
        }
        return 0;
    }
    
    public bool UpdateProductCount(KitchenObjectSO product, int count) {
        if (product == _bun && (bunCount + count >= 0)) {
            bunCount += count;
            return true;
        }
        if (product == _tomato && (tomatoCount + count >= 0)) {
            tomatoCount  += count;
            return true;
        }
        if (product == _cheese && (cheeseCount + count >= 0)) {
            cheeseCount += count;
            return true;
        }
        if (product == _greenery && (greeneryCount + count >= 0)) {
            greeneryCount += count;
            return true;
        }
        if (product == _meat && (meatCount + count >= 0)) {
            meatCount += count;
            return true;
        }
        if (product == _onion && (onionCount + count >= 0)) {
            onionCount += count;
            return true;
        }
        if (product == _mushroom && (mushroomCount + count >= 0)) {
            mushroomCount += count;
            return true;
        }
        if (product == _testo && (testoCount + count >= 0)) {
            testoCount += count;
            return true;
        }
        if (product == _apple && (appleCount + count >= 0)) {
            appleCount += count;
            return true;
        }
        if (product == _tangerine && (tangerineCount + count >= 0)) {
            tangerineCount += count;
            return true;
        }
        return false;
    }
    
    public void SaveProductsCount() {
        YG2.saves.bunCount = bunCount;
        YG2.saves.tomatoCount = tomatoCount;
        YG2.saves.cheeseCount = cheeseCount;
        YG2.saves.greeneryCount = greeneryCount;
        YG2.saves.meatCount = meatCount;
        YG2.saves.onionCount = onionCount;
        YG2.saves.mushroomCount = mushroomCount;
        YG2.saves.testoCount = testoCount;
        YG2.saves.appleCount = appleCount;
        YG2.saves.tangerineCount = tangerineCount;
        YG2.SaveProgress();
    }

}
