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

    enum State
    {
        running,
        dashing,
        wallRunning
    }
    [Header("Server Reconciliation")]
    [SerializeField] int tick = 0;
    [SerializeField] Vector3[] positions = new Vector3[1024];
    int buffer = 1024;



    private void Awake() 
    {
        controlls = new PlayerInputs();
        rb = gameObject.GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        if(!IsOwner) return;

        StateHandler();
        StateHandlerServerRpc();

        GroundCheck();
        GroundCheckServerRpc();

        SpeedConstrain();
        SpeedConstrainServerRpc();

        
        Vector2 input = controlls.Player.Running.ReadValue<Vector2>();

        if (state != State.wallRunning)
        Running(input);
        
    }
    [ServerRpc]
    private void StateHandlerServerRpc() => StateHandler();

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
    private void RunningServerRpc(Vector2 input, int clientTick)
    {
        tick = clientTick;
        Running(input); 
    } 
    private void Running(Vector2 input)
    {
        if(IsOwner)
        RunningServerRpc(input, tick);

        Vector3 runDirection = transform.forward * input.y + transform.right * input.x;
        rb.AddForce(runDirection.normalized * runSpeed * 10f, ForceMode.Force);
        speed = rb.velocity.magnitude;

        // Physics.Simulate(Time.fixedDeltaTime);

        // if(IsOwner)
        // {
        //     positions[tick % buffer] = transform.position;
        //     tick ++;
        // }
        
        // if(IsServer)
        // {
        //     PositionCorrectionClientRpc(transform.position, tick, rb.velocity, rb.angularVelocity);
        //     positions[tick % buffer] = transform.position;
        // }
        // Debug.Log(tick.ToString());
    }
           
     

    [ClientRpc]
    public void PositionCorrectionClientRpc(Vector3 serverPosition, int serverTick, Vector3 velocity, Vector3 aVelocity)
    {
        if(!IsOwner) return;
        
        Vector3 correction = serverPosition - positions[serverTick % buffer];
        if (correction.magnitude > 0.00000001)
        {
            transform.position = serverPosition;
            rb.velocity = velocity;
            rb.angularVelocity = aVelocity;
            Debug.Log(correction.ToString());
            tick = serverTick;
        }
    }

    [ServerRpc]
    private void GroundCheckServerRpc() => GroundCheck();

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position - groundCheckPosition, groundCheckRadious, whatIsGround);

        rb.drag = isGrounded == true? groundedDrag : airDrag;
    }
    [ServerRpc]
    private void SpeedConstrainServerRpc() => SpeedConstrain();

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
