using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class Dashing : NetworkBehaviour
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

    [Header("Cooldown")] 
    Image cooldownBar;
    float timePassed;
    [SerializeField] int a;

    private void Awake() 
    {
        rb = gameObject.GetComponent<Rigidbody>();
        controlls = new PlayerInputs();
        movementController = gameObject.GetComponent<MovementController>();
        cooldownBar = GameObject.Find("Dash_Indicator").GetComponent<Image>();
    }

    private void Update() 
    {
        if(!IsOwner) return;
        Dash();
    }
    private void Dash()
    {
        if(!IsOwner) return;
        if(!canDash) return;

        if (controlls.Player.RightDashing.WasPressedThisFrame())
        {
            AddingForceServerRpc(transform.right);
            StartCoroutine(AddingForce(transform.right));
        }

        if (controlls.Player.LeftDashing.WasPressedThisFrame())
        {
            AddingForceServerRpc(-transform.right);
            StartCoroutine(AddingForce(-transform.right));
        }

        if (controlls.Player.DownDashing.WasPressedThisFrame())
        {
            AddingForceServerRpc(-transform.up);
            StartCoroutine(AddingForce(-transform.up));
        }

       
    }
    [ServerRpc]
    private void AddingForceServerRpc(Vector3 direction) 
    {
        if(!canDash) return;
        StartCoroutine(AddingForce(direction));
        
    }
    
    IEnumerator AddingForce(Vector3 direction)
    {
        
        movementController.dashing = true;
        canDash = false;
        StartCoroutine(DashCooldown());
        StartCoroutine(DashDuration());

        if(IsOwner)
        StartCoroutine(DashIndicator());
        
        while(movementController.dashing)
        {
            rb.AddForce(direction * dashForce * 10f, ForceMode.Force);
            a++;
            yield return new WaitForFixedUpdate();
        } 
    }
    IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator DashIndicator()
    {
        timePassed = 0f;
        while (timePassed < dashCooldown)
        {
            cooldownBar.fillAmount = timePassed/dashCooldown;
            timePassed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        } 
        cooldownBar.fillAmount = 1f;
    }

    IEnumerator DashDuration()
    {
        yield return new WaitForSeconds(dashDuration);
        //DashDurationServerRpc();
        movementController.dashing = false;
    }

    

    private void OnEnable() => controlls.Enable();
    private void OnDisable() => controlls.Disable();



}
