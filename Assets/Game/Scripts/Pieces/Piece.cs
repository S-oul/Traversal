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
        [SerializeField] private BoardManager boardManager;
        private Player _player;
        
        private Outline _outline;

        public int Attack => attack;
        public int MovementCount => movementCount;
        public Player Player { get => _player; set => _player = value; }
        public GroundStack Stack { get => _stack; set => _stack = value; }

        void Start()
        {
            _outline = GetComponent<Outline>();
        }

        public void SelectPiece()
        {
            _outline.enabled = true;
        }

        public void UnSelectPiece()
        {
            _outline.enabled = false;
        }

        // public void Move(Vector3 arrival)
        // {

        //     int chosenMoveCount = 0;

        //     arrival = boardManager.coordinates[chosenMoveCount].position;
        // }

        public abstract char AlgebraicNotation();

        public void toAlphabet()
        {
        }

        public float GetMidHeight()
        {
            return GetComponent<Collider>().bounds.size.y/2;
        }
        
    }
    
    
}
