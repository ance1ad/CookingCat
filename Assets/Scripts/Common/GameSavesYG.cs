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
        
        public int bunCount;
        public int tomatoCount;
        public int cheeseCount;
        public int greeneryCount;
        public int meatCount;
        public int onionCount;
        public int mushroomCount;
        public int testoCount;
        public int appleCount;
        public int tangerineCount;
        
        
        // Уровень прогресс etc
        public int countBurgers;
        public int countPizza;
        public int countDrinks;
        
        public int level;
        public int xp;
        
        
        
        
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

