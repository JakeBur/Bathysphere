using UnityEngine;
using UnityEditor;
using System;

namespace Battle
{
    /// <summary>
    /// Wrapper for EntityData that contains information about an Entity's initial position on the battlegrid.
    /// </summary>
    [CreateAssetMenu(fileName = "EncounterEntity", menuName = "Battle/EncounterEntity", order = 0)]
    [Serializable]
    public class EncounterEntity : ScriptableObject
    {
        /// <summary>
        /// Called whenever any data on this EncounterEntity is modified.
        /// </summary>
        public Action OnContentsUpdated;

        /// <summary>
        /// The EntityData that should be used to instantiate the actual Entity in the Encounter.
        /// </summary>
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

        /// <summary>
        /// The position that the Entity should spawn at.
        /// </summary>
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