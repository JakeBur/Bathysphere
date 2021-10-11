using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    /// <summary>
    /// A selectable is any object that can be selected by the player.
    /// </summary>
    public interface ISelectable
    {
        void Select();
        void Deselect();

        /// <summary>
        /// Gets all IBattleActions that this selectable can currently perform.
        /// </summary>
        public List<PlayerAction> GetAvailableMenuActions();

        /// <summary>
        /// Gets all IBattleActions that the current selectable can perform with the given Entity as a target.
        /// </summary>
        /// <param name="entity">The target entity.</param>
        public List<PlayerAction> GetAvailableComboActions(Entity entity);

        bool IsSelectable();
    }

    public static class SelectableExtentions
    {
        /// <summary>
        /// Gets the GameObject this ISelectable is attached to, or null if the ISelectable is not a MonoBehavior.
        /// </summary>
        public static GameObject TryGetGameObject(this ISelectable selectable)
        {
            return (selectable as MonoBehaviour)?.gameObject;
        }
    }

}


