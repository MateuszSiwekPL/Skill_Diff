using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
   [Header("References")]
   PlayerInputs controlls;
   [SerializeField] LineRenderer lineRenderer;
   [SerializeField] Image reloadStatus;
   Camera cam;

    [Header("Raycasting")]
    [SerializeField] float rayLength;
    [SerializeField] LayerMask whatIsShootable;
    RaycastHit hit;

    private void Awake() 
    {
        controlls = new PlayerInputs();
        cam = Camera.main;

    }
   private void Update() 
   {
        if (controlls.Player.Shooting.WasPressedThisFrame())
        {
            Shoot();
        }
   }

    private void Shoot()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, whatIsShootable))
        {
            IKillable target = hit.collider.GetComponent<IKillable>();

            if(target != null)
            {
                target.Kill();
            }
        }
    }

    private void OnEnable() => controlls.Enable();
    private void OnDisable() => controlls.Disable();


    // private void OnDrawGizmos() 
    // {
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(cam.transform.position, cam.transform.forward * rayLength);
    // }

}
