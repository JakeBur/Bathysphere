using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public abstract class PlayerAction : IBattleAction
    {
        public abstract void BeginPreview();
        public abstract void UpdatePreview(GridSquare gridSquare);
        public abstract void EndPreview();
        public abstract bool CanApplyToSquare(GridSquare gridSquare);
        public abstract void Apply(GridSquare gridSquare);
        public abstract List<GridSquare> FindThreatenedSquares();
    }
}