using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    /// <summary>
    /// An Enemy is an AI controled Combatant hostile to the player.
    /// </summary>
    public class Enemy : Combatant
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
            if(actionPoints.CurrentPoints < 2)
            {
                OnEndTurn?.Invoke();
                return;
            }

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

            GameObject targetMarker = Instantiate(targetMarkerPrefab);
            targetMarker.transform.position = targets[bestIndex].transform.position;

            List<GameObject> pathMarkers = new List<GameObject>();

            Scheduler.Schedule(() =>
            {
                foreach(GridSquare gridSquare in pathsToTargets[bestIndex])
                {
                    GameObject pathMarker = Instantiate(pathMarkerPrefab);
                    pathMarker.transform.position = gridSquare.transform.position;
                    pathMarkers.Add(pathMarker);
                }

            }, 1f);

            Scheduler.Schedule(() =>
            {
                Destroy(targetMarker);
                pathMarkers.ForEach(marker => Destroy(marker));
                Square = pathsToTargets[bestIndex][pathsToTargets[bestIndex].Count - 2];

            }, 2f);

            Scheduler.Schedule(() =>
            {
                targets[bestIndex].TakeDamage(1);

                OnEndTurn?.Invoke();
            }, 3f);
        }

        public override List<PlayerAction> GetAvailableMenuActions()
        {
            return new List<PlayerAction>();
            //throw new System.NotImplementedException();
        }

        public override List<PlayerAction> GetAvailableComboActions(Entity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}

