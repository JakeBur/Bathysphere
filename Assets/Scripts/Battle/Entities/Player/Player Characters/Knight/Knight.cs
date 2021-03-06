using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Battle
{
    public partial class Knight : PlayerCharacter
    {
        public GameObject knightSwordPrefab;

        protected KnightSword _knightSword;
        
        protected override void InitializeMenuActions()
        {
            menuActions.Clear();

            // if the sword is not out on the field
            if(_knightSword == null)
            {
                menuActions.Add(new ThrowSword(this, 2, 2, knightSwordPrefab));
                menuActions.Add(new Slash(this, 2, 2));
                menuActions.Add(new Protect(this, 1, 3));
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
            knightSwordActions.Add(new Depressurize(this, 1, 5));
            knightSwordActions.Add(new Vault(this, 1, 3));

            comboActions.Add(typeof(KnightSword), knightSwordActions);

            List<PlayerAction> enemyActions = new List<PlayerAction>();
        }
    }
}