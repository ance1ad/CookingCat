using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/* В целом логика следующая: 
 выдаем пиццу в зависимости от положенных ингредиентов
если совпадает с 1 из _pizzes то выдаем ее
если не совпало выдаем абстрактную пиццу
 */
public class OvenCounter : BaseCounter, IHasProgress{


    [SerializeField] private List<DishVisual> _pizzes; // собранные пиццы, внутри ингредиенты
    [SerializeField] private DishVisual _abstractPizza;
    [SerializeField] private List<KitchenObjectSO> forbiddenObjects;
    [SerializeField] private List<KitchenObjectSO> potentialObjects;
    [SerializeField] private KitchenObjectSO _testo;
    [SerializeField] private OvenVisual _visual;


    private List<KitchenObjectSO> puttedIngredients = new List<KitchenObjectSO>();

    private KitchenObject outputPizza;
    private Plate plate;
    private KitchenObjectSO playerSO;

    private bool suit = true; // Подходит/не подходит
    private bool bakingNow = false; // В данный момент готовится
    private bool ready = false; // Готов к выдаче
    private int minimumIngredientsCount = 3; // Готов к выдаче
    private float timeToBakePizza = 10f;
    private float timer;
    // Прорисовка спрайта
    public event Action<IngredientAddedArgs> OnIndridientAddedInOven;

    public class IngredientAddedArgs {
        public KitchenObjectSO ingredient;
        public Sprite icon;
    }

