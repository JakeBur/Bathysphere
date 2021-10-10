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

            _moveAction = new Move(this);

            InitializeMenuActions();
        }

        protected void Start()
        {
            BattleInputManager.Instance.OnCancelPressed += Cancel;
        }

        protected abstract void InitializeMenuActions();

        public override List<IBattleAction> GetPrimedActions()
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
            PrimedAction = _moveAction;
        }

        public override void Deselect()
        {
            PrimedAction = null;
        }

        public override void StartTurn()
        {
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

        private class Move : PlayerAction
        {
            private PlayerCharacter player;

            public Move(PlayerCharacter player)
            {
                this.player = player;
            }

            public override void Apply(GridSquare gridSquare)
            {
                player.Square = gridSquare;
                player.EndTurn();
            }

            public override void BeginPreview()
            {
                Highlighter.Instance.moveHighlights.Highlight(this.FindTargetableSquares());
            }

            public override bool CanApplyToSquare(GridSquare gridSquare)
            {
                return gridSquare.Entities.Count == 0 && GridSquare.Distance(player.Square, gridSquare) <= player._moveSpeed;
            }

            public override bool CanTargetSquare(GridSquare gridSquare)
            {
                return CanApplyToSquare(gridSquare);
            }

            public override void EndPreview()
            {
                Highlighter.Instance.moveHighlights.Clear();
            }

            public override List<GridSquare> FindThreatenedSquares()
            {
                return this.FindTargetableSquares();
            }

            public override void UpdatePreview(GridSquare gridSquare)
            {

            }
        }
    }
}



