using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {
    [SerializeField] private ThiefCat _thiefCat;
    
    // Для тутора
    [SerializeField] private List<BaseCounter> _allCounters;
    
    [SerializeField] private List<BaseCounter> _clearCounters;
    [SerializeField] private List<BaseCounter> _containerCounters;
    [SerializeField] private List<BaseCounter> _cuttingCounters;
    [SerializeField] private List<BaseCounter> _ovenCounter;
    [SerializeField] private List<BaseCounter> _stoveCounter;
    [SerializeField] private List<BaseCounter> _trashCounters;
    [SerializeField] private List<BaseCounter> _juicerCounters;

    
    

    
    // Методы шоб показать конкретную группу
    public void ShowClearCounters() {
        foreach (var counter in _clearCounters) {
            counter.gameObject.SetActive(true);
        }
    }
    
    
    public void StartTutorial() {
        
        _thiefCat.StopThiefCycle();
    }


    public void CountersTutorial() {
        
    }
    
    
    
    public void CloseTutorial() {
        _thiefCat.StartThiefCycle();
    }
    
    /* впринципе можно разделить на такие блоки
    -  Показать все обьекты для взаимодействия, можно 
        поочереди подсвечивать каждый тип с помощью higlight 
        maanger и показывать что с ним можно делать и как работать
    - Как брать и отдавать заказ, тут и нужна будет стрелка чтоб показать на время и кол-во заказов
    - как выполнять заказ на пиццу, бургер и напиток
    - Сказать что делать с котом вором
    - Про сгнивший продукт
    */ 
}
