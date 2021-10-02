using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class SelectionHighlighter : MonoBehaviour
    {
        public GameObject icon;

        private Transform _followTarget;

        private void Start()
        {
            SelectionManager.Instance.OnSelect += HandleSelect;
        }

        private void Update()
        {
            if(_followTarget)
            {
                transform.position = _followTarget.transform.position;
            }
        }

        private void HandleSelect(ISelectable selectable)
        {
            _followTarget = selectable.GetGameObject().transform;
            icon.SetActive(true);
        }
    }
}


