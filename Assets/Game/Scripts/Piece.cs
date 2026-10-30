using UnityEngine;

namespace Game.Scripts
{
    public abstract class Piece : MonoBehaviour
    {
        // private int defense = attack + height + allies;

        public abstract void Move();
    }
}
