using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

namespace Battle
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        private InputActions inputActions;

        public Action<GridSquare> OnHoverGridSquare;

        private void Awake()
        {
            Instance = this;

            inputActions = new InputActions();

            inputActions.Enable();

            inputActions.Battle.Select.started += OnClick;
        }

        private void Update()
        {
            foreach (IClickable clickable in GetHoveredClickables())
            {
                if(clickable is GridSquare)
                {
                    OnHoverGridSquare?.Invoke(clickable as GridSquare);
                }
            }
        }

        private void OnClick(InputAction.CallbackContext context)
        {
            foreach (IClickable clickable in GetHoveredClickables())
            {
                clickable.Click();
            }
        }

        private IClickable[] GetHoveredClickables()
        {
            Vector2 screenPoint = Mouse.current.position.ReadValue();
            Physics.Raycast(Camera.main.ScreenPointToRay(screenPoint), out RaycastHit hitInfo);

            if (hitInfo.collider != null)
            {
                return hitInfo.collider.gameObject.GetComponents<IClickable>();
            }
            else
            {
                return new IClickable[0];
            }
        }
    }
}