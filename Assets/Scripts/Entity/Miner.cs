using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : Entity
{
    public GameObject turret;

    private bool canAttach;
    public Player tic;
    public BoxCollider2D myCollider;

    public MinorHUD minerHUD;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q) && GameManager.ticDocked == true)
        {
            if (GameManager.minerDocked == false && canAttach == true)
            {
                Movement.MyBody.velocity = Vector2.zero;
                tic.AttachMiner();
                CameraManager.ActivateRotation(CameraManager.Instance.planetZoom, CameraManager.ZoomContext.PlanetSurface);
                myCollider.enabled = false;
            }
            else
            {
                tic.DetachMiner();
                CameraManager.ActivateRotation(CameraManager.Instance.minerZoom, CameraManager.ZoomContext.Miner);
                myCollider.enabled = true;
            }
        }
    }
    private void FixedUpdate()
    {
        TargetUtilities.RotateSnapTowardMouse(turret.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "TICInterior")
        {
            canAttach = true;
            //release resources method 
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "TICInterior")
        {
            canAttach = false;
        }
    }
}
