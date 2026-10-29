using UnityEngine;

namespace Game.Scripts
{
    public abstract class Piece : MonoBehaviour
    {
        private string name;
        private int movementCount;
        private int attack;
        // private int defense = attack + height + allies;

        public abstract void Move();
    }
}
