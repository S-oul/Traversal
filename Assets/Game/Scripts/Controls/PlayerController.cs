using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Controls
{
    public class PlayerController : MonoBehaviour
    {
        private Outline _outliner;
        
        [SerializeField] BoardManager boardManager;
        [SerializeField] Piece lastSelectedPiece;
        public void OnLeftClick(InputAction.CallbackContext callbackContext)
        {
            if (lastSelectedPiece)
            {
                if (callbackContext.performed)
                {
                    print("first callback");
                    Camera cam = Camera.main;
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    
                    if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~0))
                    {
                        if (hitInfo.collider.CompareTag("Circle"))
                        {
                            print("Cool");
                        }
                        
                    }
                }
            }
            if (!lastSelectedPiece && callbackContext.performed)
            {
                print("Second callback");
                Camera cam = Camera.main;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                
                // If player clicks on the piece
                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~0))
                {
                    if(_outliner)
                    {
                        boardManager.HideCircles();
                        _outliner.enabled = false;
                        lastSelectedPiece = null;
                    }
                    _outliner = hitInfo.collider.GetComponent<Outline>(); // New piece
                    print(hitInfo.collider.name);

                    Piece piece = hitInfo.collider.GetComponent<Piece>();
                    
                    boardManager.PlaceCircles(piece);
                    if(_outliner)
                    {
                        _outliner.enabled = true;
                        lastSelectedPiece = hitInfo.collider.GetComponent<Piece>();
                    }
                }
            }
        }
        
        public void OnRightClick(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                boardManager.HideCircles();
                if(_outliner) _outliner.enabled = false;
                lastSelectedPiece = null;
                Debug.Log("Right click performed");
            }
        }
    }
}