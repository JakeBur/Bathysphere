using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Battle
{
    [CreateAssetMenu(fileName = "Encounter", menuName = "Battle/Encounter", order = 0)]
    public class Encounter : ScriptableObject
    {
        public Action OnContentsUpdated;

        public Vector2Int gridSize;
        public List<EncounterEntity> entities;

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

        public void Delete()
        {
            /*foreach(EncounterEntity entity in entities)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(entity));
            }*/
            string encounterPath = AssetDatabase.GetAssetPath(this);

            string[] pathElements = encounterPath.Split('/');

            string folderPath = pathElements[0];

            for(int i = 1; i < pathElements.Length - 1; i++)
            {
                folderPath += $"/{pathElements[i]}";
            }

            folderPath += $"/{pathElements[pathElements.Length - 1].Split('.')[0]} Entities";

            FileUtil.DeleteFileOrDirectory(folderPath);

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
        }

        public void AddEntity(EncounterEntity encounterEntity)
        {
            entities.Add(encounterEntity);
            EditorUtility.SetDirty(this);
            OnContentsUpdated?.Invoke();
        }

        public void RemoveEntity(EncounterEntity entity)
        {
            if(entities.Contains(entity))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(entity));
                entities.Remove(entity);
            }
            
            EditorUtility.SetDirty(this);
            OnContentsUpdated?.Invoke();
        }

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
    }
}