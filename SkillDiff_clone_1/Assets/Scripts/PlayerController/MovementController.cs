using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class MovementController : NetworkBehaviour
{
  
    PlayerInputs controlls;

    Rigidbody rb;

    [Header("Move Values")]
    [SerializeField] float runSpeed;
    [SerializeField] float speed;
    [SerializeField] float maxVelocity;
    Vector2 input;

    [Header("GroundCheck")]
    public bool isGrounded;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundCheckRadious;
    [SerializeField] Vector3 groundCheckPosition;

    [Header("Drag Values")]
    [SerializeField] float groundedDrag;
    [SerializeField] float airDrag;

    [Header("StateChecking")]
    public bool dashing;
    public bool isWallRunning;
    [SerializeField] State state;
    [SerializeField] int check;

    enum State
    {
        running,
        dashing,
        wallRunning
    }
    
   

    private void Awake() 
    {
        controlls = new PlayerInputs();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        StateHandler();
        GroundCheck();
        SpeedConstrain();

        if(IsOwner)
        {
            input = controlls.Player.Running.ReadValue<Vector2>();
            InputReadServerRpc(input);
        }
        if (state != State.wallRunning)
        Running();

        
    }
    private void StateHandler()
    {
        if(dashing)
        {
            state = State.dashing;
            maxVelocity = 40f;
        }

        else if(isWallRunning)
        {
            state = State.wallRunning;
            maxVelocity = 15f;
        }

        else
        {
            state = State.running;
            maxVelocity = 15f;
        }
    }

    [ServerRpc]
    private void InputReadServerRpc(Vector2 input) => this.input = input;
    private void Running()
    {
        Vector3 runDirection = transform.forward * input.y + transform.right * input.x;
        rb.AddForce(runDirection.normalized * runSpeed * 10f, ForceMode.Force);
        speed = rb.velocity.magnitude;
        if(input.magnitude > Vector2.zero.magnitude)
        check += 1;
    }
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position - groundCheckPosition, groundCheckRadious, whatIsGround);

        rb.drag = isGrounded == true? groundedDrag : airDrag;
    }
    private void SpeedConstrain()
    {
        Vector3 playerSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        Vector3 newSpeed = Vector3.ClampMagnitude(playerSpeed, maxVelocity);
        rb.velocity = new Vector3(newSpeed.x, rb.velocity.y, newSpeed.z);
        
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - groundCheckPosition, groundCheckRadious);
    }

   private void OnEnable() => controlls.Enable();

   private void OnDisable() => controlls.Disable();

}
