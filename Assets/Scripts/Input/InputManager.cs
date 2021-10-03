using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Linq;

namespace Battle
{
    /// <summary>
    /// A Battle.InputManager is an input handler for player input during battle.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance;

        /// <summary>
        /// Reference to the Unity InputActions object that will dispurse input events to the InputManager.
        /// </summary>
        private InputActions _inputActions;

        /// <summary>
        /// Invoked when any GridSquare is being hovered over this frame.
        /// Supplies the hovered GridSquare as an argument.
        /// </summary>
        public Action<GridSquare> OnHoverGridSquare;

        /// <summary>
        /// Invoked when the player presses the cancel button.
        /// </summary>
        public Action OnCancelPressed;

        private void Awake()
        {
            Instance = this;

            _inputActions = new InputActions();

            _inputActions.Enable();

            _inputActions.Battle.Select.started += HandleClick;
            _inputActions.Battle.Cancel.started += (InputAction.CallbackContext context) => OnCancelPressed?.Invoke();
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

        /// <summary>
        /// Handler for when the player clicks the mouse.
        /// </summary>
        /// <param name="context">Context supplied by the Unity Input System.</param>
        private void HandleClick(InputAction.CallbackContext context)
        {
            foreach (IClickable clickable in GetHoveredClickables())
            {
                clickable.Click();
            }
        }

        /// <summary>
        /// Gets an array of all clickables on the object currently hovered over by the mouse.
        /// </summary>
        /// <returns>An array of IClickables currently hovered over by the player's mouse.</returns>
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