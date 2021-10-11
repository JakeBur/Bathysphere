using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public partial class Knight : PlayerCharacter
    {
        protected KnightSword _knightSword;
        
        protected override void InitializeMenuActions()
        {
            menuActions.Clear();

            if(_knightSword == null)
            {
                menuActions.Add(new ThrowSword(this, 2, 2));
                menuActions.Add(new Slash(this, 2, 2));
            }
            else
            {
                menuActions.Add(new Stagger(this, 2, 2));
            }
        }

        protected override void InitializeComboActions()
        {
            comboActions.Clear();

            List<PlayerAction> knightSwordActions = new List<PlayerAction>();
            knightSwordActions.Add(new RecoverSword(this, 1));

            comboActions.Add(typeof(KnightSword), knightSwordActions);
        }
    }
}