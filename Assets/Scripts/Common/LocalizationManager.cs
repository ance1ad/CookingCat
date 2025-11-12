using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language { RU, EN }
public class LocalizationManager : MonoBehaviour  {
    public static Language CurrentLanguage = Language.RU;

   

    private static Dictionary<string, string> RU = new Dictionary<string, string>
    {
        { "AddDishes", "Добавьте блюда в заказ" },
        { "OrderTaken", "Взят заказ: #{0}" },
        { "NoOrders", "Заказов пока нет" },
        { "DontSlice", "Здесь нельзя нарезать" },
        { "ObjectNotSlicable", "Продукт \"{0}\" не нужно нарезать"},
        { "EmptySlicing", "На столе для нарезки ничего не лежит" },
        { "DontHaveObjectToSlice", "У вас нет предмета для нарезки" },
        { "DontHaveObject", "У вас нет ничего в руках" },
        { "DontHaveTray", "Сначала возьмите поднос слева" },
        { "ObjectIsSliced", "Ингредиент уже нарезан, заберите его" },
        { "DontPutObject", "Сюда нельзя положить этот предмет" },
        { "TableOccupied", "Стол уже занят другим предметом" },
        { "NewOrder", "Новый заказ #{0}" },
        { "OrderNumber", "Заказ #{0}" },
        { "TakeTray", "Возьмите поднос слева" },
        { "NeedCourier", "Продукты закончились, вызывай курьера!" },
        { "ProductRotten", "Этот продукт испортился, его только выбросить" },
        // Juicer
        { "JuicerWorking", "Соковыжималка уже запущена" },
        { "JuicerNowWorking", "Запущена соковыжималка" },
        { "JuiceReady", "Напиток готов" },
        { "ProductNotReadyForJuicer", "Сначала почистите или порежьте" },
        { "JuiceTaken", "Вы уже забрали напиток - только не выпейте его" },
        { "CantPutOnJuicer", "С этого напиток не сварить" },
        { "TakeWhatYouWantPutOnJuicer", "Возьмите то, что хотите положить в соковыжималку" },
        // Oven
        { "PizzaIsBaking", "Пицца готовится, уже ничего не положить" },
        { "OvenIsRunning", "Духовка уже запущена" },
        { "OvenRun", "Запуск духовки..." },
        { "PizzaIsReady", "Пицца готова!" },
        { "NeedToSlicedInOven", "{0}  нужно сначала нарезать" },
        { "DontPutInOven", "Предмет  \"{0}\" нельзя положить в духовку" },
        { "AlreadyInOven", "{0}  уже есть в духовке" },
        { "YourHandsIsEmpty", "У вас в руках ничего нет" },
        { "YourHandsIsBusy", "У вас заняты руки" },
        { "PutMinimumIngredients", "Положите минимум {0} ингредиента" },
        { "TestoNeeded", "Не забудьте положить тесто" },
        // Plates
        { "CompleteOrderFirst", "Сначала выполните взятый заказ" },
        { "YouAlreadyTakeTray", "Вы уже взяли поднос — сначала выполните заказ" },
        // Stove
        { "MeatIsReady", "Мясо готово!" },
        { "MeatIsOvercooked", "Мясо пережарилось!" },
        { "ProductNotFryible", "Продукт \"{0}\" нельзя пожарить!"},
        { "DontHaveMeat", "Возьмите мясо в руки чтобы пожарить его!"},
        // Trash Counter
        { "TrayIsDestroyable", "Нельзя выкидывать поднос — сначала собери заказ!"},
        { "NothingToDrop", "Вам нечего выкинуть!"},
        { "YouDropIs", "Вы выбросили {0}!"},
        
        // Actions
        { "BuyButtonText", "Купить"},
        { "EquipButtonText", "Надеть"},
        { "DequipButtonText", "Снять"},
        // Скинов магазин оновый
        { "SkinStoreTitleText", "Магазин"},
        { "SortHats", "Шапки"},
        { "SortGlasses", "Очки"},
        { "SortMasks", "Маски"},
        { "Colors", "Цвета"},
        { "Upgrades", "Апгрейды"},
        // Thief-cat
        { "ThiefSteals", "Кот-вор собирается что-то украсть!"},
        { "ThiefStealResult", "Кот-вор украл {0}!"},
        { "ThiefGetOut", "Вы прогнали кота вора!"},
        { "HandsNotFreeForThief", "У вас заняты лапы!"},
        // Магазин продуктов
        { "Pieces", "шт."},
        {"AllPrice", "Итого: {0}$"},
        {"NothingSet", "Вы ничего не выбрали"},
        {"LackCoins", "Недостаточно коинов"},
        {"LackGems", "Недостаточно гемов"},
        {"CourierFound", "Курьер найден"},
        {"ProductStoreTitle", "Продукты"},
        // Popup role
        {"CourierRole", "Курьер"},
        {"ThiefRole", "Кот-вор"},
        {"ClientRole", "Клиент"},
        // Order
        {"NewOrderCanvas", "Новый заказ"},
        {"CompleteOrderCanvas", "Выполнение заказа"},
        {"OrderShowTimerText", "{0} сек"},
        {"LateText", "Опоздание"},
        {"OrderStatusText", "Заказ"},
        {"CloseIn", "Закроется через"},
        // Order results
        {"OrderResultsTitle", "Заказ выполнен"},
        {"ManyErrors", "Много ошибок"},
        {"ClientUnhappy", "Клиент остался недоволен..."},
        {"NotBad", "Неплохо!"},
        {"GoodJob", "Отличная работа!"},
        {"TimeNotLate", "Время: {0:0} сек. успел!"},
        {"TimeLate", "Время: {0:0} сек. опоздание!"},
        {"Accuracy", "Точность: {0:0}%\n{1}"},
        // Combo
        {"ComboX", "Комбо x{0} (+{1}%)"},
        {"ComboNone", "Комбо: нет"},
        {"ComboBreak", "Комбо прервано на x{0}"},
        
        // TUTOR
        { "Step1Hello", "Привет! Добро пожаловать ко мне на кухню, нажми тут для продолжения" },
        { "Step2Buttons", "В игре есть 2 кнопки взаимодействия" },
        { "Step3RightButton", "Кнопка справа (E) используется чтобы что-то брать или класть" },
        { "Step4LeftButton", "Кнопка слева (F) используется, чтобы нарезать продукты или включать духовку" },
        { "Step5IntroObjects", "Теперь давай познакомимся с основными предметами на кухне" },
        { "Step6Containers", "Продукты лежат в холодильниках и контейнерах" },
        { "Step7Trash", "Иногда продукты пропадают, их придётся выкинуть" },
        { "Step8Tables", "Столы нужны для того, чтобы класть на них предметы" },
        { "Step9Cutting", "Продукты нужно резать на разделочных досках" },
        { "Step10Stove", "Мясо для бургера жарится на плите, следи за тем, чтобы оно не пережарилось!" },
        { "Step11Oven", "Ингредиенты для пиццы кладутся сразу в волшебную духовку" },
        { "Step12OvenUse", "После того, как положил все ингредиенты, включай духовку на (F)" },
        { "Step13JuicerPrep", "Для приготовления сока, на разделочной доске сначала нужно подготовить фрукты или овощи" },
        { "Step14JuicerUse", "Затем загружай подготовленный продукт в соковыжималку, достаточно 3шт. для приготовления сока" },
        { "Step15Delivery", "Следи за количеством продуктов на кухне, но если что можно всегда заказать курьера" },
        { "Step16OrderStart", "Чтобы принять заказ подойти к столу и нажми (E), заказ собирается на подносе" },
        { "Step17OrderHint", "У тебя будет время запомнить заказ и подсмотреть его пару раз в правом верхнем углу" },
        { "Step18Timer", "Также следи за временем пока выполняешь заказ, клиенты ждут!" },
        { "Step19Burger", "Бургер собирается на ранее взятом подносе, клади ингредиенты прямо на него" },
        { "Step20Pizza", "Пиццу после приготовления вытаскивай из духовки и клади на поднос" },
        { "Step21JuiceReady", "Тоже самое и с соковыжималкой" },
        { "Step22Serve", "После сбора заказа беги обратно к клиенту" },
        { "Step23Shop", "В магазине можно закупиться скинами или апгрейдами" },
        { "Step24Bonk", "Ну конечно он не признается сам... твой кот иногда может скушать то, что ты держишь, следи за ним!" },
        { "Step25Thief", "Ах да... следи внимательно за кухней, иногда на нее может забежать вор, обязательно прогони его" },
        
        { "Step26GoodLuck", "Удачи! Держи немного монеток, можешь взять в магазине шапку шефа, она бесплатная!" },
        

        
        
        { "LastStep", "Ингредиенты можно пару раз подсмотреть слева сверху!" },
        
        // Order Tutorial
        { "CreateBurgerTutorial", "Давай научимся делать бургеры! Хватай поднос и принимай свой первый заказ!" },
        { "CreatePizzaTutorial", "Бургер получился вкусными, а теперь приготовим аппетитную пиццу!" },
        { "CreateDrinkTutorial", "Пицца получилась вкусной! Осталось освоить соковыжималку и сделать вкусный напиток!" },
        { "CombineOrderTutorial", "Попробуем средний заказ из нескольких блюд!" },
        { "GoodJobAndReady", "Отличная работа! Ты освоил базу — готовься к настоящим испытаниям!" },

        // Unf.. try again
        { "TryAgainBurger", "Давай попробуем еще раз приготовить бургер, запомни ингредиенты, если что, можешь подсмотреть" },
        { "TryAgainPizza", "Давай попробуем еще раз приготовить пиццу, запомни ингредиенты, если что, можешь подсмотреть" },
        { "TryAgainDrink", "Давай попробуем еще раз приготовить напиток, запомни ингредиенты, если что, можешь подсмотреть" },
        
        
        {"TutorialInvitation", "О Привет! Вижу ты не прошел стажировку, советую пройти для понимания процесса готовки!"},
        {"GladSeeYou", "Привет! рад тебя снова видеть на кухне!"},
        {"GladSeeYouReward", "Привет! рад тебя снова видеть на кухне! Лови небольшое вознаграждение!"},
        {"TimeBonus", "Привет! рад что ты играешь в нашу игру! Лови небольшое вознаграждение!"},
        
        
        // MainMenu
        {"PlayGame", "Играть"},
        {"StartTutorial", "Обучение"},
        
        
        
        {"SecMeasurement", "c."},
        
    };

    
     private static Dictionary<string, string> EN = new Dictionary<string, string> {
        { "AddDishes", "Add dishes to the order" },
        { "OrderTaken", "Order taken {0}" },
        { "NoOrders", "There are no orders yet" },
        
        { "DontSlice", "You can’t slice this" },
        { "ObjectNotSlicable", "{0} does not need to be sliced." },
        { "EmptySlicing", "There is nothing on the cutting table." },
        { "DontHaveObjectToSlice", "You don’t have an item to slice" },
        { "DontHaveObject", "You have nothing in your hands" },
        { "ObjectIsSliced", "This item is already sliced — take it!" },
        { "DontHaveTray", "Take the tray on the left first" },
        { "DontPutObject", "You can’t put this item here" },
        { "TableOccupied", "The table is already occupied" },
        { "NewOrder", "New order #{0}" },
        { "OrderNumber", "Order #{0}" },
        { "TakeTray", "Take the tray on the left" },
        { "NeedCourier", "Out of ingredients — call a courier" },
        { "ProductRotten", "This product has spoiled — throw it away" },

        // Juicer
        { "JuicerWorking", "The juicer is already running" },
        { "JuicerNowWorking", "Juicer is running" },
        { "JuiceReady", "The juice is ready" },
        { "ProductNotReadyForJuicer", "Peel or slice it first" },
        { "JuiceTaken", "You’ve already taken the drink" },
        { "CantPutOnJuicer", "You can’t make juice from this" },
        { "TakeWhatYouWantPutOnJuicer", "Pick an ingredient to put in the juicer" },

        // Oven
        { "PizzaIsBaking", "Pizza is baking — can’t add anything now" },
        { "OvenIsRunning", "The oven is already running" },
        { "OvenRun", "Turning on the oven..." },
        { "PizzaIsReady", "Pizza is ready!" },
        { "NeedToSlicedInOven", "{0} must be sliced first" },
        { "DontPutInOven", "You can’t put {0} in the oven" },
        { "AlreadyInOven", "{0} is already in the oven" },
        { "YourHandsIsEmpty", "Your hands are empty" },
        { "YourHandsIsBusy", "Your hands are busy" },
        { "PutMinimumIngredients", "Add at least {0} ingredients" },
        { "TestoNeeded", "Dough is required" },

        // Plates
        { "CompleteOrderFirst", "Complete your current order first" },
        { "YouAlreadyTakeTray", "You’ve already taken a tray — finish the order" },

        // Stove
        { "MeatIsReady", "The meat is ready!" },
        { "MeatIsOvercooked", "The meat is overcooked!" },
        { "ProductNotFryible", "You can’t fry {0}" },
        { "DontHaveMeat", "Take meat in your hands to cook it" },

        // Trash Counter
        { "TrayIsDestroyable", "You can’t discard the tray — finish the order first" },
        { "NothingToDrop", "There’s nothing to throw away" },
        { "YouDropIs", "You threw away {0}" },

        // Actions
        { "BuyButtonText", "Buy" },
        { "EquipButtonText", "Equip" },
        { "DequipButtonText", "Unequip" },

        // Skin Store
        { "SkinStoreTitleText", "Shop" },
        { "SortHats", "Hats" },
        { "SortGlasses", "Glasses" },
        { "Colors", "Colors"},
        { "SortMasks", "Masks" },
        { "Upgrades", "Upgrades" },

        // Thief-cat
        { "ThiefSteals", "The cat thief is about to steal something!" },
        { "ThiefStealResult", "The cat thief stole {0}!" },
        { "ThiefGetOut", "You scared the cat thief away!" },
        { "HandsNotFreeForThief", "Your paws are busy!" },

        // Product store
        { "Pieces", "pcs." },
        { "AllPrice", "Total: {0}$" },
        { "NothingSet", "You haven’t selected anything" },
        { "LackCoins", "Not enough coins" },
        { "LackGems", "Not enough gems" },
        { "CourierFound", "Courier found" },
        { "ProductStoreTitle", "Ingredients" },

        // Popup roles
        { "CourierRole", "Courier" },
        { "ThiefRole", "Cat thief" },
        { "ClientRole", "Client" },

        // Order
        { "NewOrderCanvas", "New order" },
        { "CompleteOrderCanvas", "Completing order" },
        { "OrderShowTimerText", "{0} sec" },
        { "LateText", "Late" },
        { "OrderStatusText", "Order" },
        { "CloseIn", "Closes in" },

        // Order results
        {"OrderResultsTitle", "Order completed"},
        { "ManyErrors", "Too many mistakes" },
        { "ClientUnhappy", "The customer is unhappy..." },
        { "NotBad", "Not bad!" },
        { "GoodJob", "Great job!" },
        { "TimeNotLate", "Time: {0:00} sec — on time!" },
        { "TimeLate", "Time: {0:00} sec — too late!" },
        { "Accuracy", "Accuracy: {0:0}%\n{1}" },

        // Combo
        { "ComboX", "Combo x{0} (+{1}%)" },
        { "ComboNone", "No combo" },
        { "ComboBreak", "Combo broken at x{0}" },
        
        // Tutoriallo
        
        
        { "Step1Hello", "Hi there! Welcome to my kitchen. Tap here to continue." },
        { "Step2Buttons", "There are two main action buttons in the game." },
        { "Step3RightButton", "The right button (E) is used to pick up or place items." },
        { "Step4LeftButton", "The left button (F) is for chopping food or turning on the oven." },
        { "Step5IntroObjects", "Now let’s take a look at the main kitchen objects." },
        { "Step6Containers", "You can find ingredients in fridges and containers." },
        { "Step7Trash", "Sometimes food goes bad — you’ll need to throw it away." },
        { "Step8Tables", "Tables are used to place items on." },
        { "Step9Cutting", "Use cutting boards to chop ingredients." },
        { "Step10Stove", "Meat for burgers is cooked on the stove — make sure it doesn’t burn!" },
        { "Step11Oven", "Pizza ingredients go straight into the magical oven." },
        { "Step12OvenUse", "Once all ingredients are inside, turn on the oven with (F)." },
        { "Step13JuicerPrep", "To make juice, prepare fruits or veggies on the cutting board first." },
        { "Step14JuicerUse", "Then put the prepared ingredients into the juicer — 3 pieces are enough." },
        { "Step15Delivery", "Keep an eye on your ingredient stock. If you run out, you can call a courier." },
        { "Step16OrderStart", "To accept an order, go to the table and press (E), the order is collected on a tray" },
        { "Step17OrderHint", "You’ll have some time to remember the order. You can also check it anytime in the top-right corner." },
        { "Step18Timer", "Keep an eye on the timer — customers don’t like to wait!" },
        { "Step19Burger", "Assemble burgers right on the tray you picked up earlier." },
        { "Step20Pizza", "Take the pizza out of the oven when it’s ready and place it on the tray." },
        { "Step21JuiceReady", "Do the same with the juicer once the juice is done." },
        { "Step22Serve", "When the order is ready, run back to the customer!" },
        { "Step23Shop", "In the shop, you can buy skins and upgrades." },
        { "Step24Bonk", "Of course he won't admit it himself... Your cat can sometimes eat what you're holding, keep an eye on him!" },
        { "Step25Thief", "Oh, and watch out — sometimes a thief might sneak into the kitchen! Chase him away!" },
        { "Step26GoodLuck", "Good luck! Here’s some cash, you can get the chef's hat from the store, it's free." },
        { "LastStep", "You can check the ingredients a couple of times!" },
        
        // Order Tutorial
        { "CreateBurgerTutorial", "Let's learn how to make burgers! Grab a tray and take your first order!" },
        { "CreatePizzaTutorial", "Now let's see how to make a delicious pizza!" },
        { "CreateDrinkTutorial", "Time for the easiest part — making some tasty drinks!" },
        { "CombineOrderTutorial", "Let’s try putting everything together into one average order!" },
        { "GoodJobAndReady", "Great job! You’ve mastered the basics — get ready for some tougher orders!" },

        // Unf.. try again
        { "TryAgainBurger", "Let's try to make the the burger again, carefully remember the ingredients, if you forget, you can check again." },
        { "TryAgainPizza", "Let's try to make the pizza again, carefully remember the ingredients, if you forget, you can check again." },
        { "TryAgainDrink", "Let's try to make the drink again, try to remember the ingredients, if you forget, you can check again." },
        
        {"TutorialInvitation", "Oh, hello! I see you haven't completed the internship, so i advise you to do it to understand the cooking process."},
        
        {"GladSeeYou", "Hi! Glad to see you in the kitchen!"},
        {"GladSeeYouReward", "Hi! Glad to see you in the kitchen! Here’s a little reward — you can grab the chef’s hat in the shop, it’s free."},
        {"TimeBonus", "Hi! I'm glad you're playing our game! Catch a small reward!"},
        // MainMenu
        {"PlayGame", "Play"},
        {"StartTutorial", "Tutorial"},
        {"SecMeasurement", "s."},
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
