using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinChanger : MonoBehaviour {
    [SerializeField] private Transform _hatPointSpawn;
    [SerializeField] private Transform _glassesPointSpawn;
    [SerializeField] private Transform _maskPointSpawn;
    
    private List<SkinObjectSO> _playerSkins = new List<SkinObjectSO>() {};

    public bool HasSkin(SkinObjectSO skin) => _playerSkins.Contains(skin);
    public void AddSkin(SkinObjectSO skin) => _playerSkins.Add(skin);
    
    public bool SkinIsEquipped(SkinObjectSO skinObjectSo) {
        return HatCurrentSkin == skinObjectSo || 
               GlassesCurrentSkin ==  skinObjectSo ||
               MaskCurrentSkin == skinObjectSo;
    }
    public SkinObjectSO HatCurrentSkin {get; private set;}
    public SkinObjectSO GlassesCurrentSkin {get; private set;}
    public SkinObjectSO MaskCurrentSkin {get; private set;}



    public static PlayerSkinChanger Instance { get; private set; }
    
    
    
    
    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("2 PlayerSkinChanger has already been instantiated");
            return;
        }
        Instance = this;
    }

    public void EquipSkin(SkinObjectSO skinSO) {
        if (skinSO._skinType == BuyType.Hat) {
            SetHatSkin(skinSO);
        }
        else if (skinSO._skinType == BuyType.Glasses) {
            SetGlassesSkin(skinSO);
        }
        else if (skinSO._skinType == BuyType.Mask) {
            SetMaskSkin(skinSO);
        }
    }

    public void DequipSkin(SkinObjectSO skinSO) {
        if (skinSO._skinType == BuyType.Hat) {
            DestroySkin(_hatPointSpawn);
            HatCurrentSkin = null;
        }
        else if (skinSO._skinType == BuyType.Glasses) {
            DestroySkin(_glassesPointSpawn);
            GlassesCurrentSkin = null;
        }
        else if (skinSO._skinType == BuyType.Mask) {
            DestroySkin(_maskPointSpawn);
            MaskCurrentSkin = null;
        }
    }

    private void DestroySkin(Transform skinParent) {
        if (skinParent.childCount != 0) {
            Destroy(skinParent.GetChild(0).gameObject);
        }
    }

    private void SetHatSkin(SkinObjectSO hatSkinSO) {
        if (!_playerSkins.Contains(hatSkinSO)) {
            return;
        }
        if (hatSkinSO == HatCurrentSkin) {
            Debug.LogWarning("This hat skin is already in use");
            return;
        }

        DestroySkin(_hatPointSpawn);

        GameObject hatInstance = Instantiate(hatSkinSO._prefab, _hatPointSpawn);

        
        Debug.Log("Я надел шапку");
        HatCurrentSkin = hatSkinSO;
    }
    
    
    private void SetGlassesSkin(SkinObjectSO glassesSkinSO) {
        if (!_playerSkins.Contains(glassesSkinSO)) {
            return;
        }
        if (glassesSkinSO == GlassesCurrentSkin) {
            Debug.LogWarning("This glasses skin is already in use");
            return;
        }

        DestroySkin(_glassesPointSpawn);

        GameObject glassesInstance = Instantiate(glassesSkinSO._prefab,  _glassesPointSpawn);
        
        
        Debug.Log("Я надел очки");
        
        GlassesCurrentSkin = glassesSkinSO;
    }
    
    
    private void SetMaskSkin(SkinObjectSO maskSkinSO) {
        if (!_playerSkins.Contains(maskSkinSO)) {
            // Не куплен
            return;
        }
        if (maskSkinSO == MaskCurrentSkin) {
            Debug.LogWarning("This mask skin is already in use");
            return;
        }

        DestroySkin(_maskPointSpawn);
        GameObject maskInstance = Instantiate(maskSkinSO._prefab,  _maskPointSpawn);
        
        
        Debug.Log("Я надел маску");
        MaskCurrentSkin = maskSkinSO;
    }
}
