using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private TerrainGeneration generator;
        [SerializeField] private List<GroundStack> board;
        [SerializeField] private List<Coordinate> coordinates;
        private int _boardSize;
        private void Start()
        {
            _boardSize = generator.gridSize;
            board = new List<GroundStack>();
            board.AddRange(GetComponentsInChildren<GroundStack>());
            
            
            foreach(GroundStack obj in board)
            {
                print(obj.name);
            }

            for (int x = 0; x < _boardSize; x++)
            {
                for(int z = 0; z < _boardSize; z++)
                {
                    Coordinate coordinate = new Coordinate(board[x], new Vector3(x, board[x].Level, z));
                    coordinates.Add(coordinate);
                    print(board[x].name + "'s position" + coordinate.Position);
                }
            }
        }
    }
    [System.Serializable]
    public struct Coordinate
    {
        private GroundStack _groundStack;
        private Vector3 _position;
        
        public GroundStack GroundStack { get => _groundStack; set => _groundStack = value; }
        public Vector3 Position { get => _position; set => _position = value; }

        public Coordinate(GroundStack groundStack, Vector3 position)
        {
            _groundStack = groundStack;
            _position = position;
        }
    }
}