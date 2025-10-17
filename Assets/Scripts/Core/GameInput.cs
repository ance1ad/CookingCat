using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameInput : MonoBehaviour {
    private PlayerInputActions playerInputActions;

    // События для взаимодействия
    public event EventHandler OnInteractAction;
    public event EventHandler OnAlternativeInteractAction;

    private void Awake() {
        playerInputActions = new PlayerInputActions(); // Сгенерирован input system
        playerInputActions.Enable();


        // Interact мы создали, а система сконструировала его
        // performed - если действие успешно выполнилось
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternative.performed += InteractAlternative_performed;
    }

    private void InteractAlternative_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnAlternativeInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public void InteractButton() {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public void AlternativeButton() {
        OnAlternativeInteractAction?.Invoke(this, EventArgs.Empty);
    }



    // Вектор движения
    public Vector2 GetMovementVectorNormalized() =>
        playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
}
