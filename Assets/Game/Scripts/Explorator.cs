using UnityEngine;

namespace Game.Scripts
{
    public class Explorator : Piece
    {
        [SerializeField] private int attack = 1;
        [SerializeField] private int movementCount = 5;
        
        public override void Move()
        {
            throw new System.NotImplementedException();
        }
    }
}