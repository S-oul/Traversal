using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private TerrainGeneration generator; 
        [SerializeField] private List<GroundStack> board;
        [SerializeField] private List<Coordinate> coordinates;
        [SerializeField] private List<PiecePosition> piecePositions;

        [SerializeField] private Player player1;
        [SerializeField] private Player player2;
        
        private List<Piece> _team1 = new();
        private List<Piece> _team2 = new();
        
        private int _boardSize;
        private void Start()
        {
            _boardSize = generator.gridSize;
            board = new List<GroundStack>();
            
            _team1 = player1.Team; // Print safe
            _team2 = player2.Team; // Print safe
            
            board.AddRange(GetComponentsInChildren<GroundStack>());
            
            // Add coordinates to each cell
            for (int x = 0; x < _boardSize; x++)
            {
                for (int z = 0; z < _boardSize; z++)
                {
                    int index = x * _boardSize + z;

                    GroundStack stack = board[index];
                    Coordinate coordinate = new Coordinate(stack, new Vector3(x, stack.Level, z));
                    coordinates.Add(coordinate);
                    Debug.Log(stack.name + "'s position = " + coordinate.Position);
                }
            }
            
            SetUpPiecePositions();
        }
        
        private void SetUpPiecePositions()
        {
            piecePositions = new List<PiecePosition>();
            for (int x = 0; x < _boardSize; x++)
            {
                for (int z = 0; z < _boardSize; z++)
                {
                    // PiecePosition piecePosition = new PiecePosition(piece, position);
                    // piecePositions.Add(piecePosition);
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

    [System.Serializable]
    public struct PiecePosition
    {
        Piece _piece;
        Vector3 _position;
        
        public Piece piece { get => _piece; set => _piece = value; }

        public Vector3 position
        {
            get => _position;
            set => _position = value;
        }

        public PiecePosition(Piece piece, Vector3 position)
        {
            _piece = piece;
            _position = position;
        }
    }
}