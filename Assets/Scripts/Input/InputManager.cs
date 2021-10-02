using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class InputManager : MonoBehaviour
{
    private InputActions inputActions;

    private void Awake()
    {
        inputActions = new InputActions();

        inputActions.Enable();

        inputActions.Battle.Select.started += OnClick;
    }

    private void OnClick(InputAction.CallbackContext context)
    {
        Vector2 screenPoint = Mouse.current.position.ReadValue();
        Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out RaycastHit hitInfo);

        if(hitInfo.collider != null)
        {
            IClickable[] clickables = hitInfo.collider.gameObject.GetComponents<IClickable>();

            foreach(IClickable clickable in clickables)
            {
                clickable.Click();
            }
        }
    }
}
