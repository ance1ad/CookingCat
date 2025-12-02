using System;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        

        // Данные для сохранения
        public float Coins;
        public int Gems;
        
        public bool TutorialPassed;
        public int CountCompleteOrders;
        
        public long lastPlayTime;
        
        
        public float SoundForce;
        public float SFXForce;
        
        
        public List<string> OwnedPurchase = new (); // Что есть уже у игрока
        public List<SkinObject> AppliedSkins = new ();
        public List<UpgradeObject> Upgrades = new ();
    }
}

[Serializable]
public struct SkinObject {
    public PurchaseType type; 
    public string id;
}
[Serializable]
public struct UpgradeObject {
    public string id;
    public int count;
}

