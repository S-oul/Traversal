using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts
{
    public abstract class Piece : MonoBehaviour
    {
        // private int defense = attack + height + allies;
        
        
        [SerializeField] private int attack;
        [SerializeField] private int movementCount;
        
        [SerializeField] private GroundStack _stack;
        public int Attack => attack;
        public int MovementCount => movementCount;


        private Player _player;
        [SerializeField] private BoardManager boardManager;
        
        public Player Player { get => _player; set => _player = value; }
        public GroundStack Stack { get => _stack; set => _stack = value; }

        // public void Move(Vector3 arrival)
        // {

        //     int chosenMoveCount = 0;

        //     arrival = boardManager.coordinates[chosenMoveCount].position;
        // }

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
