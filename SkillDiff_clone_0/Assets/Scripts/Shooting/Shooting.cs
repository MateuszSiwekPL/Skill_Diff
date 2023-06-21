using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
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
        cam = Camera.main;
        line.positionCount = 2;
        reloadStatus = GameObject.Find("Reload_Indicator").GetComponent<Image>();
        
    }
   private void Update() 
   {
        if (controlls.Player.Shooting.WasPressedThisFrame() && canShoot)
        {
            Shoot();
        }
   }

    private void Shoot()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, whatIsShootable))
        {
            canShoot = false;
            IKillable target = hit.collider.GetComponent<IKillable>();
            if(target != null)
            {
                target.Kill();
            }

            StartCoroutine(ShootingCooldown());
            StartCoroutine(SmokeTrail());
        }
    }

    IEnumerator ShootingCooldown()
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
        canShoot = true;
    }

    IEnumerator SmokeTrail()
    {
        line.enabled = true;
        line.SetPosition(0, cam.transform.position);
        line.SetPosition(1, hit.point);
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


    // private void OnDrawGizmos() 
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(cam.transform.position, cam.transform.forward * rayLength);
    // }

}
