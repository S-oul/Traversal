using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private List<Piece> team;

        public List<Piece> Team => team;

        void Start()
        {
            team = new List<Piece>();
            team.AddRange(GetComponentsInChildren<Piece>());
        }
    }
}