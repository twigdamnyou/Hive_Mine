using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static OreClass;

public class Player : Entity
{
    //public GameObject turret;

    public Transform minerTransform;
    //public bool minerDocked;
    public bool Snapped;
    public WeaponManager weaponManager;
    public TractorBeam tractorBeam;

    #region Built In Methods

    protected override void Awake()
    {
        base.Awake();
        GameManager.minerDocked = true;
        GameManager.instance.player = this;
    }

    protected override void OnEnable()
    {
        Debug.Log("Enabling player");
        base.OnEnable();
        HUD hud = PannelManager.GetPanel("HUD") as HUD;
        hud.SetupBars();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Planet" && GameManager.minerDocked == true)
        {
            CameraManager.ActivateRotation(CameraManager.Instance.planetZoom, CameraManager.ZoomContext.PlanetSurface);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Planet")
        {
            CameraManager.ResetRoation(CameraManager.Instance.spaceZoom);
        }
    }

    #endregion

    #region Equipment

    public void ToggleTractorBeam(bool on)
    {
        tractorBeam.gameObject.SetActive(on);
    }

    #endregion

    #region Miner

    public void DetachMiner()
    {
        //Debug.Log("Detaching Miner");
        minerTransform.SetParent(null, true);
        minerTransform.GetComponent<Collider2D>().enabled = true;
        GameManager.minerDocked = false;
        weaponManager.CurrentWeapon.canAttack = false;
        //Debug.Log(weaponManager.CurrentWeapon.weaponData.weaponName + " " + weaponManager.CurrentWeapon.canAttack);
        minerTransform.GetComponent<Miner>().Movement.MyBody.isKinematic = false;
    }

    public void AttachMiner()
    {
        minerTransform.SetParent(transform, true);
        minerTransform.localPosition = Vector2.zero;
        minerTransform.GetComponent<Collider2D>().enabled = false;
        GameManager.minerDocked = true;
        weaponManager.CurrentWeapon.canAttack = true;
        minerTransform.GetComponent<Miner>().Movement.MyBody.isKinematic = true;
    }

    #endregion

    #region Stat Changes

    public override void AdjustHealth(float value)
    {
        DomeShield domeShield = InventoryManager.instance.domeShieldUpgade;
        if (domeShield != null && domeShield.CurrentShield > 0f && domeShield.IsActive == true)
        {
            domeShield.AdjustShield(value, "Dome Shield");
            return;
        }

        Shield shield = InventoryManager.instance.shieldUpgrade;
        if (shield != null && shield.CurrentShield > 0f)
        {
            shield.AdjustShield(value, "Shield");
            return;
        }

        base.AdjustHealth(value);
    }

    #endregion

    #region Old Movement
    //private void Move()
    //{
    //    Vector2 wantedPosition = transform.right * direction.x * power * Time.fixedDeltaTime;
    //    rb.AddForce(wantedPosition, ForceMode2D.Force);
    //}

    //private void VerticalMove()
    //{
    //    Vector2 wantedPosition = transform.up * direction.y * power * Time.fixedDeltaTime;
    //    rb.AddForce(wantedPosition, ForceMode2D.Force);
    //}

    //public void GetInput()
    //{
    //    direction.x = Input.GetAxisRaw("Horizontal");
    //    direction.y = Input.GetAxisRaw("Vertical");

    //    Move();
    //    VerticalMove();
    //}

    //private void Gravity()
    //{
    //    Vector2 difference = transform.position - gravityTarget.position;
    //    rb.AddForce(-difference.normalized * gravity * (rb.mass));
    //    Debug.DrawRay(transform.position, difference.normalized, Color.red);

    //    if (autoOrient)
    //    {
    //        AutoOrient(-difference);
    //    }
    //}
    #endregion
}
