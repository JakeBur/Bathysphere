using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Battle
{
    public class BattleGridManager : MonoBehaviour
    {
        public static BattleGridManager Instance;

        public PriorityAction<GridSquare> OnSquareClicked;
        public Action<GridSquare> OnSquareHovered;

        public GameObject squarePrefab;

        public List<Entity> entities;

        [HideInInspector]
        public BattleGrid grid;

        private void Awake()
        {
            Instance = this;
            OnSquareClicked = new PriorityAction<GridSquare>();

            grid = new BattleGrid(5, 5);

            for(int x = 0; x < grid.squares.GetLength(0); x++)
            {
                for (int y = 0; y < grid.squares.GetLength(0); y++)
                {
                    GameObject square = Instantiate(squarePrefab, transform);
                    square.transform.localPosition = new Vector3(x, 0, y);

                    GridSquare gridSquare = square.GetComponent<GridSquare>();

                    grid[x, y] = square.GetComponent<GridSquare>();

                    gridSquare.OnClick += HandleClick;
                }
            }

            entities[0].Square = grid.squares[0, 0];
            entities[1].Square = grid.squares[4, 4];
            entities[2].Square = grid.squares[1, 4];
        }

        private void HandleClick(GridSquare gridSquare)
        {
            OnSquareClicked?.Invoke(gridSquare);
        }

    }
}
