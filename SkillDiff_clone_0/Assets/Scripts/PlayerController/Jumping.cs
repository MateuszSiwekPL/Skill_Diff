using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Jumping : NetworkBehaviour
{
    [Header("References")]
    MovementController movementController;
    Rigidbody rb;
    PlayerInputs controlls;
    

    [Header("Jump Values")]
    [SerializeField] bool canJump;
    public bool canDoubleJump;
    [SerializeField] float jumpCooldown;
    [SerializeField] float jumpForce;
    [SerializeField] bool jumped;


    private void Awake() 
    {
        rb = gameObject.GetComponent<Rigidbody>();
        controlls = new PlayerInputs();
        movementController = gameObject.GetComponent<MovementController>();
    }
    private void Update() 
    {
        if(!IsOwner) return;
        
        if (controlls.Player.Jumping.ReadValue<float>() > 0 && (movementController.isGrounded))
        {
            JumpServerRpc();
            Jump();
        }

        if(controlls.Player.DoubleJump.WasPressedThisFrame())
        {
            DoubleJumpServerRpc();
            DoubleJump();
        }

    }
    [ServerRpc]
    private void JumpServerRpc() => Jump();
    private void Jump()
    {   
        if (!canJump)
        return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce * 10f, ForceMode.Impulse);
        canJump = false;
        StartCoroutine(JumpCooldown());
        
       
    }
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    [ServerRpc]
    private void DoubleJumpServerRpc() => DoubleJump();
    private void DoubleJump()
    {
        if((movementController.isGrounded || movementController.isWallRunning) && !canDoubleJump)
        canDoubleJump = true;

        if(!canDoubleJump || movementController.isGrounded)
        return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce * 10f, ForceMode.Impulse);
        canDoubleJump = false;
        
    }
    private void OnEnable() => controlls.Enable();

    private void OnDisable() => controlls.Disable();
}
