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
    public bool wallRunning;
    [SerializeField] State state;

    enum State
    {
        running,
        dashing
    }
    
   

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
        StateHandler();
        GroundCheck();
        SpeedConstrain();
    }

    private void StateHandler()
    {
        if(dashing)
        {
            state = State.dashing;
            maxVelocity = 20f;
        }

        else
        {
            state = State.running;
            maxVelocity = 10f;
        }
    }

    private void Running()
    {
        Vector2 input = controlls.Player.Running.ReadValue<Vector2>();
        Vector3 runDirection = transform.forward * input.y + transform.right * input.x;
        rb.AddForce(runDirection.normalized * runSpeed * 10f, ForceMode.Force);
        speed = rb.velocity.magnitude;
    }
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position - groundCheckPosition, groundCheckRadious, whatIsGround);

        rb.drag = isGrounded == true? groundedDrag : airDrag;


    }
    private void SpeedConstrain()
    {
        Vector3 playerSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (playerSpeed.magnitude > maxVelocity)
        {
            Vector3 newSpeed = playerSpeed.normalized * maxVelocity;
            rb.velocity = new Vector3(newSpeed.x, rb.velocity.y, newSpeed.z);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - groundCheckPosition, groundCheckRadious);
    }

   private void OnEnable() => controlls.Enable();

   private void OnDisable() => controlls.Disable();

}
