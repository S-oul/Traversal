using UnityEngine;

namespace Game.Scripts
{
    public class Vanguard : Piece
    {
        [SerializeField] private int attack = 2;
        [SerializeField] private int movementCount = 4;
        public override void Move()
        {
            throw new System.NotImplementedException();
        }
    }
}