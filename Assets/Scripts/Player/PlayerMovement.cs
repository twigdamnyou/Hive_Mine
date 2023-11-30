using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : EntityMovement
{
    #region Variables
    public enum PlayerMode
    {
        Space,
        Planet,
        Mining
    }

    [Header("Movement Properties")]
    public float spaceThrustSpeed = 3000f;
    public float spaceRotationSpeed = 200f;
    public float planetHorizontalSpeed = 500f;
    public float escapePlanetLaunchSpeed = 2000f;
    public float basePlanetLaunchSpeed = 1000f;
    private Coroutine launchSequance;
    private bool isLaunching;

    [Header("Required Components")]
    public BoxCollider2D myCollider;


    public PlayerMode playerMode;
    private Vector2 direction;

    #endregion

    #region Built In Methods
    private void Update()
    {
        GetInput();
        //TODO: remove this debugmode movement and do this properly later
        if (Input.GetKeyDown(KeyCode.Space) && playerMode == PlayerMode.Planet)
        {
            if (isLaunching == false)
            {
                launchSequance = StartCoroutine(SmoothlyAdjustThrustForce(escapePlanetLaunchSpeed, 10f));
            }
        }
    }

    protected override void Move()
    {
        base.Move();
        //Debug.Log("Movement is: " + CanMove);

        switch (playerMode)
        {
            case PlayerMode.Space:
                SpaceMove();
                break;
            case PlayerMode.Planet:
                PlanetMove();
                break;
            case PlayerMode.Mining:

                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Planet")
        {
            //Debug.Log("Entering Atmo");
            playerMode = PlayerMode.Planet;
        }

        if (collision.tag == "Mine Entrance")
        {

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "PlanetSurface")
        {
            //Debug.Log("Landing");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Planet")
        {
            //Debug.Log("Exiting Atmo");
            if (launchSequance != null)
            {
                StopCoroutine(launchSequance);
                isLaunching = false;
            }
            playerMode = PlayerMode.Space;
            basePlanetLaunchSpeed = 1000f;
            //myCollider.enabled = false;
            //StartCoroutine(ColliderReenabler());
        }
    }

    #endregion

    #region Movement Methods

    private void ForwardThrust()
    {
        Vector2 wantedPosition = transform.up * direction.y * spaceThrustSpeed * Time.fixedDeltaTime;
        MyBody.AddForce(wantedPosition, ForceMode2D.Force);
    }

    public void Rotation()
    {
        float rotSpeed = direction.x * spaceRotationSpeed * Time.fixedDeltaTime;
        MyBody.AddTorque(-rotSpeed);
    }

    private void HorizontalMove()
    {
        Vector2 wantedPosition = transform.right * direction.x * planetHorizontalSpeed * Time.fixedDeltaTime;
        MyBody.AddForce(wantedPosition, ForceMode2D.Force);
    }

    private void VerticleLaunch()
    {
        Vector2 wantedPosition = transform.up * direction.y * basePlanetLaunchSpeed * Time.fixedDeltaTime;
        MyBody.AddForce(wantedPosition, ForceMode2D.Force);
    }

    private IEnumerator SmoothlyAdjustThrustForce(float targetThrustForce, float speed = 1f)
    {
        isLaunching = true;
        while (basePlanetLaunchSpeed != targetThrustForce)
        {
            float newThrust = Mathf.MoveTowards(basePlanetLaunchSpeed, targetThrustForce, Time.fixedDeltaTime * speed);
            basePlanetLaunchSpeed = newThrust;
            yield return new WaitForEndOfFrame();
            //Debug.Log("Base Speed: " + basePlanetLaunchSpeed);
        }
    }

    public void SpaceMove()
    {
        ForwardThrust();
        Rotation();
    }

    public void PlanetMove()
    {
        VerticleLaunch();
        HorizontalMove();
    }

    #endregion

    #region Input Methods

    private void GetInput()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
    }

    private void MineButtonInput()
    {
         
    }

    #endregion
}
