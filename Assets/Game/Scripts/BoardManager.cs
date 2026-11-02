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
            board = new List<GroundStack>();
            coordinates = new List<Coordinate>();
            piecePositions = new List<PiecePosition>();
            
            _boardSize = generator.gridSize;
            
            _team1 = player1.Team; // Print safe
            _team2 = player2.Team; // Print safe
            
            Debug.Log(_team1.Count);
            Debug.Log(_team2.Count);
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
                    
                    if(coordinate.position == new Vector3(0, stack.Level, 9)) Debug.Log("-----------------------------" + player1.name + "-----------------------------");
                    
                    if (coordinate.position == new Vector3(9, stack.Level, 2))
                    {
                        PiecePosition p = new PiecePosition(player1.Team[0], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if (coordinate.position == new Vector3(9, stack.Level, 3))
                    {
                        PiecePosition p = new PiecePosition(player1.Team[1], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if (coordinate.position == new Vector3(9, stack.Level, 4))
                    {
                        PiecePosition p = new PiecePosition(player1.Team[2], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if(coordinate.position == new Vector3(9, stack.Level, 5))
                    {
                        PiecePosition p = new PiecePosition(player1.Team[3], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if(coordinate.position == new Vector3(9, stack.Level, 6))
                    {
                        PiecePosition p = new PiecePosition(player1.Team[4], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if(coordinate.position == new Vector3(9, stack.Level, 7))
                    {
                        PiecePosition p = new PiecePosition(player1.Team[5], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    
                    if(coordinate.position == new Vector3(0, stack.Level, 0)) Debug.Log("-----------------------------" + player2.name + "-----------------------------");

                    if (coordinate.position == new Vector3(0, stack.Level, 2))
                    {
                        PiecePosition p = new PiecePosition(player2.Team[0], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if (coordinate.position == new Vector3(0, stack.Level, 3))
                    {
                        PiecePosition p = new PiecePosition(player2.Team[1], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if (coordinate.position == new Vector3(0, stack.Level, 4))
                    {
                        PiecePosition p = new PiecePosition(player2.Team[2], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if(coordinate.position == new Vector3(0, stack.Level, 5))
                    {
                        PiecePosition p = new PiecePosition(player2.Team[3], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if(coordinate.position == new Vector3(0, stack.Level, 6))
                    {
                        PiecePosition p = new PiecePosition(player2.Team[4], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                    if(coordinate.position == new Vector3(0, stack.Level, 7))
                    {
                        PiecePosition p = new PiecePosition(player2.Team[5], coordinate);
                        piecePositions.Add(p);
                        Debug.Log(p.piece.name + ", pos=" + p.coordinateOnBoard.position);
                    }
                }
            }
            
            // Set up pieces of both team on the board
            SetUpPiecePosition();

            SetUpPiecePosition();
        }

        private void SetUpPiecePosition()
        {
           
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