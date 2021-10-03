using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Companion class to the selection manager that applies player-driven actions like movement, attacks, and abilities.
    /// </summary>
    public class BattleActionManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the currently selected IBattleActor, if any.
        /// This is null when the currently selected object does not implement IBattleActor
        /// </summary>
        private IBattleActor _currentActor;

        void Start()
        {
            // match object tracking to the selection manager
            SelectionManager.Instance.OnSelect += TrySetActor;

            // try to execute actions when a square on the grid is clicked
            BattleGridManager.Instance.OnSquareClicked.AddListener(TryExecuteAction, 1);
        }

        /// <summary>
        /// If the given selectable has a component that implements IActor, it will be set as the current actor.
        /// Otherwise the currently selected object has no actor and no actions should be taken.
        /// </summary>
        /// <param name="selectable"></param>
        private void TrySetActor(ISelectable selectable)
        {
            IBattleActor actor = selectable.TryGetGameObject()?.GetComponent<IBattleActor>();

            _currentActor = actor;
        }

        /// <summary>
        /// Executes the highest priority valid IBattleAction associated with the currently selected IBattleActor, if any.
        /// </summary>
        /// <param name="square">The target GridSquare for the action.</param>
        /// <param name="context">The priority event that called this action.</param>
        private void TryExecuteAction(GridSquare square, PriorityEvent<GridSquare> context)
        {
            // if the currently selected object isn't a IBattleActor, then we will not execute any actions
            if (_currentActor == null) return;

            List<IBattleAction> actions = _currentActor.GetPrimedActions();

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


