using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    MovementController movementController;
    Rigidbody rb;
    PlayerInputs controlls;

    [Header("DashValues")]
    [SerializeField] float dashForce;
    [SerializeField] float dashCooldown;
    [SerializeField] bool canDash;
    [SerializeField] float dashDuration;

    [Header("Slash Attack")]
    bool attacking;
    Camera cam;

    private void Awake() 
    {
        rb = gameObject.GetComponent<Rigidbody>();
        controlls = new PlayerInputs();
        movementController = gameObject.GetComponent<MovementController>();
        cam = Camera.main;
    }

    private void Update() 
    {
        Dash();
    }
    private void Dash()
    {
        if(!canDash)
        return;

        if (controlls.Player.RightDashing.WasPressedThisFrame())
        {
            StartCoroutine(AddingForce(transform.right));
        }

        if (controlls.Player.LeftDashing.WasPressedThisFrame())
        {
            StartCoroutine(AddingForce(-transform.right));
        }

        if (controlls.Player.DownDashing.WasPressedThisFrame())
        {
            StartCoroutine(AddingForce(-transform.up));
        }

        if (controlls.Player.SlashAttack.WasPressedThisFrame())
        {
            StartCoroutine(EnableAttack());
            StartCoroutine(AddingForce(cam.transform.forward));

        }
    }
    IEnumerator AddingForce(Vector3 direction)
    {
        movementController.dashing = true;
        canDash = false;
        StartCoroutine(DashDuration());
        StartCoroutine(DashCooldown());

        while(movementController.dashing)
        {
            yield return new WaitForFixedUpdate();
            rb.AddForce(direction * dashForce * 10f, ForceMode.Force);
        } 
    }
    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown); 
        canDash = true;
        
    }
    IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(dashDuration);
        movementController.dashing = false;
    }

    IEnumerator EnableAttack()
    {
        attacking = true;
        yield return new WaitForSeconds(dashDuration);
        attacking = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }

    private void OnCollisionEnter(Collision col) 
    {
        if(attacking)
        {
        IKillable target = col.gameObject.GetComponent<IKillable>();
        if (target != null)
        target.Kill();
        }
    }

    private void OnEnable() => controlls.Enable();
    private void OnDisable() => controlls.Disable();



}
