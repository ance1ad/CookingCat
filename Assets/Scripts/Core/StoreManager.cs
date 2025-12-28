using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class StoreManager : MonoBehaviour {
    [SerializeField] private GameObject _storeCanvas;
    [SerializeField] private List<SkinItem> _skins;
    [SerializeField] private List<UpgradeItem> _upgrades;

    [SerializeField] private Button _sortHats;
    [SerializeField] private Button _sortGlasses;
    [SerializeField] private Button _sortMasks;
    [SerializeField] private Button _sortColors;
    [SerializeField] private Button _sortUpgrades;
    
    [SerializeField] private TMP_Text _sortHatsText;
    [SerializeField] private TMP_Text _sortGlassesText;
    [SerializeField] private TMP_Text _sortMasksText;
    [SerializeField] private TMP_Text _sortColorsText;
    [SerializeField] private TMP_Text _upgradesText;
    
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private PlayerData _data;
    
    // Храним во вкладке, что конкретно экипировано
    private SkinItem hatSkin;
    private SkinItem glassesSkin;
    private SkinItem maskSkin;
    private SkinItem colorSkin;

    private PurchaseType _windowCategoryLastOpened = PurchaseType.Hat;
    
    
    // Обработка нажатий + UI
    private void Awake() {
        YG2.onGetSDKData += DownloadData;
    }

    private void Start() {
        _storeCanvas.SetActive(false);
        foreach (var skin in _skins) {
            skin.OnSkinBought += OnBought;
            skin.OnSkinEquipped += OnSkinEquipped;
            skin.OnSkinDequipped += OnSkinDequipped;
        }

        foreach (var upgrade in _upgrades) {
            upgrade.OnUpgradeBought += OnBought;
        }
        SetSortButtons();
        LanguageLocalization();
        ChangeItemsLanguage();
        SettingsManager.Instance.OnSwipeLanguage += OnSwipeLanguage;
        KitchenEvents.OnSettingsCanvasOpen += HideCanvas;
    }

    private void HideCanvas() {
        if (_storeCanvas != null && _storeCanvas.activeSelf) {
            _storeCanvas.SetActive(false);
        }
    }

    private void OnSwipeLanguage() {
        languageIsChanged = true;
    }

    private void DownloadData() {
        ShowHideStoreWindow();
        // Загрузить из данных значения и отобразить у игрока сё это
        SetSkinsBought();
        EquipDownloadSkins();
        // Загрузка обновочек
        UpdateUpgrades();
        ShowHideStoreWindow();
    }

    private void SetSkinsBought() {
        List<string> OwnedPurchase = YG2.saves.OwnedPurchase;
        foreach (var skin in _skins) {
            if (OwnedPurchase.Contains(skin.ObjectSO.Id)) {
                skin.SetBought();
            }
            else {
                skin.SetUnbought();
            }
        }
    }

    private void EquipDownloadSkins() {
        var AppliedSkins = YG2.saves.AppliedSkins;
        foreach (var skin in _skins) {
            if (AppliedSkins.FindIndex(s => s.id == skin.ObjectSO.Id) >= 0) {
                skin.SetSkinEquipped();
                RememberSkin(skin);
                skin.ObjectSO.Apply(_data);
                skin.PurchaseSO.Buy(_data);
            }
        }
    }


    private void UpdateUpgrades() {
        _data.DownloadUpgrades(); 
        foreach (var upgrade in _upgrades) {
            int index = YG2.saves.Upgrades.FindIndex(s => s.id == upgrade.ObjectSO.Id);
            if (index >= 0) {
                for (int i = 0; i < YG2.saves.Upgrades[index].count; i++) {
                    upgrade.SetBought();
                }
            }
        }
    }


    private void SetSortButtons() {
        _sortHats.onClick.AddListener(() => SortItems(PurchaseType.Hat));
        _sortGlasses.onClick.AddListener(() => SortItems(PurchaseType.Glasses));
        _sortMasks.onClick.AddListener(() => SortItems(PurchaseType.Mask));
        _sortColors.onClick.AddListener(() => SortItems(PurchaseType.Color));
        _sortUpgrades.onClick.AddListener(() => SortItems(PurchaseType.Upgrade));
    }

    private void SortItems(PurchaseType _purchaseType) {
        HideAll();
        _windowCategoryLastOpened = _purchaseType;
        SortList(_skins, _purchaseType);
        SortList(_upgrades, _purchaseType);
    }

    private void SortList<T>(List<T> items, PurchaseType _purchaseType) where T : PurchaseObject {
        foreach (var obj in items) {
            if (obj.PurchaseType == _purchaseType) {
                if (!obj.gameObject.activeSelf) {
                    obj.gameObject.SetActive(true);
                    obj.ResetWarningText(); // сбивается корутина или хз че когда обьект неактивен
                }
            }
            else {
                obj.gameObject.SetActive(false);
            }
        }
    }

    private void HideAll() {
        foreach (var upgrade in _upgrades) {
            upgrade.gameObject.SetActive(false);
        }
    }
    
    
    private void OnBought(PurchaseObject item) {
        // Debug.Log(item.PurchaseSO);
        ValuteType valuteType = item.ValuteType;
        if (valuteType == ValuteType.Coins) {
            if (CurrencyManager.Instance.Coins >= item.CurrentPrice) {
                CurrencyManager.Instance.UpdateCash(-1* item.CurrentPrice, 0);
            }
            else{
                item.SetMessageResultText(LocalizationManager.Get("NotEnoughCoins"),false);
                SoundManager.Instance.PlaySFX("Warning");
                return;
            }
        }
        else if (valuteType == ValuteType.Gems) {
            if (CurrencyManager.Instance.Gems >= item.CurrentPrice) {
                CurrencyManager.Instance.UpdateCash(0, (int)-item.CurrentPrice);
            }
            else{
                item.SetMessageResultText(LocalizationManager.Get("NotEnoughGems"),false);
                SoundManager.Instance.PlaySFX("Warning");
                return;
            }
        }
        else if (valuteType == ValuteType.Adv) {
            YGManager.Instance.ShowRewardAdv("BuyItem",
                () => {
                    item.SetMessageResultText(LocalizationManager.Get("PurchaseSuccesfull"),true);
                    item.PurchaseSO.Buy(_data);
                    item.SetBought();
                });
            return;
        }
        item.SetMessageResultText(LocalizationManager.Get("PurchaseSuccesfull"),true);
        item.PurchaseSO.Buy(_data);
        item.SetBought();
    }
    
    private void OnSkinDequipped(SkinItem skin) {
        skin.ObjectSO.Remove(_data);
        skin.SetSkinDequipped();
    }


    private void OnSkinEquipped(SkinItem skin) {
        skin.SetSkinEquipped();
        RememberSkin(skin);
        skin.ObjectSO.Apply(_data);
    }


    private bool languageIsChanged = true;
    public void ShowHideStoreWindow() {
        _storeCanvas.SetActive(!_storeCanvas.activeSelf);
        if (!_storeCanvas.activeSelf) {
            PlayerBankVisual.Instance?.HideBank();
            KitchenEvents.ShopClose();
            return;
        }
        KitchenEvents.ShopOpen();
        PlayerBankVisual.Instance?.ShowBank();
        if (languageIsChanged) {
            ChangeItemsLanguage();
            SortItems(_windowCategoryLastOpened);
            LanguageLocalization();
            languageIsChanged = false;
        }
        foreach (var skin in _skins) {
            skin.SetEmptyWarningMessage();
        }
    }

    private void ChangeItemsLanguage()
    {
        foreach (var skin in _skins) {
            skin.SetTextLocalization();
        }

        foreach (var upgrade in _upgrades) {
            upgrade.SetTextLocalization();
        }
    }


    private void LanguageLocalization()
    {
        _titleText.text =  LocalizationManager.Get("SkinStoreTitleText");
        _sortHatsText.text =  LocalizationManager.Get("SortHats");
        _sortGlassesText.text =  LocalizationManager.Get("SortGlasses");
        _sortMasksText.text =  LocalizationManager.Get("SortMasks");
        _sortColorsText.text =  LocalizationManager.Get("Colors");
        _upgradesText.text =  LocalizationManager.Get("Upgrades");
    }

    
    
    // Remember for skins on page
    private void RememberSkin(SkinItem skin) {
        // Запоминаем надетый скин
        if (skin.PurchaseType == PurchaseType.Hat) {
            if (hatSkin != null  &&  hatSkin != skin) {
                // Debug.Log("SetSkinDequipped");
                hatSkin.SetSkinDequipped();
                hatSkin.ObjectSO.Remove(_data);
            }
            hatSkin = skin;
        }
        else if (skin.PurchaseType == PurchaseType.Glasses) {
            if (glassesSkin != null  && glassesSkin != skin) {
                glassesSkin.SetSkinDequipped();
                glassesSkin.ObjectSO.Remove(_data);
            }
            glassesSkin = skin;
        }
        else if (skin.PurchaseType == PurchaseType.Mask) {
            if (maskSkin != null && maskSkin != skin) {
                maskSkin.SetSkinDequipped();
                maskSkin.ObjectSO.Remove(_data);
                
            }
            maskSkin = skin;
        }
        else if (skin.PurchaseType == PurchaseType.Color) {
            if (colorSkin != null && colorSkin != skin) {
                colorSkin.SetSkinDequipped();
                colorSkin.ObjectSO.Remove(_data);
            }
            colorSkin = skin;
        }
    }
}
