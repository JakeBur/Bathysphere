using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class AoEAction : IBattleAction
    {
        public abstract void Apply(GridSquare gridSquare);
        public abstract bool CanApplyToSquare(GridSquare gridSquare);
        public abstract void BeginPreview();
        public abstract void UpdatePreview(GridSquare gridSquare);
        public abstract void EndPreview();
        public abstract List<GridSquare> FindThreatenedSquares();
        public abstract bool CanTargetSquare(GridSquare gridSquare);
    }
}