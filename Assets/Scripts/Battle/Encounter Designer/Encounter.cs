using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    [CreateAssetMenu(fileName = "Encounter", menuName = "Battle/Encounter", order = 0)]
    public class Encounter : ScriptableObject
    {
        public Vector2Int gridSize;
        //public List<EncounterEntityContainer> entities;
        public List<EncounterEntity> entities;
        //public EntityList entities;

        /*public class EntityList : ScriptableObject
        {
            List<EncounterEntity> entities;
        }*/
    }
}