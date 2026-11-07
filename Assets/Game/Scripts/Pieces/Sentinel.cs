using UnityEngine;

namespace Game.Scripts
{
    public class Sentinel : Piece
    {
        public int Attack => 3;
        public int MovementCount => 3;

        public override char AlgebraicNotation()
        {
            return 'S';
        }
    }
}