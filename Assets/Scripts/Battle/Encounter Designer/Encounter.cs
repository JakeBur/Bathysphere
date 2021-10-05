using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    /*[Serializable]
    public class EncounterEntityContainer
    {
        public EncounterEntity encounterEntity;
    }*/


    [Serializable]
    public class EncounterEntity
    {
        public EntityData entityData;
        public Vector2Int position;
    }
}