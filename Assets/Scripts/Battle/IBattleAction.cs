using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public interface IBattleAction
    {
        bool CanApplyToSquare(GridSquare gridSquare);
        void Apply(GridSquare gridSquare);
    }
}


