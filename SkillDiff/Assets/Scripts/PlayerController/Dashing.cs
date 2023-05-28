using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] MovementController movementController;
    Rigidbody rb;
    PlayerInputs controlls;

    [Header("DashValues")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    [SerializeField] float waitForDash;

    private void Awake() 
    {
        rb = gameObject.GetComponent<Rigidbody>();
        controlls = new PlayerInputs();
    }

    private void Update() 
    {
        StartCoroutine(Dash());
    }
    IEnumerator Dash()
    {
        if (controlls.Player.RightDashing.WasPressedThisFrame())
        {
            movementController.dashing = true;
            StartCoroutine(StopDash());
            while (movementController.dashing)
            {
            yield return null;
            rb.AddForce(transform.right * dashForce * 10, ForceMode.Force);
            }
            

        }

        if (controlls.Player.LeftDashing.WasPressedThisFrame())
        {
            movementController.dashing = true;
            StartCoroutine(StopDash());
            while (movementController.dashing)
            {
            yield return null;
            rb.AddForce(-transform.right * dashForce * 10, ForceMode.Force);
            }
        }

    }

    IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashDuration); 
        movementController.dashing = false;
    }


    private void OnEnable() => controlls.Enable();

    private void OnDisable() => controlls.Disable();

}
