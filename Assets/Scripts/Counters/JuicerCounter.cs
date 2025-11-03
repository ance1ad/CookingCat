using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JuicerCounter : BaseCounter, IHasProgress {
    
    [SerializeField] private List<KitchenObjectSO> _fruitsSO;
    [SerializeField] private List<KitchenObjectSO> _maySliced;
    [SerializeField] private KitchenObjectSO _orangeColorJuiceSO;
    [SerializeField] private KitchenObject _juiceKO;
    [SerializeField] private Plate _plate;

    private List<KitchenObjectSO> addedIngredients = new List<KitchenObjectSO>();



    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs {
        public float Progress;
    }


    public event Action OnKitchenObjectTake;
    private float fruitCountToEnable = 3;
    private int fruitCurrentCount;
    private bool readyToGive;


    private bool ValidateKitchenObject(KitchenObjectSO ko) {
        return _fruitsSO.Contains(ko);
    }   


    public override void Interact(Player player) {
        if (_juicerWorking) {
            MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("JuicerWorking"));
            return;
        }
        // Кладет фрукт
        if (player.HasKitchenObject() && ValidateKitchenObject(player.GetKitchenObject().GetKitchenObjectSO()) && !readyToGive) {
            ++fruitCurrentCount;
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                Progress = fruitCurrentCount / fruitCountToEnable
            });
            addedIngredients.Add(player.GetKitchenObject().GetKitchenObjectSO());
            HighlightManager.Instance.OnObjectDrop();
            player.GetKitchenObject().DestroyMyself();
            if (fruitCurrentCount == fruitCountToEnable) {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("JuicerNowWorking"));
                StartCoroutine(JuicerRoutine());
                SoundManager.Instance.PlayLoopSfx("Juicer");
            }
        }

        // Забрать компот
        else if (readyToGive) {
            if (player.HasKitchenObject() && player.GetKitchenObject() is Plate) {
                _plate = player.GetKitchenObject() as Plate;
                _plate.AddIngredient(GetKitchenObject());
                fruitCurrentCount = 0;
                readyToGive = false;
                OnKitchenObjectTake.Invoke();
                addedIngredients.Clear();
            }
            else if (!player.HasKitchenObject()) {
                KitchenObject.CreateKitchenObject(_orangeColorJuiceSO, player);
                HighlightManager.Instance.OnObjectTake(_orangeColorJuiceSO);
                DishVisual _juiceDish = player.GetKitchenObject().GetComponent<DishVisual>();
                _juiceDish.Ingredients = new List<KitchenObjectSO>(addedIngredients);
                GetKitchenObject().DestroyMyself();
                readyToGive = false;
                OnKitchenObjectTake.Invoke();
                addedIngredients.Clear();
            }
            // Рандом ингредиент
            else if(player.HasKitchenObject() && !(player.GetKitchenObject() is Plate)) {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("JuiceReady"));
            }
        }
        else if (player.HasKitchenObject() ) {
            if(MaySliced(player.GetKitchenObject().GetKitchenObjectSO())) {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("ProductNotReadyForJuicer"));

            }
            else if (player.GetKitchenObject().GetKitchenObjectSO() == _orangeColorJuiceSO) {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("JuiceTaken"));

            }
            else {
                MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("CantPutOnJuicer"));

            }
        }
        else {
            MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("TakeWhatYouWantPutOnJuicer"));

        }
    }

    private bool _juicerWorking;
    private IEnumerator JuicerRoutine () {
        _juicerWorking = true;
        float workingTime = 4f;
        float progress = 0;
        while(progress < workingTime) {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                Progress = progress / workingTime
            });
            progress += Time.deltaTime;
            yield return null;
        }
        
        _juicerWorking = false;
        readyToGive = true;
        KitchenObject.CreateKitchenObject(_orangeColorJuiceSO, this);
        DishVisual _juiceDish = GetKitchenObject().GetComponent<DishVisual>();
        _juiceDish.Ingredients = new List<KitchenObjectSO>(addedIngredients);


        MessageUI.Instance.ShowPlayerPopup(LocalizationManager.Get("JuiceReady"));

        fruitCurrentCount = 0;
        SoundManager.Instance.StopLoopSfx("Juicer");
    }


    private bool MaySliced(KitchenObjectSO obj) {
        return _maySliced.Contains(obj);
    }
}
