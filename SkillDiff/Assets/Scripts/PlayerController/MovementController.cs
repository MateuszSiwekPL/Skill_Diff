using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
  
    PlayerInputs controlls;

    Rigidbody rb;

    [Header("Move Values")]
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float speed;

    [Header("GroundCheck")]
    [SerializeField] bool isGrounded;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundCheckRadious;
    [SerializeField] Vector3 groundCheckPosition;

    [Header("Drag Values")]
    [SerializeField] float groundedDrag;
    [SerializeField] float airDrag;

    [Header("Jumping")]
    [SerializeField] bool canJump;
    [SerializeField] bool canDoubleJump;
    [SerializeField] bool jumped;
    [SerializeField] float jumpCooldown;






    private void Awake() 
    {
        controlls = new PlayerInputs();
        rb = gameObject.GetComponent<Rigidbody>();
        
    }

    private void FixedUpdate()
    {
        Running();
    }

    private void Update() 
    {
        GroundCheck();
        Jumping();
        DoubleJumping();
    }


    private void Running()
    {
        Vector2 input = controlls.Player.Running.ReadValue<Vector2>();
        Vector3 runDirection = transform.forward * input.y + transform.right * input.x;

        if (isGrounded)
        rb.AddForce(runDirection.normalized * runSpeed * 10f, ForceMode.Force);
        else
        rb.AddForce(runDirection.normalized * runSpeed, ForceMode.Force);
    }

    private void Jumping()
    {
        if(isGrounded == false)
        return;

        

        if (controlls.Player.Jumping.ReadValue<float>() > 0 && canJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce * 10f, ForceMode.Impulse);
            canJump = false;
            jumped = true;
            StartCoroutine(JumpCooldown());
        }
       
    }
    private void DoubleJumping()
    {
        if(!canDoubleJump || isGrounded)
        return;

        
            if(controlls.Player.DoubleJump.WasPressedThisFrame())
            {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce * 10f, ForceMode.Impulse);
            canDoubleJump = false;
            }
        
    }
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position - groundCheckPosition, groundCheckRadious, whatIsGround);
        rb.drag = isGrounded == true? groundedDrag : airDrag;
        if(!canDoubleJump && isGrounded)
        canDoubleJump = true;
        if(jumped && isGrounded)
        jumped = false;

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - groundCheckPosition, groundCheckRadious);
    }

   




   private void OnEnable() => controlls.Enable();

   private void OnDisable() => controlls.Disable();

}
