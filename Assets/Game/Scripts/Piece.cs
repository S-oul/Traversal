using System;
using UnityEngine;

namespace Game.Scripts
{
    public abstract class Piece : MonoBehaviour
    {
        // private int defense = attack + height + allies;
        private int movementCount;

        [SerializeField] private BoardManager boardManager;
        
        public void Move()
        {
            
        }
        
        public string AlgebraicNotation()
        {
            return "";
        }

        public void toAlphabet()
        {
        }

        public float GetMidHeight()
        {
            return GetComponent<Collider>().bounds.size.y/2;
        }
        
    }
    
    
}
