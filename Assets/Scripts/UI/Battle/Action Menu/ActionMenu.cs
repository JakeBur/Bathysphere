using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace Battle
{
    public class ActionMenu : MonoBehaviour
    {
        public Canvas canvas;
        public RectTransform buttonContainer;
        public RectTransform descriptionContainer;
        public GameObject buttonPrefab;
        public GameObject actionDescriptionPrefab;
        public float buttonSpacing;
        public float buttonYOffset;

        protected List<ActionMenuButton> buttons;
        ActionMenuButton hoveredButton;

        protected ActionMenuDescription actionDescription;

        protected List<EventTrigger.Entry> eventTriggerEntries;

        private void Awake()
        {
            buttons = new List<ActionMenuButton>();
            canvas.gameObject.SetActive(false);
        }

        // Start is called before the first frame update
        void Start()
        {
            SelectionManager.Instance.OnSelect += HandleSelect;

            TurnOrder.Instance.OnTurnAdvance += (ITurnOrderEntry entry) => HandleSelect(SelectionManager.Instance.selected);
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Updates the action menu with the set of all available actions for the newly selected entity, if applicable
        /// </summary>
        /// <param name="selectable">The selectable to use as a basis for action menu state</param>
        private void HandleSelect(ISelectable selectable)
        {
            //clear list of buttons
            buttons.ForEach(button => button.Shutdown());
            buttons.Clear();
            
            if (!selectable.HasGameObject()) return;

            transform.position = selectable.TryGetGameObject().transform.position;

            ITurnOrderEntry currentTurnOrderEntry = TurnOrder.CurrentEntry;
            PlayerCharacter playerCharacter = currentTurnOrderEntry as PlayerCharacter;

            if(playerCharacter)
            {
                foreach (PlayerAction playerAction in selectable.GetAvailableMenuActions())
                {
                    buttons.Add(new ActionMenuButton(playerCharacter, playerAction, this));
                }

                foreach(PlayerAction playerAction in playerCharacter.GetAvailableComboActions(selectable as Entity))
                {
                    buttons.Add(new ActionMenuButton(playerCharacter, playerAction, this));
                }
            }

            canvas.gameObject.SetActive(buttons.Count > 0);
        }

        private void SetHoveredButton(ActionMenuButton actionMenuButton)
        {
            hoveredButton = actionMenuButton;
            actionDescription?.Shutdown();
            actionDescription = new ActionMenuDescription(hoveredButton.playerCharacter, hoveredButton.playerAction, this);
        }
        
        private void ClearHoveredButton()
        {
            actionDescription?.Shutdown();
            actionDescription = null;
            hoveredButton = null;
        }

        /// <summary>
        /// Wrapper for unity button behavior that codifies that responds to button events with updates to button and action menu state.
        /// </summary>
        protected class ActionMenuButton
        {
            public ActionMenu parentMenu;
            public PlayerCharacter playerCharacter;
            public PlayerAction playerAction;
            public Button button;

            protected EventTrigger.TriggerEvent onTriggerHoverEnter;
            protected EventTrigger.TriggerEvent onTriggerHoverExit;

            public ActionMenuButton(PlayerCharacter playerCharacter, PlayerAction playerAction, ActionMenu actionMenu)
            {
                this.playerCharacter = playerCharacter;
                this.playerAction = playerAction;
                parentMenu = actionMenu;

                button = Instantiate(actionMenu.buttonPrefab, actionMenu.buttonContainer).GetComponent<Button>();
                EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();

                // display
                button.GetComponentInChildren<TextMeshProUGUI>().text = playerAction.GetDisplayName();

                // events
                button.onClick.AddListener(() => playerCharacter.PrimeAction(playerAction));

                EventTrigger.Entry onPointerEnter = new EventTrigger.Entry();
                onPointerEnter.eventID = EventTriggerType.PointerEnter;
                onPointerEnter.callback.AddListener((BaseEventData eventData) => OnHoverEnter());
                eventTrigger.triggers.Add(onPointerEnter);
                
                EventTrigger.Entry onPointerExit = new EventTrigger.Entry();
                onPointerExit.eventID = EventTriggerType.PointerExit;
                onPointerExit.callback.AddListener((BaseEventData eventData) => OnHoverExit());
                eventTrigger.triggers.Add(onPointerExit);
            }

            protected void OnHoverEnter()
            {
                parentMenu.SetHoveredButton(this);
                
            }
            
            protected void OnHoverExit()
            {
                parentMenu.ClearHoveredButton();
            }

            public void Shutdown()
            {
                Destroy(button.gameObject);
            }
        }

        protected class ActionMenuDescription
        {
            public ActionMenu parentMenu;
            public PlayerAction playerAction;
            public GameObject instance;

            public ActionMenuDescription(PlayerCharacter playerCharacter, PlayerAction playerAction, ActionMenu actionMenu)
            {
                parentMenu = actionMenu;
                this.playerAction = playerAction;

                instance = Instantiate(actionMenu.actionDescriptionPrefab, actionMenu.descriptionContainer);

                // display
                instance.GetComponentInChildren<TextMeshProUGUI>().text = playerAction.GetDisplayDescription();
            }

            public void Shutdown()
            {
                Destroy(instance.gameObject);
            }
        }
    }
}