    // Запуск духовки
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float Progress;
    }
    public event Action OnKitchenObjectTake;

    private void Start() {
    }


    public override void Interact(Player player) {
        if (bakingNow) {
            MessageUI.Instance.ShowPlayerPopup("Пицца готовится, уже ничего не положить");
            return;
        }
        // Кладка ингредиента
        if (player.HasKitchenObject() && !ready) {
            playerSO = player.GetKitchenObject().GetKitchenObjectSO();
            // Проверка на запрещёнку
            if (potentialObjects.Contains(playerSO)) {
                MessageUI.Instance.ShowPlayerPopup(playerSO.objectName + " нужно сначала нарезать");
                return;
            }
            if (forbiddenObjects.Contains(playerSO)) {
                MessageUI.Instance.ShowPlayerPopup(playerSO.objectName + " нельзя положить в духовку");
                return;
            }
            if (puttedIngredients.Contains(playerSO)) {
                MessageUI.Instance.ShowPlayerPopup(playerSO.objectName + " ингредиент уже добавлен в духовку");
                return;
            }
            puttedIngredients.Add(playerSO);
            _visual.PlayAnimation();

            OnIndridientAddedInOven?.Invoke(new IngredientAddedArgs {
                ingredient = playerSO,
                icon = playerSO.sprite
            });
            HighlightManager.Instance.OnObjectDrop();
            player.GetKitchenObject().DestroyMyself();
        }
        else if (!HasKitchenObject() && !ready) {
            MessageUI.Instance.ShowPlayerPopup("У вас в руках ничего нет");
        }
        if (ready) {
            PizzaTake(player);
        }
    }




    private IEnumerator Timer(float time) {
        bakingNow = true;
        float timeNow = 0f;
        while(timeNow < time) {
            timeNow += Time.deltaTime;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                Progress = timeNow / time
            });
            yield return null;
        }
        ready = true;
        bakingNow = false;
        MessageUI.Instance.ShowPlayerPopup("Пицца готова!");
        _visual.SetPizzaReady();
        SoundManager.Instance.PlaySFX("Success");
        SoundManager.Instance.StopLoopSfx("Oven");
        
    }


    // Запуск готовки и выдача заказа
    public override void AlternativeInteract(Player player) {
        if (bakingNow) {
            MessageUI.Instance.ShowPlayerPopup("Духовка уже запущена");
            return;
        }
        if (puttedIngredients.Count < minimumIngredientsCount) {
            MessageUI.Instance.ShowPlayerPopup("Положите минимум " + minimumIngredientsCount + " ингредиента");
            return;
        }
        if (!puttedIngredients.Contains(_testo)) {
            MessageUI.Instance.ShowPlayerPopup("Положите обязательно тесто!");
            return;
        }

        MessageUI.Instance.ShowPlayerPopup("Запуск духовки");
        SoundManager.Instance.PlayLoopSfx("Oven");
        BakePizza();
        StartCoroutine(Timer(timeToBakePizza));
    }






    private void BakePizza() {

        // Логика подбора пиццы
        foreach (var pizza in _pizzes) {
            // Проверка ингредиента
            foreach (var ingredient in puttedIngredients) {
                // Ничего лишнего
                if (pizza.info.ingredients.Length != puttedIngredients.Count) {
                    suit = false; // Разное количество, 100% фигня
                }
                // Полнота, т.к повторов нет проверим, что все ингредиенты пиццы есть в духовке
                if (!pizza.info.ingredients.Contains(ingredient)) {
                    suit = false; // 1 ингредиент не совпал, минус вся пицца
                }
            }
            // Если все ок, мы нашли рецепт
            if (suit) {
                AssignPizza(pizza);
                return;
            }
            // Фигня, заново
            else {
                suit = true;
            }
        }
        // Прошлись по всем пиццам, нельзя такую собрать вернём ебаный импровизированный пицца
        AssignPizza(_abstractPizza);
    }



    private void AssignPizza(DishVisual pizza) {
        if (pizza.info.prefab.TryGetComponent(out KitchenObject pizzaFounded)) {
            outputPizza = pizzaFounded;
        }
    }



    private void PizzaTake(Player player) {
        // Выдача пиццы если есть в тарелку
        if (player.HasKitchenObject()) {
            if (player.GetKitchenObject() is Plate) {
                plate = player.GetKitchenObject() as Plate;

                KitchenObject.CreateKitchenObject(outputPizza.GetKitchenObjectSO(), this);
                DishVisual pizza = GetKitchenObject().GetComponent<DishVisual>();
                pizza.Ingredients = new List<KitchenObjectSO>(puttedIngredients);
                _visual.PlayAnimation();
                plate.AddIngredient(pizza.GetComponent<KitchenObject>());

                ClearData();

            }
            else {
                MessageUI.Instance.ShowPlayerPopup("У вас заняты руки, чтобы забрать пиццу");
            }
        }
        else {
            _visual.PlayAnimation();
            HighlightManager.Instance.OnObjectTake(outputPizza.GetKitchenObjectSO());
            KitchenObject.CreateKitchenObject(outputPizza.GetKitchenObjectSO(), player);

            DishVisual pizza = player.GetKitchenObject().GetComponent<DishVisual>();
            pizza.Ingredients = new List<KitchenObjectSO>(puttedIngredients);
            
            ClearData();
        }
    }

    private void ThiefPizzaTake(ThiefCat thief) {
        if (ready) {
            KitchenObject.CreateKitchenObject(outputPizza.GetKitchenObjectSO(), thief);
            _visual.PlayAnimation();
            DishVisual pizza = thief.GetKitchenObject().GetComponent<DishVisual>();
            pizza.Ingredients = new List<KitchenObjectSO>(puttedIngredients);
            ClearData(); 
        }
    }


    private void ClearData() {
        outputPizza = null;
        ready = false;
        _visual.ClearIcons();
        puttedIngredients.Clear();
        timer = 0;
        OnKitchenObjectTake?.Invoke();
    }


    public override bool ThiefInteract(ThiefCat thief) {
        if (ready) {
            ThiefPizzaTake(thief);
            ClearData();
            return true;
        }
        if (puttedIngredients.Count >= minimumIngredientsCount && puttedIngredients.Contains(_testo) && !bakingNow ) {
            BakePizza();
            StartCoroutine(Timer(timeToBakePizza));
            return true;
        }
        
        return false;
    }

    public override bool CanTakeObject() => (!bakingNow && (puttedIngredients.Count >= minimumIngredientsCount && puttedIngredients.Contains(_testo)) || ready);

}
