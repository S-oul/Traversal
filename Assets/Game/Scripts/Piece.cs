using System;
using UnityEngine;

namespace Game.Scripts
{
    public abstract class Piece : MonoBehaviour
    {
        // private int defense = attack + height + allies;
        
        
        [SerializeField] private int attack;
        [SerializeField] private int movementCount;

        public int Attack => attack;
        public int MovementCount => movementCount;

        [SerializeField] private BoardManager boardManager;
        
        public void Move(Vector3 arrival)
        {

            int chosenMoveCount = 0;
            
            arrival = boardManager.coordinates[chosenMoveCount].position;
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
