using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// An Entity is a GameObject that can be placed on a BattleGrid at a specific GridSquare
    /// </summary>
    public abstract class Entity : MonoBehaviour, ISelectable
    {
        /// <summary>
        /// Action to invoke when this Entity is removed from play.
        /// Supplies this Entity as an argument.
        /// </summary>
        public Action<Entity> OnRemovedFromPlay;

        /// <summary>
        /// The square that this Entity resides in.
        /// Set to move this Entity to a different square.
        /// </summary>
        public GridSquare Square
        {
            get
            {
                return _gridSquare;
            }

            set
            {
                if(_gridSquare) _gridSquare.RemoveEntity(this);
                _gridSquare = value;

                if(_gridSquare != null)
                {
                    _gridSquare.AddEntity(this);
                    transform.position = _gridSquare.transform.position;
                }
            }
        }

        /// <summary>
        /// Private reference to the GridSquare this Entity is occupying.
        /// </summary>
        private GridSquare _gridSquare;

        private List<ActionInterceptor> _actionInterceptors;

        protected void Awake()
        {
            _actionInterceptors = new List<ActionInterceptor>();
        }

        /// <summary>
        /// Call when this Entity is removed from play for any reason.
        /// (For instance, if a child class of Entity can Die, it should call RemoveFromPlay() as part of the process of dying).
        /// </summary>
        public void RemoveFromPlay()
        {
            OnRemovedFromPlay?.Invoke(this);
        }

        public abstract void Select();
        public abstract void Deselect();

        public virtual bool IsSelectable()
        {
            return true;
        }

        public virtual void Initialize() { }

        public abstract List<PlayerAction> GetAvailableMenuActions();
        public abstract List<PlayerAction> GetAvailableComboActions(Entity entity);

        public void AddActionInterceptor(ActionInterceptor actionInterceptor)
        {
            _actionInterceptors.Add(actionInterceptor);
        }

        public void RemoveActionInterceptor(ActionInterceptor actionInterceptor)
        {
            _actionInterceptors.Remove(actionInterceptor);
        }

        /// <summary>
        /// Intercepts the given action with any ActionInterceptors on this Entity. 
        /// Returns whether the action was consumed in the interception.
        /// </summary>
        /// <param name="battleAction">The action to intercept.</param>
        /// <returns>True if the action was consumed, false otherwise.</returns>
        public bool TryInterceptAction(BattleAction battleAction, Combatant actingCombatant)
        {
            if(_actionInterceptors.Count > 0)
            {
                for (int i = _actionInterceptors.Count - 1; i >= 0; i--)
                {
                    if (_actionInterceptors[i].CanIntercept(battleAction, actingCombatant, Square))
                    {
                        _actionInterceptors[i].InterceptAction(battleAction, actingCombatant, Square);

                        if (_actionInterceptors[i].BlocksAction)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
