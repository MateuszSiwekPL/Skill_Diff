using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Cooldown")] 
    Image cooldownBar;
    float timePassed;

    private void Awake() 
    {
        rb = gameObject.GetComponent<Rigidbody>();
        controlls = new PlayerInputs();
        movementController = gameObject.GetComponent<MovementController>();
        cam = Camera.main;
        cooldownBar = GameObject.Find("Dash_Indicator").GetComponent<Image>();
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
        timePassed = 0f;
        while (timePassed < dashCooldown)
        {
            cooldownBar.fillAmount = timePassed/dashCooldown;
            timePassed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        } 
        cooldownBar.fillAmount = 1f;
        canDash = true;
        
    }
    IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(dashDuration);
        movementController.dashing = false;
    }

    

    private void OnEnable() => controlls.Enable();
    private void OnDisable() => controlls.Disable();



}