using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
   
     PlayerInputs controlls;
     public Transform player;


    float sensitivity = 25f;
    float rotationX = 0f;
    float rotationY = 0f;
    Vector2 look;
    


   private void Awake() 
   {
          controlls = new PlayerInputs();
          //Cursor.lockState = CursorLockMode.Locked;
          //Cursor.visible = false;
          
   } 

   private void Update() 
   {
        Looking();
   }

   private void Looking()
   {
        look = controlls.Player.Look.ReadValue<Vector2>();
        float lookX = look.x * sensitivity * Time.deltaTime;
        float lookY = look.y * sensitivity * Time.deltaTime;

        rotationY += lookX;

        rotationX -= lookY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);

        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        player.rotation = Quaternion.Euler(0, rotationY, 0);

   }



   private void OnEnable() => controlls.Enable();

   private void OnDisable() => controlls.Disable();
}
