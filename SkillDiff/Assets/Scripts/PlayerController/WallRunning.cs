using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
   [Header("References")]
   MovementController movementController;
   PlayerInputs controlls;
   Rigidbody rb;

   [Header("Wall Run Values")]
   [SerializeField] float rayLength;
   [SerializeField] LayerMask whatIsWall;
   [SerializeField] bool isWallRunning;
   [SerializeField] bool canWallRun;
   RaycastHit wallHit;
   [SerializeField] Vector3 wallDirection;
   [SerializeField] Vector3 wallRunDirection;
   [SerializeField] float wallRunCooldwn;


   [Header("Wall Run Values")]
   [SerializeField] float wallRunSpeed;


    private void Awake() 
    {
        movementController = gameObject.GetComponent<MovementController>();
        rb = gameObject.GetComponent<Rigidbody>();
        controlls = new PlayerInputs();
    }

    private void Update() 
    {
        if (!isWallRunning && canWallRun)
        WallCheck();

        else
        StopCheck();
    }

    private void WallCheck()
    {
        if (movementController.isGrounded)
        return;

        if(Physics.Raycast(transform.position, transform.right, out wallHit, rayLength, whatIsWall))
        {
            wallDirection = wallHit.normal;
            wallRunDirection = Vector3.Cross(wallDirection, transform.up);
            if((transform.forward - wallRunDirection).magnitude > (transform.forward - -wallRunDirection).magnitude)
            wallRunDirection = -wallRunDirection;

            StartCoroutine(nameof(WallRun));
            return;
        }

         if(Physics.Raycast(transform.position, -transform.right, out wallHit, rayLength, whatIsWall))
        {
            wallDirection = wallHit.normal;
            wallRunDirection = Vector3.Cross(wallDirection, transform.up);
            if((transform.forward - wallRunDirection).magnitude > (transform.forward - -wallRunDirection).magnitude)
            wallRunDirection = -wallRunDirection;

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

        if(!Physics.Raycast(transform.position, transform.right, rayLength, whatIsWall) && !Physics.Raycast(transform.position, -transform.right, rayLength, whatIsWall))
        {
            StopWallRun();
            return;
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

    }

    IEnumerator WallRunCooldwn()
    {
        yield return new WaitForSeconds(wallRunCooldwn);
        canWallRun = true;
    }
    IEnumerator WallRun()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        movementController.isWallRunning = true;
        rb.useGravity = false;
        isWallRunning = true;
        

        while(true)
        {
            yield return new WaitForFixedUpdate();
            rb.AddForce(wallRunDirection * wallRunSpeed * 10f, ForceMode.Force);
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
