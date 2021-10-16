using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Battle
{
    /// <summary>
    /// An Encounter holds all necesarry data to set up the initial state of a battle.
    /// </summary>
    [CreateAssetMenu(fileName = "Encounter", menuName = "Battle/Encounter", order = 0)]
    public class Encounter : ScriptableObject
    {
        /// <summary>
        /// Called whenever any data in this Encounter is modified.
        /// </summary>
        public Action OnContentsUpdated;

        /// <summary>
        /// Size of this grid in squares. Format is (x, y).
        /// </summary>
        public Vector2Int gridSize;

        /// <summary>
        /// List of EncounterEntities that exist in this Encounter.
        /// </summary>
        public List<EncounterEntity> Entities
        {
            get
            {
                if (_entities == null) _entities = new List<EncounterEntity>();
                return _entities;
            }
        }

        /// <summary>
        /// List of EncounterEntities that exist in this Encounter.
        /// </summary>
        [SerializeField]
        private List<EncounterEntity> _entities;

        /// <summary>
        /// Adds a new EncounterEntity to this Encounter based on the given EntityData.
        /// </summary>
        /// <param name="entityData">The EntityData that describes the Entity to be created.</param>
        /// <param name="position">The position to create the EncounterEntity at.</param>
        /// <returns>The created EncounterEntity.</returns>
        public EncounterEntity AddEntity(EntityData entityData, Vector2Int? position = null)
        {
            string encounterPath = MovePathUpOneLevel(AssetDatabase.GetAssetPath(this));
            string folderPath = encounterPath + "/" + name + " Entities";

            // create folder if necessary
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(encounterPath, name + " Entities");
            }

            // create new EncounterEntity container for entityData
            EncounterEntity createdEncounterEntity = ScriptableObject.CreateInstance(typeof(EncounterEntity)) as EncounterEntity;
            createdEncounterEntity.EntityData = entityData;
            createdEncounterEntity.Position = position != null ? (Vector2Int)position : new Vector2Int(-1, -1);

            // add that created EncounterEntity to the asset database
            string assetPath = folderPath + "/Encounter" + entityData.name;
            string postfix = "";

            int postfixNumber = 0;
            while (AssetDatabase.LoadAssetAtPath(assetPath + postfix + ".asset", typeof(EncounterEntity)) != null)
            {
                postfix = " " + postfixNumber.ToString();
                postfixNumber++;
            }

            AssetDatabase.CreateAsset(createdEncounterEntity, assetPath + postfix + ".asset");

            // finally, add it to this encounter
            AddEntity(createdEncounterEntity);

            return createdEncounterEntity;
        }

        /// <summary>
        /// Deletes this Encounter and all EncounterEntities associated with it.
        /// </summary>
        public void Delete()
        {
            string folderPath = GenerateEntityFolderPath();

            FileUtil.DeleteFileOrDirectory(folderPath);

            AssetDatabase.DeleteAsset(folderPath + ".meta");
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
        }

        

        public void RemoveEntity(EncounterEntity entity)
        {
            if(Entities.Contains(entity))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(entity));
                Entities.Remove(entity);
            }
            
            EditorUtility.SetDirty(this);
            OnContentsUpdated?.Invoke();
        }

        /// <summary>
        /// Adds an EncounterEntity to this Encounter.
        /// </summary>
        /// <param name="encounterEntity">The EncounterEntity to be added.</param>
        private void AddEntity(EncounterEntity encounterEntity)
        {
            Entities.Add(encounterEntity);
            EditorUtility.SetDirty(this);
            OnContentsUpdated?.Invoke();
        }

        /// <summary>
        /// Helper function to remove the final element of a filepath.
        /// </summary>
        /// <param name="path">The path to truncate.</param>
        /// <returns>The truncated path.</returns>
        private static string MovePathUpOneLevel(string path)
        {
            string[] elements = path.Split('/');

            string finalPath = "";

            for (int i = 0; i < elements.Length - 1; i++)
            {
                finalPath += elements[i] + (i == elements.Length - 2 ? "" : "/");
            }

            return finalPath;
        }

        /// <summary>
        /// Generates a folderpath where the EncounterEntities associated with this Encounter should reside.
        /// </summary>
        /// <returns>The generated filepath.</returns>
        private string GenerateEntityFolderPath()
        {
            string encounterPath = AssetDatabase.GetAssetPath(this);

            string[] pathElements = encounterPath.Split('/');

            string folderPath = pathElements[0];

            for (int i = 1; i < pathElements.Length - 1; i++)
            {
                folderPath += $"/{pathElements[i]}";
            }

            folderPath += $"/{pathElements[pathElements.Length - 1].Split('.')[0]} Entities";

            return folderPath;
        }

        /// <summary>
        /// Renames this Encounter in the filesystem, along with its associated folder.
        /// </summary>
        /// <param name="name">The new name.</param>
        public void Rename(string name)
        {
            string oldFolderPath = GenerateEntityFolderPath();

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), name);
            
            AssetDatabase.MoveAsset(oldFolderPath, GenerateEntityFolderPath());
        }
    }
}