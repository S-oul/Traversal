using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Controls
{
    public class PlayerController : MonoBehaviour
    {
        private Piece _lastSelectedPiece;
        
        [SerializeField] BoardManager boardManager;
        
        public void OnLeftClick(InputAction.CallbackContext callbackContext)
        {
            
            if (callbackContext.performed)
            {
                Camera cam = Camera.main;
                
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                string log = "Left click performed";

                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~0))
                {
                    if(_lastSelectedPiece)
                    {
                        boardManager.HideCircles();
                        _lastSelectedPiece.UnSelectPiece();
                    }

                    if (hitInfo.collider.CompareTag("Circles"))
                    {
                        log += " On Circle";
                        //HandleMov    
                    }

                    if (hitInfo.collider.CompareTag("Piece"))
                    {
                        log += " On Piece";

                        Piece piece = hitInfo.collider.GetComponent<Piece>();
                        
                        piece.SelectPiece();
                        _lastSelectedPiece = piece;

                        boardManager.ManhattanDistanceWithHeight(piece);
                    }
                }
                
                Debug.Log(log);
            }
            
        }
        
        public void OnRightClick(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                Debug.Log("Right click performed");
                boardManager.HideCircles();

            }
        }
    }
}