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
                    if(outliner) outliner.enabled = false;
                    outliner = hitInfo.collider.GetComponent<Outline>(); // New piece
                    print(hitInfo.collider.name);
                    
                    Piece piece = hitInfo.collider.GetComponent<Piece>();
                    Vector3 currentPos = hitInfo.transform.position;
                    int moveCount = piece.MovementCount;
                    
                    
                    for (int i = 0; i < moveCount; i++)
                    {
                        Vector3 possiblePos = currentPos;
                        possiblePos += new Vector3(currentPos.x + i, currentPos.y, currentPos.z + i);
                    }
                    if(outliner) outliner.enabled = true;
                }
            }
            
        }
        
        public void OnRightClick(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                if(outliner) outliner.enabled = false;
                Debug.Log("Right click performed");
            }
        }
    }
}