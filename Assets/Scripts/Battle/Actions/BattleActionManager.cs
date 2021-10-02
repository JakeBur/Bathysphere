using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BattleActionManager : MonoBehaviour
    {
        IBattleActor currentActor;

        void Start()
        {
            SelectionManager.Instance.OnSelect += TrySetActor;
            BattleGridManager.Instance.OnSquareClicked.AddListener(TryExecuteAction, 1);
        }

        /// <summary>
        /// If the given selectable has a component that implements IActor, it will be set as the current actor.
        /// Otherwise the currently selected object has no actor and no actions should be taken.
        /// </summary>
        /// <param name="selectable"></param>
        private void TrySetActor(ISelectable selectable)
        {
            IBattleActor actor = selectable.GetGameObject()?.GetComponent<IBattleActor>();

            currentActor = actor;
        }

        private void TryExecuteAction(GridSquare square, PriorityAction<GridSquare> context)
        {
            if (currentActor == null) return;

            List<IBattleAction> actions = currentActor.GetPrimedActions();

            // apply the first action that can be applied
            foreach(IBattleAction action in actions)
            {
                if(action.CanApplyToSquare(square))
                {
                    action.Apply(square);
                    
                    // if we could do something, consume the event
                    context.ConsumeEvent();
                    break;
                }
            }
        }
    }
}


