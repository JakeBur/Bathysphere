using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HoverIndicator : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            InputManager.Instance.OnHoverGridSquare += UpdateHoverTarget;
        }

        private void UpdateHoverTarget(GridSquare gridSquare)
        {
            transform.position = gridSquare.transform.position;
        }
    }
}
