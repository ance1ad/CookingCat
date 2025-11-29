using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerSkinChanger : MonoBehaviour {
    [SerializeField] private Transform _hatPointSpawn;
    [SerializeField] private Transform _glassesPointSpawn;
    [SerializeField] private Transform _maskPointSpawn;
    [SerializeField] private SkinObjectSO _defaultSkinColor;
    [SerializeField] private Renderer _catRenderer;
    [SerializeField] private PlayerData _data;
    
    
    public SkinObjectSO HatCurrentSkin {get; private set;}
    public SkinObjectSO GlassesCurrentSkin {get; private set;}
    public SkinObjectSO MaskCurrentSkin {get; private set;}
    public SkinObjectSO ColorCurrentSkin {get; private set;}

    public static PlayerSkinChanger Instance { get; private set; }
    
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("2 PlayerSkinChanger has already been instantiated");
            return;
        }
        Instance = this;
        _data.OnSkinActivate += EquipSkin;
        _data.OnSkinDeactivate += DequipSkin;
    }

    private void EquipSkin(SkinObjectSO skinSO) {
        Debug.Log("Надевание скина " + skinSO.GetLocalizationName());
        if (skinSO.PurchaseType == PurchaseType.Hat) {
            SetSkin(skinSO, HatCurrentSkin, _hatPointSpawn);
        }
        else if (skinSO.PurchaseType == PurchaseType.Glasses) {
            SetSkin(skinSO, GlassesCurrentSkin, _glassesPointSpawn);
        }
        else if (skinSO.PurchaseType == PurchaseType.Mask) {
            SetSkin(skinSO, MaskCurrentSkin, _maskPointSpawn);
        }
        else if (skinSO.PurchaseType == PurchaseType.Color) {
            SetColor(skinSO);
        }
    }

    private void DequipSkin(SkinObjectSO skinSO) {
        Debug.Log("Деактивирование скина " + skinSO.GetLocalizationName());
        if (skinSO.PurchaseType == PurchaseType.Hat) {
            DestroySkin(_hatPointSpawn);
            HatCurrentSkin = null;
        }
        else if (skinSO.PurchaseType == PurchaseType.Glasses) {
            DestroySkin(_glassesPointSpawn);
            GlassesCurrentSkin = null;
        }
        else if (skinSO.PurchaseType == PurchaseType.Mask) {
            DestroySkin(_maskPointSpawn);
            MaskCurrentSkin = null;
        }
        else if (skinSO.PurchaseType == PurchaseType.Color) {
            SetColor(_defaultSkinColor);
            ColorCurrentSkin = _defaultSkinColor;
        }
    }

    
    
    private void SetSkin(SkinObjectSO skinSO, SkinObjectSO CurrentSkin, Transform pointSpawn) {

        if (skinSO == CurrentSkin) {
            Debug.LogWarning("This " + CurrentSkin + " skin is already in use");
            return;
        }

        DestroySkin(pointSpawn);

        GameObject glassesInstance = Instantiate(skinSO.Prefab,  pointSpawn);
        Debug.Log("I put on " + CurrentSkin);
        CurrentSkin = skinSO;
    }
    

    private void SetColor(SkinObjectSO colorSkinSO) {
        Debug.Log("Смена шёрстки");
        _catRenderer.material.mainTexture = colorSkinSO.Texture;
        ColorCurrentSkin = colorSkinSO;
    }
    
    private void DestroySkin(Transform skinParent) {
        if (skinParent.childCount != 0) {
            Destroy(skinParent.GetChild(0).gameObject);
        }
    }
}
