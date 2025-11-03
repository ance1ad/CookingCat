using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinStoreManager : MonoBehaviour {
    [SerializeField] private GameObject _storeCanvas;
    [SerializeField] private List<SkinItem> _skins;

    [SerializeField] private Button _sortHats;
    [SerializeField] private Button _sortGlasses;
    [SerializeField] private Button _sortMasks;
    
    [SerializeField] private TMP_Text _sortHatsText;
    [SerializeField] private TMP_Text _sortGlassesText;
    [SerializeField] private TMP_Text _sortMasksText;
    [SerializeField] private TMP_Text _upgradesText;
    
    [SerializeField] private TMP_Text _titleText;
    
    
    // Храним во вкладке, что конкретно экипировано
    private SkinItem hatSkin;
    private SkinItem glassesSkin;
    private SkinItem maskSkin;

    private BuyType _windowCategoryLastOpened = BuyType.Hat;
    
    
    private void Start() {
        _storeCanvas.SetActive(false);
        foreach (var skin in _skins) {
            skin.OnSkinBought += OnBought;
            skin.OnSkinEquipped += OnSkinEquipped;
            skin.OnSkinDequipped += OnSkinDequipped;
        }
        _sortHats.onClick.AddListener(() => SortSkins(BuyType.Hat));
        _sortGlasses.onClick.AddListener(() => SortSkins(BuyType.Glasses));
        _sortMasks.onClick.AddListener(() => SortSkins(BuyType.Mask));
        
    }

    private void SortSkins(BuyType _skinType) {
        _windowCategoryLastOpened = _skinType;
        foreach (var skin in _skins) {
            if (skin.SkinType == _skinType) {
                if (!skin.gameObject.activeSelf) {
                    skin.gameObject.SetActive(true);
                    skin.ResetWarningText(); // сбивается корутина или хз че когда обьект неактивен
                }
            }
            else {
                skin.gameObject.SetActive(false);
            }
        }
    }

    private void OnBought(SkinItem skin) {
        ValuteType valuteType = skin.ValuteType;
        Debug.Log(valuteType);
        if (valuteType == ValuteType.Coins) {
            if (CurrencyManager.Instance.Coins >= skin.Price) {
                CurrencyManager.Instance.UpdateCash(-1* skin.Price, 0);
                skin.SetMessageResultText("Скин успешно куплен",true);
                skin.SetSkinBought();
                PlayerSkinChanger.Instance.AddSkin(skin.SkinObject);
            }
            else{
                skin.SetMessageResultText("Не хватает коинов",false);
                SoundManager.Instance.PlaySFX("Warning");
            }
        }
        else if (valuteType == ValuteType.Gems) {
            if (CurrencyManager.Instance.Gems >= skin.Price) {
                CurrencyManager.Instance.UpdateCash(0, (int)-skin.Price);
                skin.SetMessageResultText("Скин успешно куплен",true);
                skin.SetSkinBought();
                PlayerSkinChanger.Instance.AddSkin(skin.SkinObject);
            }
            else{
                skin.SetMessageResultText("Не хватает гемов",false);
                SoundManager.Instance.PlaySFX("Warning");
            }
        }
    }

    private void OnSkinDequipped(SkinItem skin) {
        PlayerSkinChanger.Instance.DequipSkin(skin.SkinObject);
        skin.SetSkinDequipped();
    }


    private void OnSkinEquipped(SkinItem skin) {
        PlayerSkinChanger.Instance.EquipSkin(skin.SkinObject);
        skin.SetSkinEquipped();
        RememberSkin(skin);
    }



    public void ShowHideStoreWindow() {
        _storeCanvas.SetActive(!_storeCanvas.activeSelf);
        if (!_storeCanvas.activeSelf) {
            return;
        }
        foreach (var skin in _skins) {
            if (PlayerSkinChanger.Instance.HasSkin(skin.SkinObject)) {
                if (PlayerSkinChanger.Instance.SkinIsEquipped(skin.SkinObject)) {
                    skin.SetSkinEquipped();
                    RememberSkin(skin);

                }
                else {
                    skin.SetSkinDequipped();
                }
            }
            skin.SetTextLocalization();
        }
        SortSkins(_windowCategoryLastOpened);
        LanguageLocalization();
    }

    private void LanguageLocalization()
    {
        _titleText.text =  LocalizationManager.Get("SkinStoreTitleText");
        _sortHatsText.text =  LocalizationManager.Get("SortHats");
        _sortGlassesText.text =  LocalizationManager.Get("SortGlasses");
        _sortMasksText.text =  LocalizationManager.Get("SortMasks");
        _upgradesText.text =  LocalizationManager.Get("Upgrades");
    }


    private void RememberSkin(SkinItem skin) {
        // Запоминаем надетый скин
        if (skin.SkinType == BuyType.Hat) {
            if (hatSkin != null  &&  hatSkin != skin) {
                hatSkin.SetSkinDequipped();
            }
            hatSkin = skin;
        }
        else if (skin.SkinType == BuyType.Glasses) {
            if (glassesSkin != null  && glassesSkin != skin) {
                glassesSkin.SetSkinDequipped();
            }
            glassesSkin = skin;
        }
        else if (skin.SkinType == BuyType.Mask) {
            if (maskSkin != null && maskSkin != skin) {
                maskSkin.SetSkinDequipped();
            }
            maskSkin = skin;
        }
    }
}
