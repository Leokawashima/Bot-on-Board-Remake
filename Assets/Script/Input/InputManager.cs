using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static event Action<Vector2> MouseMove;
    public static event Action MouseClick;

    private InputMap map;

    private void OnEnable()
    {
        map = new();
        map.UI.Point.started += OnMousePosition;
        map.UI.Point.performed += OnMousePosition;
        map.UI.Point.canceled += OnMousePosition;
        map.UI.Click.performed += OnMouseClick;
        map.Enable();
    }
    private void OnDisable()
    {
        map.UI.Point.started -= OnMousePosition;
        map.UI.Point.performed -= OnMousePosition;
        map.UI.Point.canceled -= OnMousePosition;
        map.UI.Click.performed -= OnMouseClick;
        map.Disable();
        map = null;
    }

    private void OnMousePosition(InputAction.CallbackContext context_)
    {
        MouseMove?.Invoke(context_.ReadValue<Vector2>());
    }
    private void OnMouseClick(InputAction.CallbackContext _)
    {
        MouseClick?.Invoke();
    }
}