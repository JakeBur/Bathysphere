using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            _moveAction = new Move(this, 1);

            InitializeMenuActions();
        }

        protected new void Start()
        {
            base.Start();
            BattleInputManager.Instance.OnCancelPressed += Cancel;
        }

        protected abstract void InitializeMenuActions();

        public override List<IBattleAction> GetAvailableMenuActions()
        {
            List<IBattleAction> actions = new List<IBattleAction>();
            if(this.TurnActive())
            {
                if(PrimedAction != null) actions.Add(PrimedAction);
                //actions.Add(new Move(this));
            }

            return actions;
        }

        /// <summary>
        /// Primes the given IBattleAction if its contained in the character's menu actions.
        /// </summary>
        /// <param name="action"></param>
        public void PrimeMenuAction(PlayerAction action)
        {
            if(menuActions.Contains(action))
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
            BattleGridManager.Instance.OnSquareClicked.AddListener(TryExecuteAction, 1);
            PrimedAction = _moveAction;
        }

        public override void Deselect()
        {
            BattleGridManager.Instance.OnSquareClicked.RemoveListener(TryExecuteAction);
            PrimedAction = null;
        }

        public override void StartTurn()
        {
            base.StartTurn();

            if (SelectionManager.Instance.selected == this as ISelectable)
            {
                SelectionManager.Instance.Select(this);
            }
        }

        public override void EndTurn()
        {
            PrimedAction = null;
            base.EndTurn();
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

        protected void TryExecuteAction(GridSquare square, PriorityEvent<GridSquare> context)
        {
            if (PrimedAction == null) return;

            if (PrimedAction.CanApplyToSquare(square))
            {
                ApplyAction(PrimedAction, square);

                // if we could do something, consume the event
                context.ConsumeEvent();
            }
        }

        private class Move : PlayerAction
        {
            public Move(PlayerCharacter player, int cost) : base(player, cost)
            {

            }

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

            public override List<GridSquare> FindThreatenedSquares()
            {
                return this.FindTargetableSquares();
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
        }
    }
}



