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
                        _lastSelectedPiece.UnselectPiece();
                    }

                    if (hitInfo.collider.CompareTag("Circles"))
                    {
                        log += " On Circle";
                        Debug.Log(hitInfo.collider.transform.position);
                        if (boardManager.circleAndStack.TryGetValue(hitInfo.collider.gameObject, out GroundStack currentStack))
                        {
                            Vector3 position3D = boardManager.ToBoardCoordinatePiece(_lastSelectedPiece, currentStack);
                            Vector2Int newPos = new Vector2Int(Mathf.RoundToInt(position3D.x), Mathf.RoundToInt(position3D.z));
                            
                            _lastSelectedPiece.transform.position = boardManager.ToBoardCoordinatePiece(_lastSelectedPiece, currentStack);
                            
                            boardManager.HideCircles();
                            boardManager.UpdateGameState(_lastSelectedPiece, currentStack, newPos);
                        }
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