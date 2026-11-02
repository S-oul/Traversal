using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Controls
{
    public class PlayerController : MonoBehaviour
    {
        private Outline outliner;
        public void OnLeftClick(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                Debug.Log("Left click performed");
                Camera cam = Camera.main;
                print(cam.ScreenToWorldPoint(new Vector3(0, 0, 10)));
                
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, ~0))
                {
                    if(outliner) outliner.enabled = false;
                    outliner = hitInfo.collider.GetComponent<Outline>(); // New piece
                    if(outliner) outliner.enabled = true;
                }
            }
            
        }
        
        public void OnRightClick(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                if(outliner) outliner.enabled = false;
                else print("tamere3");
                Debug.Log("Right click performed");
            }
        }
    }
}