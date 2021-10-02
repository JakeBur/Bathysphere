using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public interface ISelectable
    {
        void Select();
        void Deselect();

        bool IsSelectable();
    }

    public static class SelectableExtentions
    {
        public static GameObject GetGameObject(this ISelectable selectable)
        {
            return (selectable as MonoBehaviour)?.gameObject;
        }
    }

}


