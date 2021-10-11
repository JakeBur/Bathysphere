using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Highlighter : MonoBehaviour
    {
        public static Highlighter Instance;

        public HighlightSet moveHighlights;
        public HighlightSet pathHighlights;
        public HighlightSet playerAttackHighlights;
        public HighlightSet playerAttackGreyoutHighlights;

        private void Awake()
        {
            Instance = this;
        }

        [System.Serializable]
        public class HighlightSet
        {
            [SerializeField]
            private GameObject _highlightPrefab;
            private List<GameObject> _highlights;

            public void Highlight(List<GridSquare> gridSquares)
            {
                if(_highlights == null) _highlights = new List<GameObject>();

                foreach (GridSquare gridSquare in gridSquares)
                {
                    GameObject highlight = Instantiate(_highlightPrefab, Instance.transform);
                    highlight.transform.position = gridSquare.transform.position;
                    _highlights.Add(highlight);
                }
            }

            public void Clear()
            {
                if (_highlights == null) return;

                foreach(GameObject highlight in _highlights)
                {
                    Destroy(highlight);
                }

                _highlights.Clear();
            }
        }
    }
}