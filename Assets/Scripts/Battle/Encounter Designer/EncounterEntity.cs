using UnityEngine;
using UnityEditor;
using System;

namespace Battle
{
    [CreateAssetMenu(fileName = "EncounterEntity", menuName = "Battle/EncounterEntity", order = 0)]
    [Serializable]
    public class EncounterEntity : ScriptableObject
    {
        public Action OnContentsUpdated;

        public EntityData EntityData
        {
            get
            {
                return _entityData;
            }

            set
            {
                _entityData = value;
                EditorUtility.SetDirty(this);
                OnContentsUpdated?.Invoke();
            }
        }

        public Vector2Int Position
        {
            get
            {
                return _position;
            }

            set
            {
                _position = value;
                EditorUtility.SetDirty(this);
                OnContentsUpdated?.Invoke();
            }
        }

        [SerializeField]
        private Vector2Int _position;
        [SerializeField]
        private EntityData _entityData;
    }
}