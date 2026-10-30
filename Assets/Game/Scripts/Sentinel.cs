using UnityEngine;

namespace Game.Scripts
{
    public class Sentinel : Piece
    {
        [SerializeField] private int attack = 3;
        [SerializeField] private int movementCount = 3;
        public override void Move()
        {
            throw new System.NotImplementedException();
        }
    }
}