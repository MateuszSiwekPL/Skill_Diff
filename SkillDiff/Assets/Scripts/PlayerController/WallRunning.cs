using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WallRunning : NetworkBehaviour
{
   [Header("References")]
   MovementController movementController;
   PlayerInputs controlls;
   Rigidbody rb;
   Jumping jumping;

   [Header("Wall Run Values")]
   [SerializeField] float rayLength;
   [SerializeField] LayerMask whatIsWall;
   [SerializeField] bool isWallRunning;
   [SerializeField] bool canWallRun;
   RaycastHit wallHit;
   [SerializeField] Vector3 wallDirection;
   [SerializeField] Vector3 wallRunDirection;
   [SerializeField] float wallRunCooldwn;

   private enum WallSide
   {
        right,
        left
   }
   [SerializeField] WallSide wallSide;


   [Header("Wall Run Values")]
   [SerializeField] float wallRunSpeed;


    private void Awake() 
    {
        movementController = gameObject.GetComponent<MovementController>();
        rb = gameObject.GetComponent<Rigidbody>();
        jumping = gameObject.GetComponent<Jumping>();
        controlls = new PlayerInputs();
    }

    private void Update() 
    {
        if (!isWallRunning && canWallRun)
        WallCheck();

        else if(isWallRunning)
        StopCheck();
    }

    private void WallCheck()
    {
        if (movementController.isGrounded)
        return;

        if(Physics.Raycast(transform.position, transform.right, out wallHit, rayLength, whatIsWall))
        {
            wallSide = WallSide.right;
            StartCoroutine(nameof(WallRun));
            return;
        }

        if(Physics.Raycast(transform.position, -transform.right, out wallHit, rayLength, whatIsWall))
        {
            wallSide = WallSide.left;
            StartCoroutine(nameof(WallRun));
            return;
        }
    }
    private void StopCheck()
    {
        if (controlls.Player.DoubleJump.WasPressedThisFrame())
        {
            StopWallRun();
            return;
        }

        if(wallSide == WallSide.right)
        {
             if(!Physics.Raycast(transform.position, transform.right, out wallHit, rayLength + 0.5f, whatIsWall))
             {
                StopWallRun();
                return;  
             }
        }
        else 
        {
            if(!Physics.Raycast(transform.position, -transform.right, out wallHit, rayLength + 0.5f, whatIsWall))
             {
                StopWallRun();
                return;  
             }
        }
    }
    private void StopWallRun()
    {
        StopCoroutine(nameof(WallRun));
        StartCoroutine(WallRunCooldwn());
        rb.useGravity = true;
        isWallRunning = false;
        canWallRun = false;
        movementController.isWallRunning = false;
        jumping.canDoubleJump = true;
    }

    IEnumerator WallRunCooldwn()
    {
        yield return new WaitForSeconds(wallRunCooldwn);
        canWallRun = true;
    }
    IEnumerator WallRun()
    {
        movementController.isWallRunning = true;
        rb.useGravity = false;
        isWallRunning = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        while(true)
        {
            wallDirection = wallHit.normal;
            wallRunDirection = Vector3.Cross(wallDirection, transform.up);
            if((transform.forward - wallRunDirection).magnitude > (transform.forward - -wallRunDirection).magnitude)
            wallRunDirection = -wallRunDirection;

            rb.AddForce(wallRunDirection * wallRunSpeed * 10f, ForceMode.Force);

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.right * rayLength);
        Gizmos.DrawRay(transform.position, -transform.right * rayLength);
    }
    private void OnEnable() => controlls.Enable();
    private void OnDisable() => controlls.Disable();
}
