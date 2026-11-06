using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Controls
{
    public class PlayerController : MonoBehaviour
    {
        private Outline outliner;
        
        [SerializeField] BoardManager boardManager;
        
        public void OnLeftClick(InputAction.CallbackContext callbackContext)
        {
            
            if (callbackContext.performed)
            {
                Debug.Log("Left click performed");
                Camera cam = Camera.main;
                
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~0))
                {
                    if(outliner)
                    {
                        boardManager.HideCircles();
                        outliner.enabled = false;
                    }
                    outliner = hitInfo.collider.GetComponent<Outline>(); // New piece
                    print(hitInfo.collider.name);
                   
                    Piece piece = hitInfo.collider.GetComponent<Piece>();
                    GroundStack stack;
                    if(!boardManager.piecePositions.TryGetValue(piece,out stack))
                    {
                        print(piece + "Does not exist in priceposition");
                        return;
                    }
                    boardManager.ManhattanDistanceWithHeight(piece);
                    if(outliner) outliner.enabled = true;
                }
            }
            
        }
        
        public void OnRightClick(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                boardManager.HideCircles();
                if(outliner) outliner.enabled = false;
                Debug.Log("Right click performed");
            }
        }
    }
}