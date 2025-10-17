using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/* � ����� ������ ���������: 
 ������ ����� � ����������� �� ���������� ������������
���� ��������� � 1 �� _pizzes �� ������ ��
���� �� ������� ������ ����������� �����
 */
public class OvenCounter : BaseCounter, IHasProgress{


    [SerializeField] private List<DishVisual> _pizzes; // ��������� �����, ������ �����������
    [SerializeField] private DishVisual _abstractPizza;
    [SerializeField] private List<KitchenObjectSO> forbiddenObjects;
    [SerializeField] private List<KitchenObjectSO> potentialObjects;
    [SerializeField] private KitchenObjectSO _testo;
    [SerializeField] private OvenVisual _visual;


    private List<KitchenObjectSO> puttedIngredients = new List<KitchenObjectSO>();

    private KitchenObject outputPizza;
    private Plate plate;
    private KitchenObjectSO playerSO;

    private bool suit = true; // ��������/�� ��������
    private bool bakingNow = false; // � ������ ������ ���������
    private bool ready = false; // ����� � ������
    private int minimumIngredientsCount = 3; // ����� � ������
    private float timeToBakePizza = 5f;
    private float timer;
    // ���������� �������
    public event Action<IngredientAddedArgs> OnIndridientAddedInOven;

    public class IngredientAddedArgs {
        public KitchenObjectSO ingredient;
        public Sprite icon;
    }

    // ������ �������
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float Progress;
    }
    public event Action OnKitchenObjectTake;

    private void Start() {
    }


    public override void Interact(Player player) {
        if (bakingNow) {
            ShowPopupText("����� ���������, ��� ������ �� ��������");
            return;
        }
        // ������ �����������
        if (player.HasKitchenObject() && !ready) {
            playerSO = player.GetKitchenObject().GetKitchenObjectSO();
            // �������� �� ����������
            if (potentialObjects.Contains(playerSO)) {
                ShowPopupText("������� " + "\"" + playerSO.objectName + "\"" + " ����� ������� ��������");
                return;
            }
            if (forbiddenObjects.Contains(playerSO)) {
                ShowPopupText("������� "  + "\"" + playerSO.objectName + "\"" + " ������ �������� � �������");
                return;
            }
            if (puttedIngredients.Contains(playerSO)) {
                ShowPopupText("���� ���������� ��� �������� � �������");
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
        if (!player.HasKitchenObject()) {
            ShowPopupText("� ��� � ����� ������ ���");
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
        ShowPopupText("����� ������!");
        _visual.SetPizzaReady();
    }


    // ������ ������� � ������ ������
    public override void AlternativeInteract(Player player) {
        if (puttedIngredients.Count < minimumIngredientsCount) {
            ShowPopupText("�������� ������� " + minimumIngredientsCount + " �����������");
            return;
        }
        if (!puttedIngredients.Contains(_testo)) {
            ShowPopupText("�������� ����������� �����!");
            return;
        }

        ShowPopupText("������ �������");
        BakePizza();
        StartCoroutine(Timer(timeToBakePizza));
    }






    private void BakePizza() {

        // ������ ������� �����
        foreach (var pizza in _pizzes) {
            // �������� �����������
            foreach (var ingredient in puttedIngredients) {
                // ������ �������
                if (pizza.info.ingredients.Length != puttedIngredients.Count) {
                    suit = false; // ������ ����������, 100% �����
                }
                // �������, �.� �������� ��� ��������, ��� ��� ����������� ����� ���� � �������
                if (!pizza.info.ingredients.Contains(ingredient)) {
                    suit = false; // 1 ���������� �� ������, ����� ��� �����
                }
            }
            // ���� ��� ��, �� ����� ������
            if (suit) {
                AssignPizza(pizza);
                return;
            }
            // �����, ������
            else {
                suit = true;
            }
        }
        // �������� �� ���� ������, ������ ����� ������� ����� ������ ����������������� �����
        AssignPizza(_abstractPizza);
    }



    private void AssignPizza(DishVisual pizza) {
        if (pizza.info.prefab.TryGetComponent(out KitchenObject pizzaFounded)) {
            outputPizza = pizzaFounded;
        }
    }



    private void PizzaTake(Player player) {
        // ������ ����� ���� ���� � �������
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
                ShowPopupText("����� ��� ������, �������� ������ � �������� �!");
            }
        }
        else {
            _visual.PlayAnimation();
            HighlightManager.Instance.OnObjectTake(outputPizza.GetKitchenObjectSO());
            KitchenObject.CreateKitchenObject(outputPizza.GetKitchenObjectSO(), player);

            DishVisual pizza = player.GetKitchenObject().GetComponent<DishVisual>();
            pizza.Ingredients = new List<KitchenObjectSO>(puttedIngredients);
            player.visualPlate.SetActive(true);
            
            ClearData();
        }
    }

    private void ThiefPizzaTake(ThiefCat thief) {
        KitchenObject.CreateKitchenObject(outputPizza.GetKitchenObjectSO(), thief);
        DishVisual pizza = thief.GetKitchenObject().GetComponent<DishVisual>();
        pizza.Ingredients = new List<KitchenObjectSO>(puttedIngredients);
        ClearData();
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
        if (outputPizza != null) {
            ThiefPizzaTake(thief);
            ClearData();
            return true;
        }
        if (puttedIngredients.Count >= minimumIngredientsCount && puttedIngredients.Contains(_testo)) {
            BakePizza();
            StartCoroutine(Timer(timeToBakePizza));
            return true;
        }
        
        return false;
    }

    public override bool CanTakeObject() => (outputPizza != null || puttedIngredients.Count >= minimumIngredientsCount && puttedIngredients.Contains(_testo));

}
