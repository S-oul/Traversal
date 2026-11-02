using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private TerrainGeneration generator; 
        [SerializeField] private List<GroundStack> board;
        [SerializeField] private List<Coordinate> coordinates;
        [SerializeField] private Dictionary<Piece, Coordinate> piecePositions;
        [SerializeField] private Player player1;
        [SerializeField] private Player player2;
        
        private List<Piece> _team1 = new();
        private List<Piece> _team2 = new();
        
        private int _boardSize = 10;

        public GameObject boardObject;
        
        private void Start()
        {
            SetUpBoard();
        }
        public void SetUpBoard()
        {
            board = new List<GroundStack>();
            coordinates = new List<Coordinate>();
            piecePositions = new Dictionary<Piece, Coordinate>();
            
            _team1 = player1.Team; // Print safe
            _team2 = player2.Team; // Print safe
            
            Debug.Log(_team1.Count);
            Debug.Log(_team2.Count);
            
            board.AddRange( boardObject.GetComponentsInChildren<GroundStack>());
            
            // Add coordinates to each cell
            for (int x = 0; x < _boardSize; x++)
            {
                for (int z = 0; z < _boardSize; z++)
                {
                    int index = x * _boardSize + z;
        
                    GroundStack stack = board[index];
                    Coordinate coordinate = new Coordinate(stack, new Vector3(x, stack.Level, z));
                    print(coordinate.position);
                    coordinates.Add(coordinate);
                    
                    // Set up pieces of both teams on the board
                    if(coordinate.position == new Vector3(0, stack.Level, 9)) Debug.Log("-----------------------------" + player1.name + "-----------------------------");
                    
                    if (coordinate.position == new Vector3(9, stack.Level, 2))
                    {
                        SetUpPiecePositions(player1, 0, coordinate, stack);
                        
                    }
                    if (coordinate.position == new Vector3(9, stack.Level, 3))
                    {
                        SetUpPiecePositions(player1, 1, coordinate, stack);
                    }
                    if (coordinate.position == new Vector3(9, stack.Level, 4))
                    {
                        SetUpPiecePositions(player1, 2, coordinate, stack);
                    }
                    if(coordinate.position == new Vector3(9, stack.Level, 5))
                    {
                        SetUpPiecePositions(player1, 3, coordinate, stack);
                    }
                    if(coordinate.position == new Vector3(9, stack.Level, 6))
                    {
                        SetUpPiecePositions(player1, 4, coordinate, stack);

                    }
                    if(coordinate.position == new Vector3(9, stack.Level, 7))
                    {
                        SetUpPiecePositions(player1, 5, coordinate, stack);

                    }
                    
                    if(coordinate.position == new Vector3(0, stack.Level, 0)) Debug.Log("-----------------------------" + player2.name + "-----------------------------");

                    if (coordinate.position == new Vector3(0, stack.Level, 2))
                    {
                        SetUpPiecePositions(player2, 0, coordinate, stack);
                    }
                    if (coordinate.position == new Vector3(0, stack.Level, 3))
                    {
                        SetUpPiecePositions(player2, 1, coordinate, stack);
                        
                    }
                    if (coordinate.position == new Vector3(0, stack.Level, 4))
                    {
                        SetUpPiecePositions(player2, 2, coordinate, stack);
                    }
                    if(coordinate.position == new Vector3(0, stack.Level, 5))
                    {
                        SetUpPiecePositions(player2, 3, coordinate, stack);
                    }
                    if(coordinate.position == new Vector3(0, stack.Level, 6))
                    {
                        SetUpPiecePositions(player2, 4, coordinate, stack);
                    }

                    if (coordinate.position == new Vector3(0, stack.Level, 7))
                    {
                        SetUpPiecePositions(player2, 5, coordinate, stack);
                    }
                }
            }
        }

        private void SetUpPiecePositions(Player player, int pieceNumber, Coordinate coordinate, GroundStack stack)
        {
            Piece piece = player.Team[pieceNumber];
            piecePositions.Add(piece, coordinate);
            Debug.Log(piece.name + ", pos=" + coordinate.position);
            piece.transform.position = ToBoardCoordinate(piece, stack);
        }

        private Vector3 ToBoardCoordinate(Piece piece, GroundStack stack)
        {
            return new Vector3(stack.transform.position.x, stack.Top() + piece.GetMidHeight(), stack.transform.position.z);
        }
    }
    [System.Serializable]
    public struct Coordinate
    {
        private GroundStack _groundStack;
        private Vector3 _position;
        
        public GroundStack groundStack { get => _groundStack; set => _groundStack = value; }
        public Vector3 position { get => _position; set => _position = value; }

        public Coordinate(GroundStack groundStack, Vector3 position)
        {
            _groundStack = groundStack;
            _position = position;
        }
        
    }

    [System.Serializable]
    public struct PiecePosition
    {
        private Piece _piece;
        private Coordinate _coordinateOnBoard;
        
        public Piece piece { get => _piece; set => _piece = value; }

        public Coordinate coordinateOnBoard { get => _coordinateOnBoard; set => _coordinateOnBoard = value; }

        public PiecePosition(Piece piece, Coordinate coordinateOnBoard)
        {
            _piece = piece;
            _coordinateOnBoard = coordinateOnBoard;
        }
    }
}