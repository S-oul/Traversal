using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        #region variables
        [SerializeField] private TerrainGeneration generator; 
        [SerializeField] private GroundStack[,] board;
        [SerializeField] private int[,] boardHeights;
        [SerializeField] private Dictionary<Piece, GroundStack> _pieceGroundStack;
        [SerializeField] private Dictionary<Vector2Int, Piece> _positionPiece;
        [SerializeField] private Dictionary<GameObject, GroundStack> _circleAndStack;
        [SerializeField] private Player player1;
        [SerializeField] private Player player2;
        
        [SerializeField] string FEN_Notation;
        
        private readonly Graph _graph = new();
        private List<Piece> _team1 = new();
        private List<Piece> _team2 = new();
        
        private int _boardSize = 10;
        public GameObject boardObject;
        
        [SerializeField] private GameObject circlePrefab;
        private List<GameObject> circlePreFabs;
        
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();
        
        #endregion
        #region getters
        public Dictionary<Piece, GroundStack> PieceGroundStack { get => _pieceGroundStack; set => _pieceGroundStack = value; }
        public int[,] BoardHeights { get => boardHeights; set => boardHeights = value; }
        public GroundStack[,] Board { get => board; set => board = value; }
        public Dictionary<GameObject, GroundStack> circleAndStack => _circleAndStack;

        public Dictionary<Vector2Int, Piece> positionPiece { get => _positionPiece; set => _positionPiece = value; }

        #endregion

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
            // SetUpBoard();
        }

        #region board-setup
        public void SetUpBoard()
        {
            _circleAndStack = new();
            FEN_Notation = "";
            boardHeights = new int[_boardSize, _boardSize];
            board = new GroundStack[_boardSize, _boardSize];
            _pieceGroundStack = new Dictionary<Piece, GroundStack>();
            positionPiece = new Dictionary<Vector2Int, Piece>(); 
            _team1 = player1.Team;
            _team2 = player2.Team;
            
            foreach (var stack in boardObject.GetComponentsInChildren<GroundStack>())
            {
                int x = stack.GridPos.x;
                int y = stack.GridPos.y;

                if (x >= 0 && x < _boardSize && y >= 0 && y < _boardSize)
                {
                    board[x, y] = stack;
                    _graph.AddVertex(new Vector2Int(x, y));
                }
            }
            // RandomSetupTeamPieces();
            SetupTeamPieces();
            AddGraphEdges();
            print(CreateFenNotation());
        }

        private string CreateFenNotation()
        {
            StringBuilder fen = new();

            Vector2Int[] positions = new Vector2Int[12];

            int index = 0;
            foreach(var g in _pieceGroundStack.Values)
            {
                positions[index++] = g.GridPos;
            }


            for (int y = 9; y >= 0; y--)
            {
                int emptyCount = 0;

                for (int x = 0; x < 10; x++)
                {
                    if (positions.Contains(new Vector2Int(x, y)))
                    {
                        if (emptyCount > 0)
                        {
                            fen.Append(emptyCount);
                            emptyCount = 0;
                        }
                        
                        Piece piece;
                        positionPiece.TryGetValue(new Vector2Int(x, y), out piece);
                        char notation = piece.AlgebraicNotation();
                        notation = piece.Player == player1? char.ToUpper(notation) : char.ToLower(notation);
                        fen.Append(notation);
                    }
                    else
                    {
                        emptyCount++;
                    }
                }

                if (emptyCount > 0)
                    fen.Append(emptyCount);

                if (y > 0)
                    fen.Append('/');
            }
            FEN_Notation = fen.ToString();
            return fen.ToString();
        }

        private void SetupTeamPieces()
        {
            // Player 1 positions
            SetupPieceIfValid(player1, 0, new Vector2Int(9, 2));
            SetupPieceIfValid(player1, 1, new Vector2Int(9, 3));
            SetupPieceIfValid(player1, 2, new Vector2Int(9, 4));
            SetupPieceIfValid(player1, 3, new Vector2Int(1, 4));
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

        private void RandomSetupTeamPieces()
        {
            usedPositions = new HashSet<Vector2Int>();
            SetupRandomTeam(player1);
            SetupRandomTeam(player2);
        }

        private void SetupRandomTeam(Player player)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2Int randomPos;
                do
                {
                    randomPos = new Vector2Int(
                        Random.Range(2, 8),
                        Random.Range(2, 8)
                    );
                }
                while (!usedPositions.Add(randomPos));
                SetupPieceIfValid(player, i, randomPos);
            }
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
                    piece.Stack = stack;
                    _pieceGroundStack.TryAdd(piece,stack);
                    positionPiece.TryAdd(coordinate, piece);
                    piece.transform.position = ToBoardCoordinatePiece(piece, stack);
                }
            }
        }
        
        public Vector3 ToBoardCoordinatePiece(Piece piece, GroundStack stack)
        {
            return new Vector3(stack.transform.position.x, stack.Top() + piece.GetMidHeight(), stack.transform.position.z);
        }
        
        public Vector3 ToBoardCoordinate(GroundStack stack)
        {
            return new Vector3(stack.transform.position.x, stack.Top() + .25f, stack.transform.position.z);
        }
        #endregion
        #region piece-movement
        public void HandleMovement(Vector2Int targetPosition)
        {
            
        }
        
        public void HideCircles()
        {
            foreach(GameObject circle in circlePreFabs)
            {
                circle.gameObject.SetActive(false);
            }
            circleAndStack.Clear();
        }

        public void UpdateGameState(Piece piece, GroundStack newStack)
        {
            if (_pieceGroundStack.TryGetValue(piece, out GroundStack oldStack))
            {
                _positionPiece.Remove(oldStack.GridPos);
            }

            _pieceGroundStack[piece] = newStack;
            _positionPiece[newStack.GridPos] = piece;
            piece.Stack = newStack;
            piece.transform.position = ToBoardCoordinatePiece(piece, newStack);
        }

        private static readonly (int dx, int dy)[] Directions = 
        {
            (0, 1), (0, -1), (-1, 0), (1, 0),
            //(1, 1), (-1, 1), (-1, -1),(1, -1), 
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
                    
                    //need to add Win Board Conditions
                    if (x2 < 0 || x2 >= _boardSize || y2 < 0 || y2 >= _boardSize)
                        continue;

                    bool doSkip = false;
                    bool enqueueToSearch = true;
                    foreach(var kvp in _pieceGroundStack)
                    {
                        Piece otherPiece = kvp.Key;
                        if(otherPiece == piece) continue;
                        if(!doSkip && kvp.Value.GridPos == new Vector2Int(x2, y2))
                        {
                            if
                            (
                                otherPiece.Player != piece.Player 
                                && otherPiece.Attack <= piece.Attack
                            )
                            {
                                //can attack Case Valid but do not search for case after;
                                enqueueToSearch = false;
                            }
                            else 
                            { 
                                doSkip = true;
                            }
                        }
                    }
                    if(doSkip) continue;


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
                        if(enqueueToSearch) toSearch.Enqueue((nextStack, nextBudget));
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
                circleAndStack.Add(circle, i);
                index++;
            } 

            return result;
        }

        public void PlaceCircles(Piece piece)
        {
            Vector2Int sourceCoordinate = piece.Stack.GridPos;
            Dictionary<Vector2Int, int> distances = GetDistances(piece);
            int index = 0;
            foreach (KeyValuePair<Vector2Int, int> distance in distances)
            {
                Vector2Int possiblePosition = distance.Key;
                int d = distance.Value;

                // If distance is too much for piece, don't show a circle
                if (d > piece.MovementCount) continue;
                
                // If it is the source, don't show
                if (possiblePosition.x == Mathf.RoundToInt(sourceCoordinate.x) && possiblePosition.y == Mathf.RoundToInt(sourceCoordinate.y)) continue;
                
                // If it is occupied, don't show
                if (IsOccupied(possiblePosition)) continue;
                
                GroundStack stack = GetStack(possiblePosition);

                if (index >= circlePreFabs.Count) break;
                
                var circle = circlePreFabs[index];
                circlePreFabs[index].transform.position = ToBoardCoordinate(stack);
                circlePreFabs[index].gameObject.SetActive(true);
                circleAndStack.Add(circle, stack);
                index++;
            }
            
            // foreach (var entry in distances)
            // {
            //     if (entry.Value == int.MaxValue)
            //         continue;
            //
            //     Debug.Log(
            //         $"Position: {entry.Key} | " +
            //         $"Total cost: {entry.Value} | " +
            //         $"MovementCount: {piece.MovementCount}"
            //     );
            // }
        }
        #endregion
        
        #region GraphBuild
        private void AddGraphEdges()
        {
            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };
            
            // If you combine directions with the explorator, cost of the edge is reduced
            // Same if you make two steps forward regardless height/direction
            
            foreach (Vector2Int vertex in _graph.Vertices)
            {
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = vertex + direction;

                    if (!_graph.ContainsVertex(neighbor)) continue;

                    GroundStack from = GetStack(vertex);
                    GroundStack to = GetStack(neighbor);

                    int cost = GetMovementCost(from, to);
                    if (!IsOccupied(neighbor)) _graph.AddEdge(vertex, neighbor, cost);
                }
            }
        }
        #endregion
        #region Dijsktra
        
        private bool IsOccupied(Vector2Int targetPosition)
        {
            return positionPiece.ContainsKey(targetPosition);
        }
        
        private GroundStack GetStack(Vector2Int position)
        {
            return board[position.x, position.y];
        }
        
        private int GetMovementCost(GroundStack from, GroundStack to)
        {
            int heightDifference = Mathf.Abs(to.Level - from.Level);
            return 1 + heightDifference;
        }
        
        /// <summary>
        /// C'est très compliqué, sujet de doctorat possible
        /// </summary>
        /// <param name="adjacency"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private Dictionary<Vector2Int, int> Dijkstra(Dictionary<Vector2Int, List<Edge>> adjacency, Vector2Int source)
        {
            int vertexCount = adjacency.Count;

            Dictionary<Vector2Int, int> distances = new(vertexCount);
            HashSet<Vector2Int> visited = new(vertexCount);

            foreach (Vector2Int vertex in adjacency.Keys) distances[vertex] = int.MaxValue;
            if (!adjacency.ContainsKey(source)) return distances;
            
            distances[source] = 0;

            while (true)
            {
                Vector2Int? current = null;
                int minDistance = int.MaxValue;

                foreach (Vector2Int vertex in adjacency.Keys)
                {
                    if (visited.Contains(vertex)) continue;
                    if (distances[vertex] < minDistance)
                    {
                        minDistance = distances[vertex];
                        current = vertex;
                    }
                }

                if (current == null) break;
                Vector2Int u = current.Value;
                int d = distances[u];

                foreach (Edge edge in adjacency[u])
                {
                    Vector2Int v = edge.To;
                    int cost = edge.Cost;

                    if (visited.Contains(v)) continue;
                    int newDistance = d + cost;
                    if (newDistance < distances[v]) distances[v] = newDistance;
                    
                }
                visited.Add(u);
            }
            return distances;
        }
        
        private Dictionary<Vector2Int, int> GetDistances(Piece piece)
        {
            GroundStack stack = _pieceGroundStack[piece];
            Dictionary<Vector2Int, int> distances = Dijkstra(_graph.adjacency, stack.GridPos);
            return distances;
        }
        #endregion
    }
    
    
}
