using System;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Scripts
{
    public class BoardManager : MonoBehaviour
    {
        [SerializeField] private TerrainGeneration generator; 
        [SerializeField] private List<GroundStack> board;
        [SerializeField] private List<Coordinate> _coordinates;
        [SerializeField] private Dictionary<Piece, Coordinate> _piecePositions;
        [SerializeField] private Player player1;
        [SerializeField] private Player player2;
        [SerializeField] private List<Vector2Int> occupiedPositions;

        private Graph _graph = new();
        
        private List<Piece> _team1;
        private List<Piece> _team2;
        
        private int _boardSize = 10;
        public GameObject boardObject;
        
        [SerializeField] private GameObject circlePrefab;
        private List<GameObject> circlePreFabs;
        
        public Dictionary<Piece, Coordinate> piecePositions { get => _piecePositions; set => _piecePositions = value; }
        public List<Coordinate> coordinates { get => _coordinates; set => _coordinates = value; }

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
            
            occupiedPositions = new List<Vector2Int>(piecePositions.Count);
            
            foreach (var piecePosition in piecePositions)
            {
                occupiedPositions.Add
                (
                    new Vector2Int(
                        Mathf.RoundToInt(piecePosition.Value.position.x),
                        Mathf.RoundToInt(piecePosition.Value.position.z)
                    )
                );

                Debug.Log("Occupied by" + piecePosition.Key.name + ", at" + piecePosition.Value.position);
            }
            AddGraphEdges();
        }
        public void SetUpBoard()
        {
            board = new List<GroundStack>();
            coordinates = new List<Coordinate>();
            piecePositions = new Dictionary<Piece, Coordinate>();

            _team1 = player1.Team; // Print safe
            _team2 = player2.Team; // Print safe
            
            board.AddRange(boardObject.GetComponentsInChildren<GroundStack>());
            
            // Add coordinates to each cell
            for (int z = 0; z < _boardSize; z++)
            {
                for (int x = 0; x < _boardSize; x++)
                {
                    int index = z * _boardSize + x;
                    
                    GroundStack stack = board[index]; // Get each stack from the board
                    Coordinate coordinate = new Coordinate(stack, new Vector3(x, stack.Level, z)); // Associate them with coordinates
                    coordinates.Add(coordinate);
                    _graph.AddVertex(new Vector2Int(x, z));

                    #region SetupPieceOnBoard

                    

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
                    if(coordinate.position == new Vector3(5, stack.Level, 5)) // Please change to (9,y,5)
                    {
                        SetUpPiecePositions(player1, 3, coordinate, stack);
                    }
                    if(coordinate.position == new Vector3(9, stack.Level, 6))
                    {
                        SetUpPiecePositions(player1, 4, coordinate, stack);
                    }

                    if (coordinate.position == new Vector3(9, stack.Level, 7))
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
                    #endregion
                }
            }
        }

        private void AddGraphEdges()
        {
            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (Vector2Int vertex in _graph.Vertices)
            {
                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = vertex + direction;

                    if (!_graph.ContainsVertex(neighbor)) continue;

                    int fromIndex = vertex.y * _boardSize + vertex.x;
                    int toIndex = neighbor.y * _boardSize + neighbor.x;

                    GroundStack from = board[fromIndex];
                    GroundStack to = board[toIndex];

                    int cost = GetMovementCost(from, to);
                    if (!IsOccupied(neighbor)) _graph.AddEdge(vertex, neighbor, cost);
                }
            }
        }
        
        public Dictionary<Vector2Int, int> GetDistances(Piece piece)
        {
            Coordinate coordinate = piecePositions[piece];
            Dictionary<Vector2Int, int> distances =
                Dijkstra(_graph.adjacency,
                    new Vector2Int(
                        Mathf.RoundToInt(coordinate.position.x),
                        Mathf.RoundToInt(coordinate.position.z)
                    )
                );
            return distances;
        }

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
        
        
        private int GetMovementCost(GroundStack from, GroundStack to)
        {
            int heightDifference = Mathf.Abs(to.Level - from.Level);
            return 1 + heightDifference;
        }

        public void PlaceCircles(Piece piece)
        {
            List<Vector2Int> occupiedPositions = new List<Vector2Int>(piecePositions.Count);

            foreach (var piecePosition in piecePositions)
            {
                occupiedPositions.Add
                (
                    new Vector2Int(
                        Mathf.RoundToInt(piecePosition.Value.position.x),
                        Mathf.RoundToInt(piecePosition.Value.position.z)
                    )
                );

                Debug.Log("Occupied by" + piecePosition.Key.name + ", at" + piecePosition.Value.position);
            }

            Coordinate sourceCoordinate = piecePositions[piece];
            Dictionary<Vector2Int, int> distances = GetDistances(piece);
            int index = 0;
            foreach (KeyValuePair<Vector2Int, int> distance in distances)
            {
                Vector2Int possiblePosition = distance.Key;
                int d = distance.Value;

                // If distance is too much for piece, don't show a circle
                if (d > piece.MovementCount) continue;
                
                // If it is the source, don't show
                if (possiblePosition.x == Mathf.RoundToInt(sourceCoordinate.position.x) && possiblePosition.y == Mathf.RoundToInt(sourceCoordinate.position.z)) continue;
                
                // If it is occupied, don't show
                if (IsOccupied(possiblePosition)) continue;
                
                GroundStack stack = GetStack(possiblePosition);

                if (index >= circlePreFabs.Count) break;

                circlePreFabs[index].transform.position = ToBoardCoordinate(stack);
                circlePreFabs[index].gameObject.SetActive(true);
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

        private bool IsOccupied(Vector2Int targetPosition)
        {
            if(occupiedPositions.Contains(targetPosition)) return true;
            return false;
        }
        
        private GroundStack GetStack(Vector2Int position)
        {
            int index = position.y * _boardSize + position.x;
            return board[index];
        }

        public void HideCircles()
        {
            foreach(GameObject circle in circlePreFabs)
            {
                circle.gameObject.SetActive(false);
            }
        }
        
        private void SetUpPiecePositions(Player player, int pieceNumber, Coordinate coordinate, GroundStack stack)
        {
            Piece piece = player.Team[pieceNumber];
            piecePositions.Add(piece, coordinate);
            piece.transform.position = ToBoardCoordinatePiece(piece, stack);
        }

        private Vector3 ToBoardCoordinatePiece(Piece piece, GroundStack stack)
        {
            return new Vector3(stack.transform.position.x, stack.Top() + piece.GetMidHeight(), stack.transform.position.z);
        }
        
        private Vector3 ToBoardCoordinate(GroundStack stack)
        {
            return new Vector3(stack.transform.position.x, stack.Top() + 0.12f, stack.transform.position.z);
        }
    }
    
    [Serializable]
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

    [Serializable]
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