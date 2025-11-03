using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language { RU, EN }
public class LocalizationManager : MonoBehaviour  {
    public static Language CurrentLanguage = Language.EN;

    private static Dictionary<string, string> RU = new Dictionary<string, string>
    {
        { "AddDishes", "Добавьте блюда в заказ" },
        { "OrderTaken", "Взят заказ: {0}" },
        { "DontSlice", "Здесь нельзя нарезать" },
        { "DontHaveObjectToSlice", "У вас нет предмета для нарезки" },
        { "DontHaveObject", "У вас нет предмета, который можно было бы положить" },
        { "DontHaveTray", "Сначала возьмите поднос слева" },
        { "ObjectIsSliced", "Обьект уже нарезан, заберите его" },
        { "DontPutObject", "Этот обьект нельзя положить сюда" },
        { "TableOccupied", "Стол уже занят другим предметом" },
        { "NewOrder", "Новый заказ №{0} !" },
        { "TakeTray", "Возьмите поднос слева" },
        { "NeedCourier", "Продукты закончились, нужен курьер" },
        { "ProductRotten", "Этот продукт пропал, его только выбросить" },
        // Juicer
        { "JuicerWorking", "Соковыжималка уже запущена" },
        { "JuicerNowWorking", "Запущена соковыжималка" },
        { "JuiceReady", "Напиток уже готов" },
        { "ProductNotReadyForJuicer", "Сначала почистите или порежьте" },
        { "JuiceTaken", "Вы уже забрали напиток, только не пейте его" },
        { "CantPutOnJuicer", "С этого напиток не сварить" },
        { "TakeWhatYouWantPutOnJuicer", "Возьмите то, что хотите положить в соковыжималку" },
        // Oven
        { "PizzaIsBaking", "Пицца готовится, уже ничего не положить" },
        { "OvenIsRunning", "Духовка уже запущена" },
        { "OvenRun", "Запуск духовки..." },
        { "PizzaIsReady", "Пицца готова!" },
        { "NeedToSlicedInOven", "{0}  нужно сначала нарезать" },
        { "DontPutInOven", "{0}  нельзя положить в духовку" },
        { "AlreadyInOven", "{0}  уже есть в духовке" },
        { "YourHandsIsEmpty", "У вас в руках ничего нет" },
        { "YourHandsIsBusy", "У вас заняты руки" },
        { "PutMinimumIngredients", "Положите минимум {0} ингредиента" },
        { "TestoNeeded", "Положите обязательно тесто" },
        // Plates
        { "CompleteOrderFirst", "Сначала выполните взятый заказ" },
        { "YouAlreadyTakeTray", "Вы уже взяли поднос, выполните заказ" },
        // Stove
        { "MeatIsReady", "Мясо готово!" },
        { "MeatIsOvercooked", "Мясо пережарилось!" },
        { "ProductNotFryible", "Продукт \"{0}\" нельзя пожарить!"},
        { "DontHaveMeat", "Возьмите мясо в руки чтобы пожарить его!"},
        // Trash Counter
        { "TrayIsDestroyable", "Нельзя выкинуть поднос, соберите заказ!"},
        { "NothingToDrop", "Вам нечего выкинуть!"},
        { "YouDropIs", "Вы выкинули  + {0}!"},
        
        // Actions
        { "BuyButtonText", "Купить"},
        { "EquipButtonText", "Надеть"},
        { "DequipButtonText", "Снять"},
        // Скинов магазин оновый
        { "SkinStoreTitleText", "Магазин"},
        { "SortHats", "Шапки"},
        { "SortGlasses", "Очки"},
        { "SortMasks", "Маски"},
        { "Upgrades", "Апгрейды"},
        // Thief-cat
        { "ThiefSteals", "Кот-вор собирается что-то украсть!"},
        { "ThiefStealResult", "Кот-вор украл {0}!"},
        { "ThiefGetOut", "Вы прогнали кота вора!"},
        { "HandsNotFreeForThief", "У вас заняты лапы !"},
        
        
    };

