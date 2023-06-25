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
        ShootingServerRpc(cam.transform.forward);
   }

    [ServerRpc]
    private void ShootingServerRpc(Vector3 direction)
    {
        if(!canShoot) return;

        if(Physics.Raycast(transform.position, direction, out hit, rayLength, whatIsShootable))
        {
            IKillable target = hit.collider.GetComponent<IKillable>();
            if(target != null) target.Kill();
            
            StartCoroutine(ShootingCooldown());
            SmokeTrailClientRpc(hit.point);
        }
        
    }

    [ClientRpc]
    private void SmokeTrailClientRpc(Vector3 hitPosition) => StartCoroutine(SmokeTrail(hitPosition));
    

    IEnumerator ShootingCooldown()
    {
        canShoot = false;
        reloadStatus.fillAmount = 0f;
        timePassed = 0f;
        while (timePassed < shootingCooldown)
        {
            reloadStatus.fillAmount = timePassed;
            yield return new WaitForFixedUpdate();
            timePassed += Time.deltaTime;
        }
        reloadStatus.fillAmount = 1f;
        canShoot = true;
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
