using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    [Serializable]
    [CreateAssetMenu(fileName = "EntityData", menuName = "Battle/EntityData", order = 0)]
    public class EntityData : ScriptableObject
    {
        public GameObject prefab;
        public Texture icon;
    }
}