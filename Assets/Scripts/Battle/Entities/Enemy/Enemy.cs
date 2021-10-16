using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    /// <summary>
    /// An Enemy is an AI controled Combatant hostile to the player.
    /// </summary>
    public partial class Enemy : Combatant
    {
        public List<PlayerCharacter> targets;

        public GameObject pathMarkerPrefab;
        public GameObject targetMarkerPrefab;

        private new void Start()
        {
            base.Start();

            targets = GameObject.FindObjectsOfType<PlayerCharacter>().ToList();
        }

        public override void Deselect()
        {
        }

        public override void Select()
        {
        }

        protected override void StartTurnBehavior()
        {
            List<List<GridSquare>> pathsToTargets = new List<List<GridSquare>>();

            foreach(PlayerCharacter target in targets)
            {
                pathsToTargets.Add(Square.Grid.CalculatePath(Square, target.Square));
            }

            int bestIndex = 0;

            for(int i = 1; i < targets.Count; i++)
            {
                if(pathsToTargets[i] == null)
                {
                    continue;
                }

                if(pathsToTargets[i].Count < pathsToTargets[bestIndex].Count)
                {
                    bestIndex = i;
                }
            }

            Attack attack = new Attack(this, 2);

            if (attack.CanApplyToSquare(targets[bestIndex].Square))
            {
                attack.TryApply(targets[bestIndex].Square);
            }
            else
            {
                EndTurn();
            }

            MoveToNearestPlayer move = new MoveToNearestPlayer(this, 1, targets[bestIndex]);
            if(move.CanApplyToSquare(null))
            {
                move.TryApply(null);
            }

            attack = new Attack(this, 2);

            if(attack.CanApplyToSquare(targets[bestIndex].Square))
            {
                attack.TryApply(targets[bestIndex].Square);
            }
            else
            {
                EndTurn();
            }
        }

        public override List<PlayerAction> GetAvailableMenuActions()
        {
            return new List<PlayerAction>();
        }

        public override List<PlayerAction> GetAvailableComboActions(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}

