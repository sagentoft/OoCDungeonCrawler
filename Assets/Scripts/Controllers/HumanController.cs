using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class HumanController : MonoBehaviour
{
    private ControlManager controlManager;
    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Move.performed += Move_OnPerformed;
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= Move_OnPerformed;
        inputActions.Player.Disable();
    }

    private void Move_OnPerformed(InputAction.CallbackContext obj)
    {
        Vector2 input = obj.ReadValue<Vector2>();
        
        controlManager.SubmitMove(Vector2Int.RoundToInt(input), ControlSource.Human);
    }

    public void Initialize(ControlManager manager)
    {
        controlManager = manager;
    }
}
