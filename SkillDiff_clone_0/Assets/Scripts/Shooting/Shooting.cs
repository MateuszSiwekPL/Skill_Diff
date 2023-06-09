using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Shooting : NetworkBehaviour
{
   [Header("References")]
   PlayerInputs controlls;
   [SerializeField] LineRenderer line;
   Image reloadStatus;
   Camera cam;

    [Header("Raycasting")]
    [SerializeField] float rayLength;
    [SerializeField] LayerMask whatIsShootable;
    RaycastHit hit;

    [Header("Shooting")]
    [SerializeField] bool canShoot;
    [SerializeField] float shootingCooldown;
    float timePassed;


    private void Awake()
    {
        controlls = new PlayerInputs();
    }
    private void Start() 
    {
        cam = Camera.main;
        line.positionCount = 2;
        reloadStatus = GameObject.Find("Reload_Indicator").GetComponent<Image>();
        
    }
   private void Update() 
   {
        if(!IsOwner) return;

        if (controlls.Player.Shooting.WasPressedThisFrame())
        {
            ShootServerRpc(cam.transform.forward);
            Shoot(cam.transform.forward);
        }
   }

    [ServerRpc]
    private void ShootServerRpc(Vector3 direction) => Shoot(direction);
    private void Shoot(Vector3 direction)
    {
        if(!canShoot) return;

        if(Physics.Raycast(transform.position, direction, out hit, rayLength, whatIsShootable))
        {
            IKillable target = hit.collider.GetComponent<IKillable>();
            if(target != null) target.Kill();
            
            StartCoroutine(ShootingCooldown());

            if(IsOwner)
            {
            StartCoroutine(SmokeTrail(hit.point));
            StartCoroutine(ShootingIndicator());
            }

            if(IsServer)
            SmokeTrailClientRpc(hit.point);
        }
        
    }
    IEnumerator ShootingCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }

    IEnumerator ShootingIndicator()
    {
        reloadStatus.fillAmount = 0f;
        timePassed = 0f;
        while (timePassed < shootingCooldown)
        {
            reloadStatus.fillAmount = timePassed;
            yield return new WaitForFixedUpdate();
            timePassed += Time.deltaTime;
        }
        reloadStatus.fillAmount = 1f;
    }

    [ClientRpc]
    private void SmokeTrailClientRpc(Vector3 hitPosition)
    {
        if(!IsOwner)
        StartCoroutine(SmokeTrail(hitPosition));
    }
    IEnumerator SmokeTrail(Vector3 hitPosition)
    {
        
        line.enabled = true;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, hitPosition);
        line.startWidth = 0.25f;
        line.endWidth = 0.25f;
        
        while (line.startWidth > 0f)
        {
            yield return new WaitForFixedUpdate();
            line.startWidth -= 0.007f;
            line.endWidth -= 0.007f;
        }

        line.enabled = false;
        
    }

    private void OnEnable() => controlls.Enable();
    private void OnDisable() => controlls.Disable();

}