    private static Dictionary<string, string> EN = new Dictionary<string, string>
    {
        { "AddDishes", "Add dishes to the order" },
        { "OrderTaken", "Order taken: {0}" },
        { "DontSlice", "It can't be sliced" },
        { "DontHaveObjectToSlice", "You don't have an item to slice" },
        { "DontHaveObject", "You don't have an item to put" },
        { "ObjectIsSliced", "The object has already been sliced, take it away" },
        { "DontHaveTray", "First, take the tray on the left" },
        { "DontPutObject", "You can't put this object here." },
        { "TableOccupied", "The table is already occupied by another item" },
        { "NewOrder", "New order №{0} !" },
        { "TakeTray", "Take the tray on the left" },
        { "NeedCourier", "We're out of groceries, we need a courier." },
        { "ProductRotten", "This product is gone, it just needs to be thrown away." },
        // Juicer
        { "JuicerWorking", "The juicer is already running" },
        { "JuicerNowWorking", "The juicer is running" },
        { "JuiceReady", "The juice is ready" },
        { "ProductNotReadyForJuicer", "Peel or slice it first" },
        { "JuiceTaken", "You've already taken the drink, just don't drink it." },
        { "CantPutOnJuicer", "You can't make a drink from that" },
        { "TakeWhatYouWantPutOnJuicer", "Take what you want to put in the juicer." },
        // Oven
        { "PizzaIsBaking", "Pizza is being prepared, there's nothing left to put in" },
        { "OvenIsRunning", "The oven is already running" },
        { "OvenRun", "Starting the oven..." },
        { "PizzaIsReady", "The pizza is ready!" },
        { "NeedToSlicedInOven", "The {0} must be cut first" },
        { "DontPutInOven", "{0} should not be put in the oven" },
        { "AlreadyInOven", "{0}  is already in the oven" },
        { "YourHandsIsEmpty", "You have nothing in your hands." },
        { "YourHandsIsBusy", "Your hands are busy." },
        { "PutMinimumIngredients", "Add at least {0} ingredients" },
        { "TestoNeeded", "Be sure to put the dough" },
        // Plates
        { "CompleteOrderFirst", "First, complete the order you took" },
        { "YouAlreadyTakeTray", "You have already taken the tray, complete the order." },
        // Stove
        { "MeatIsReady", "The meat is ready!" },
        { "MeatIsOvercooked", "The meat is overcooked!" },
        { "ProductNotFryible", "The product \"{0}\" cannot be fried !"},
        { "DontHaveMeat", "Take the meat in your hands to roast it!"},
        // Trash Counter
        { "TrayIsDestroyable", "You can not throw away the tray, collect the order!"},
        { "NothingToDrop", "You have nothing to throw away!"},
        { "YouDropIs", "You threw away the {0}"},
        
        // Actions
        { "BuyButtonText", "Buy"},
        { "EquipButtonText", "Equip"},
        { "DequipButtonText", "Dequip"},
        
        // Скинов магазин оновый
        { "SkinStoreTitleText", "Skin shop"},
        { "SortHats", "Hats"},
        { "SortGlasses", "Glasses"},
        { "SortMasks", "Masks"},
        { "Upgrades", "Upgrades"},
        // Thief
        { "ThiefSteals", "The cat-thief is going to steal something!"},
        { "ThiefStealResult", "The cat-thief stole a {0}!"},
        { "ThiefGetOut", "You chased the cat-thief away!"},
        { "HandsNotFreeForThief", "Are your paws busy !"},
    };

    public static string Get(string key, params object[] args)
    {
        string text = CurrentLanguage switch
        {
            Language.EN => EN.ContainsKey(key) ? EN[key] : key,
            _ => RU.ContainsKey(key) ? RU[key] : key
        };
        return string.Format(text, args);
    }

    public static void SwipeLanguage() {
        CurrentLanguage = (CurrentLanguage == Language.EN ? CurrentLanguage = Language.RU  : CurrentLanguage = Language.EN);
    }
}
