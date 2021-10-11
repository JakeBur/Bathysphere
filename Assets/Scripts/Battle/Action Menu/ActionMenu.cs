using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Battle
{
    public class ActionMenu : MonoBehaviour
    {
        public Canvas canvas;
        public RectTransform buttonContainer;
        public GameObject buttonPrefab;
        public float buttonSpacing;
        public float buttonYOffset;

        private List<Button> buttons;

        private void Awake()
        {
            buttons = new List<Button>();
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

        private void HandleSelect(ISelectable selectable)
        {
            transform.position = selectable.TryGetGameObject().transform.position;

            //clear list of buttons
            buttons.ForEach(button => Destroy(button.gameObject));
            buttons.Clear();

            ITurnOrderEntry currentTurnOrderEntry = TurnOrder.CurrentEntry;
            PlayerCharacter playerCharacter = currentTurnOrderEntry as PlayerCharacter;

            if(playerCharacter)
            {
                foreach (PlayerAction playerAction in selectable.GetAvailableMenuActions())
                {
                    CreateActionButton(playerCharacter, playerAction);
                }

                foreach(PlayerAction playerAction in playerCharacter.GetAvailableComboActions(selectable as Entity))
                {
                    CreateActionButton(playerCharacter, playerAction);
                }
            }

            canvas.gameObject.SetActive(buttons.Count > 0);
        }

        private void CreateActionButton(PlayerCharacter playerCharacter, PlayerAction playerAction)
        {
            Button button = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();
            button.GetComponentInChildren<TextMeshProUGUI>().text = playerAction.ToString();

            RectTransform buttonTransform = button.GetComponent<RectTransform>();

            button.onClick.AddListener(() => playerCharacter.PrimeAction(playerAction));

            buttons.Add(button);
        }
    }
}