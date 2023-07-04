using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
   
     PlayerInputs controlls;
     public Transform player;
     PlayerRotation playerRotation;


    float sensitivity = 25f;
    float rotationX = 0f;
    float rotationY = 0f;
    


   private void Awake() 
   {
          controlls = new PlayerInputs();
          //Cursor.lockState = CursorLockMode.Locked;
          //Cursor.visible = false;
   } 

   private void Start() 
   {
     playerRotation = player.gameObject.GetComponent<PlayerRotation>();
   }

   private void Update() => Looking();

     private void Looking()
     {
          Vector2 look = controlls.Player.Look.ReadValue<Vector2>();
          float lookX = look.x * sensitivity * Time.deltaTime;
          float lookY = look.y * sensitivity * Time.deltaTime;

          rotationY += lookX;

          rotationX -= lookY;
          rotationX = Mathf.Clamp(rotationX, -90f, 90f);

          transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
          player.rotation = Quaternion.Euler(0, rotationY, 0);
          playerRotation.RotationServerRpc(rotationY);
     }

     private void OnEnable() => controlls.Enable();

     private void OnDisable() => controlls.Disable();
}
