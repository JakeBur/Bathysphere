using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// A PlayerCharacter is a player controled combatant.
    /// </summary>
    public abstract class PlayerCharacter : Combatant
    {
        [SerializeField]
        protected int _moveSpeed;

        /// <summary>
        /// List of actions that should appear in the contextual action menu when this PlayerCharacter is selected.
        /// </summary>
        public List<PlayerAction> menuActions;

        public Dictionary<Type, List<PlayerAction>> comboActions;

        /// <summary>
        /// The currently primed action, to be supplied when GetPrimedActions() is called.
        /// </summary>
        private PlayerAction PrimedAction
        {
            get
            {
                return _primedAction;
            }

            set
            {
                if(_primedAction != null)
                {
                    _primedAction.EndPreview();
                    BattleInputManager.Instance.OnHoverGridSquare -= _primedAction.UpdatePreview;
                }
                _primedAction = value;
                if(_primedAction != null)
                {
                    BattleInputManager.Instance.OnHoverGridSquare += _primedAction.UpdatePreview;
                    _primedAction.BeginPreview();
                }
            }
        }

        private PlayerAction _primedAction;

        private Move _moveAction;

        protected void Awake()
        {
            menuActions = new List<PlayerAction>();
            comboActions = new Dictionary<Type, List<PlayerAction>>();

            _moveAction = new Move(this, 1);

            InitializeMenuActions();
            InitializeComboActions();
        }

        protected new void Start()
        {
            base.Start();
            BattleInputManager.Instance.OnCancelPressed += Cancel;
        }

        protected abstract void InitializeMenuActions();
        protected abstract void InitializeComboActions();

        public override List<PlayerAction> GetAvailableMenuActions()
        {
            if(this.TurnActive())
            {
                List<PlayerAction> actions = new List<PlayerAction>(menuActions);

                // clean out any unapplicable actions
                // TODO maybe we can have greyed out actions as an option with a hover that says what the requirements are
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    if (actions[i].IsInstant && !actions[i].CanApplyToSquare(null))
                    {
                        actions.RemoveAt(i);
                    }
                    else if(actions[i].CalculateCost(null) > actionPoints.CurrentPoints)
                    {
                        actions.RemoveAt(i);
                    }
                }

                return actions;
            }

            return new List<PlayerAction>();
        }

        public override List<PlayerAction> GetAvailableComboActions(Entity entity)
        {
            Type type = entity.GetType();

            if (comboActions.ContainsKey(type))
            {
                List<PlayerAction> actions = new List<PlayerAction>(comboActions[type]);

                // clean out any unapplicable actions
                // TODO maybe we can have greyed out actions as an option with a hover that says what the requirements are
                for(int i = actions.Count - 1; i >= 0; i--)
                {
                    if(actions[i].IsInstant && !actions[i].CanApplyToSquare(null))
                    {
                        actions.RemoveAt(i);
                    }
                }

                return actions;
            }

            return new List<PlayerAction>();
        }

        /// <summary>
        /// Primes the given IBattleAction if its contained in the character's menu actions.
        /// </summary>
        /// <param name="action"></param>
        public void PrimeAction(PlayerAction action)
        {
            if(action.IsInstant)
            {
                if(action.CanApplyToSquare(null))
                {
                    ApplyAction(action, null);
                }
            }
            else
            {
                PrimedAction = action;
            }
        }

        /// <summary>
        /// Cancels the currently primed action, if any.
        /// </summary>
        private void Cancel()
        {
            if (SelectionManager.Instance.selected == this as ISelectable) PrimedAction = _moveAction;
            else PrimedAction = null;
        }

        public override void Select()
        {
            BattleGridManager.Instance.OnSquareClicked.AddListener(TryApplyAction, 1);
            PrimedAction = _moveAction;
        }

        public override void Deselect()
        {
            BattleGridManager.Instance.OnSquareClicked.RemoveListener(TryApplyAction);
            PrimedAction = null;
        }

        protected override void StartTurnBehavior()
        {
            if (SelectionManager.Instance.selected == this as ISelectable)
            {
                SelectionManager.Instance.Select(this);
            }
        }

        protected override void EndTurnBehavior()
        {
            PrimedAction = null;
        }

        public override bool ApplyAction(CombatantAction combatantAction, GridSquare targetSquare)
        {
            bool success = base.ApplyAction(combatantAction, targetSquare);

            PrimedAction = PrimedAction;

            if(actionPoints.CurrentPoints == 0)
            {
                EndTurn();
            }

            return success;
        }

        protected void TryApplyAction(GridSquare square, PriorityEvent<GridSquare> context)
        {
            if (PrimedAction == null) return;

            if (PrimedAction.CanApplyToSquare(square))
            {
                ApplyAction(PrimedAction, square);
                PrimedAction = null;

                // if we could do something, consume the event
                context.ConsumeEvent();
            }
        }

        

        private class Move : PlayerAction
        {
            public Move(PlayerCharacter player, int cost) : base(player, cost)
            {

            }

            //public override bool IsInstant() => false;

            public override int CalculateCost(GridSquare targetSquare)
            {
                List<GridSquare> path = BattleGridManager.Instance.Grid.CalculatePath(_player.Square, targetSquare);

                return (base.CalculateCost(targetSquare) * path.Count - 1) / _player._moveSpeed;
            }

            public override void Apply(GridSquare gridSquare)
            {
                _player.Square = gridSquare;
            }

            public override void BeginPreview()
            {
                Highlighter.Instance.moveHighlights.Highlight(this.FindTargetableSquares());
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                if (!base.CanApplyToSquare(gridSquare)) return false;

                if (gridSquare.Entities.Count != 0) return false;

                List<GridSquare> path = BattleGridManager.Instance.Grid.CalculatePath(_player.Square, gridSquare);
                if (_player._moveSpeed * _player.actionPoints.CurrentPoints < path.Count - 1) return false;

                return true;
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return CanApplyToSquare(gridSquare);
            }

            public override void EndPreview()
            {
                Highlighter.Instance.moveHighlights.Clear();
                Highlighter.Instance.pathHighlights.Clear();
            }

            public override void UpdatePreview(GridSquare gridSquare)
            {
                Highlighter.Instance.pathHighlights.Clear();

                if(CanApplyToSquare(gridSquare))
                {
                    List<GridSquare> path = BattleGridManager.Instance.Grid.CalculatePath(_player.Square, gridSquare);
                    path.RemoveAt(0);
                    Highlighter.Instance.pathHighlights.Highlight(path);
                }

            }

            public override List<GridSquare> FindThreatenedSquaresAtTarget(GridSquare gridSquare)
            {
                return new List<GridSquare>();
            }
        }
    }
}



