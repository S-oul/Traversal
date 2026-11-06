using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private TerrainGeneration generator; 
        [SerializeField] private GroundStack[,] board;
        [SerializeField] private int[,] boardHeights;
        [SerializeField] private Dictionary<Piece, GroundStack> _piecePositions;
        [SerializeField] private Player player1;
        [SerializeField] private Player player2;
        
        private List<Piece> _team1 = new();
        private List<Piece> _team2 = new();
        
        private int _boardSize = 10;
        public GameObject boardObject;
        
        [SerializeField] private GameObject circlePrefab;
        private List<GameObject> circlePreFabs;
        
        public Dictionary<Piece, GroundStack> piecePositions
        {
            get => _piecePositions;
            set => _piecePositions = value;
        }

        public int[,] BoardHeights { get => boardHeights; set => boardHeights = value; }
        public GroundStack[,] Board { get => board; set => board = value; }

        private void Start()
        {
            GameObject container = new GameObject("CirclesContainer"); 
            circlePreFabs = new List<GameObject>();
            for (int i = 0; i <= 200; i++)
            {
                GameObject circle = Instantiate(circlePrefab, new Vector3(-100, 0, -100), Quaternion.Euler(90,0,0));
                circle.SetActive(false);
                circle.transform.SetParent(container.transform);
                circlePreFabs.Add(circle);
            }
            SetUpBoard();
        }

        public void SetUpBoard()
        {
            boardHeights = new int[_boardSize, _boardSize];
            board = new GroundStack[_boardSize, _boardSize];
            piecePositions = new Dictionary<Piece, GroundStack>();
            
            _team1 = player1.Team;
            _team2 = player2.Team;
            
            foreach (var stack in boardObject.GetComponentsInChildren<GroundStack>())
            {
                int x = stack.GridPos.x;
                int y = stack.GridPos.y;

                if (x >= 0 && x < _boardSize && y >= 0 && y < _boardSize)
                {
                    board[x, y] = stack;
                }
            }

            SetupTeamPieces();
        }

        private void SetupTeamPieces()
        {
            // Player 1 positions
            SetupPieceIfValid(player1, 0, new Vector2Int(9, 2));
            SetupPieceIfValid(player1, 1, new Vector2Int(9, 3));
            SetupPieceIfValid(player1, 2, new Vector2Int(9, 4));
            SetupPieceIfValid(player1, 3, new Vector2Int(5, 5));
            SetupPieceIfValid(player1, 4, new Vector2Int(9, 6));
            SetupPieceIfValid(player1, 5, new Vector2Int(9, 7));

            // Player 2 positions
            SetupPieceIfValid(player2, 0, new Vector2Int(0, 2));
            SetupPieceIfValid(player2, 1, new Vector2Int(0, 3));
            SetupPieceIfValid(player2, 2, new Vector2Int(0, 4));
            SetupPieceIfValid(player2, 3, new Vector2Int(0, 5));
            SetupPieceIfValid(player2, 4, new Vector2Int(0, 6));
            SetupPieceIfValid(player2, 5, new Vector2Int(0, 7));
        }

        private void SetupPieceIfValid(Player player, int pieceIndex, Vector2Int coordinate)
        {
            if (coordinate.x >= 0 && coordinate.x < _boardSize && coordinate.y >= 0 && coordinate.y < _boardSize)
            {
                GroundStack stack = board[coordinate.x, coordinate.y];
                if (stack != null && player.Team.Count > pieceIndex)
                {
                    Piece piece = player.Team[pieceIndex];
                    piece.Player = player;
                    piecePositions[piece] = stack;
                    piece.Stack = stack;
                    _piecePositions.TryAdd(piece,stack);
                    piece.transform.position = ToBoardCoordinatePiece(piece, stack);
                }
            }
        }

        // private void SetUpPiecePositions(Player player, int pieceNumber, Vector2Int coordinate, GroundStack stack)
        // {
        //     Piece piece = player.Team[pieceNumber];
        //     piecePositions[piece] = stack;
        //     piece.transform.position = ToBoardCoordinatePiece(piece, stack);
        // }

        public Vector3 ToBoardCoordinatePiece(Piece piece, GroundStack stack)
        {
            return new Vector3(stack.transform.position.x, stack.Top() + piece.GetMidHeight(), stack.transform.position.z);
        }
        
        public Vector3 ToBoardCoordinate(GroundStack stack)
        {
            return new Vector3(stack.transform.position.x, stack.Top(), stack.transform.position.z);
        }

        public void HideCircles()
        {
            foreach(GameObject circle in circlePreFabs)
            {
                circle.gameObject.SetActive(false);
            }
        }

        private static readonly (int dx, int dy)[] Directions = 
        {
            (0, 1), (0, -1), (-1, 0), (1, 0)
        };

        public List<GroundStack> ManhattanDistanceWithHeight(Piece piece)
        {
            GroundStack startPoint = piece.Stack;
            int maxMovement = piece.MovementCount;

            List<GroundStack> result = new();

            int[,] remainingAt = new int[_boardSize, _boardSize];
            for (int x = 0; x < _boardSize; x++)
            {
                for (int y = 0; y < _boardSize; y++)
                {
                    remainingAt[x, y] = -1; // -1 = unvisited
                }
            }

            var toSearch = new Queue<(GroundStack stack, int budget)>();

            int startX = startPoint.GridPos.x;
            int startY = startPoint.GridPos.y;

            remainingAt[startX, startY] = maxMovement;
            toSearch.Enqueue((startPoint, maxMovement));
            //result.Add(startPoint);

            while (toSearch.Count > 0)
            {
                var current = toSearch.Dequeue();
                var currentCoord = current.stack;
                int currentBudget = current.budget;

                if (currentBudget < remainingAt[currentCoord.GridPos.x, currentCoord.GridPos.y])
                    continue;

                int currentX = currentCoord.GridPos.x;
                int currentY = currentCoord.GridPos.y;
                int currentElevation = boardHeights[currentX, currentY];

                foreach (var (dx, dy) in Directions)
                {
                    int x2 = currentX + dx;
                    int y2 = currentY + dy;
                    
                    bool doSkip = false;
                    foreach(var kvp in _piecePositions)
                    {
                        if(!doSkip && kvp.Value.GridPos == new Vector2Int(x2,y2))
                        doSkip = true;
                    }
                    if(doSkip) continue;

                    if (x2 < 0 || x2 >= _boardSize || y2 < 0 || y2 >= _boardSize)
                        continue;

                    int nextElevation = boardHeights[x2, y2];
                    int heightDiff = Mathf.Abs(nextElevation - currentElevation);

                    int totalStepCost = 1 + heightDiff; 
                    int nextBudget = currentBudget - totalStepCost;

                    if (nextBudget < 0)
                        continue;

                    GroundStack nextStack = board[x2, y2];
                    if (nextStack == null) continue;

                    if (nextBudget > remainingAt[x2, y2])
                    {
                        if (remainingAt[x2, y2] == -1)
                        {
                            result.Add(nextStack);
                        }

                        remainingAt[x2, y2] = nextBudget;
                        toSearch.Enqueue((nextStack, nextBudget));
                    }
                }
            }
            
            HideCircles();

            //display circles plz
            int index = 0;
            foreach(var i in result)
            {
                if (index >= circlePreFabs.Count) break;

                var circle = circlePreFabs[index];
                circle.transform.position = ToBoardCoordinate(i) + Vector3.up * .11f;
                circle.gameObject.SetActive(true);
                circle.GetComponent<SpriteRenderer>().color = piece.Player == player1? Color.blue: Color.red;
                index++;
            } 

            return result;
        }
    }
}