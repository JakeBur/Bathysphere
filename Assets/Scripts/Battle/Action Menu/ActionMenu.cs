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
            TurnManager.Instance.OnTurnAdvance += (ITurnOrderEntry entry) => HandleSelect(SelectionManager.Instance.selected);
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

            if(selectable is PlayerCharacter)
            {
                PlayerCharacter playerCharacter = selectable as PlayerCharacter;

                if (playerCharacter.TurnActive())
                {
                    int buttonCount = 0;

                    foreach(IBattleAction battleAction in playerCharacter.menuActions)
                    {
                        Button button = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();
                        button.GetComponentInChildren<TextMeshProUGUI>().text = battleAction.ToString();
                        
                        RectTransform buttonTransform = button.GetComponent<RectTransform>();
                       
                        /*buttonTransform.anchoredPosition = new Vector2(
                            buttonTransform.anchoredPosition.x,
                            buttonYOffset + (buttonCount * buttonSpacing) + buttonTransform.sizeDelta.y);*/

                        button.onClick.AddListener(() => HandleActionButtonPressed(battleAction));

                        buttons.Add(button);
                        buttonCount++;
                    }
                }
            }

            canvas.gameObject.SetActive(buttons.Count > 0);
        }

        private void HandleActionButtonPressed(IBattleAction battleAction)
        {
            (SelectionManager.Instance.selected as PlayerCharacter).PrimeMenuAction(battleAction);
        }
    }
}