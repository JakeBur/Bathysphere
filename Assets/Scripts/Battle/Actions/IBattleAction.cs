using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    /// <summary>
    /// Interface for any object that can, under certain conditions, apply effects to a given GridSquare.
    /// Use this to implement attacks and abilities that target a specific square.
    /// </summary>
    public interface IBattleAction
    {
        bool CanApplyToSquare(GridSquare gridSquare);
        void Apply(GridSquare gridSquare);
    }
}


