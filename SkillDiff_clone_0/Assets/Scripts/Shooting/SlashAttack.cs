using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SlashAttack : NetworkBehaviour
{
    [Header("References")]
    MovementController movementController;
    Rigidbody rb;
    PlayerInputs controlls;

    [Header("Attack Values")]
    Camera cam;
    [SerializeField] float attackCooldown;
    [SerializeField] float dashForce;
    [SerializeField] float attackDuration;
    [SerializeField] bool canAttack;
    [SerializeField] bool attacking;

    [Header("Cooldown")]
    Image cooldownBar;
    float timePassed;

    private void Awake()
    {
        controlls = new PlayerInputs();
    }
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        movementController = gameObject.GetComponent<MovementController>();
        cam = Camera.main;
        cooldownBar = GameObject.Find("Attack_Indicator").GetComponent<Image>();
    }

    private void Update() 
    {
        if(!IsOwner) return;

        if (controlls.Player.SlashAttack.WasPressedThisFrame())
        AttackServerRpc(cam.transform.forward);
        
    }

    [ServerRpc]
    private void AttackServerRpc(Vector3 direction)
    {
        if (!canAttack) return;

        StartCoroutine(EnableAttack());
        StartCoroutine(AddingForce(direction));
        SlashIndicatorClientRpc();
        
    }

    IEnumerator EnableAttack()
    {
        attacking = true;
        yield return new WaitForSeconds(attackDuration);
        attacking = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
    }
    IEnumerator AddingForce(Vector3 direction)
    {
        movementController.dashing = true;
        StartCoroutine(AttackDuration());
        StartCoroutine(AttackCooldown());

        while(movementController.dashing)
        {
            yield return new WaitForFixedUpdate();
            rb.AddForce(direction * dashForce * 10f, ForceMode.Force);
        } 
    }
    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    [ClientRpc]
    private void SlashIndicatorClientRpc()
    {
        if(!IsOwner) return;
        StartCoroutine(SlashIndicator());
    }
    IEnumerator SlashIndicator()
    {
        timePassed = 0f;
        while (timePassed < attackCooldown)
        {
            cooldownBar.fillAmount = timePassed/attackCooldown;
            timePassed += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        } 
        cooldownBar.fillAmount = 1f;
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(attackDuration);
        movementController.dashing = false;
    }

    private void OnTriggerEnter(Collider col) 
    {
        if(attacking)
        {
            IKillable target = col.GetComponent<IKillable>();
            if (target != null)
            target.Kill();
        }
    }

    private void OnEnable() => controlls.Enable();
    private void OnDisable() => controlls.Disable();

}
