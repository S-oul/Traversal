using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Graph
    {
        private Dictionary<Vector2Int, List<Edge>> _adjacency = new();

        public Dictionary<Vector2Int, List<Edge>> adjacency
        {
            get => _adjacency;
        }
        
        public IEnumerable<Vector2Int> Vertices => _adjacency.Keys;

        public bool ContainsVertex(Vector2Int vertex)
        {
            return _adjacency.ContainsKey(vertex);
        }

        public void AddVertex(Vector2Int vertex)
        {
            if (!adjacency.ContainsKey(vertex)) adjacency[vertex] = new List<Edge>();
        }

        public void AddEdge(Vector2Int from, Vector2Int to, int cost)
        {
            AddVertex(from);
            AddVertex(to);

            adjacency[from].Add(new Edge(to, cost));
        }

        public List<Edge> GetNeighbors(Vector2Int vertex)
        {
            if (adjacency.TryGetValue(vertex, out var edges)) return edges;

            return null;
        }
    }
    public struct Edge
    {
        public Vector2Int To { get; }
        public int Cost { get; }

        public Edge(Vector2Int to, int cost)
        {
            To = to;
            Cost = cost;
        }
    }
}