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

        GameObject GetGameObject();

        bool IsSelectable();
    }
}


