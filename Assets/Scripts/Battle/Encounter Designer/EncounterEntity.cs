using UnityEngine;
using System;

namespace Battle
{
    /*[Serializable]
    public class EncounterEntityContainer
    {
        public EncounterEntity encounterEntity;
    }*/


    [CreateAssetMenu(fileName = "EncounterEntity", menuName = "Battle/EncounterEntity", order = 0)]
    [Serializable]
    public class EncounterEntity : ScriptableObject
    {
        public EntityData entityData;
        public Vector2Int position;
    }
}