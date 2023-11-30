using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public List<Weapon> weapons = new List<Weapon>();

    private Entity owner;


    public Weapon CurrentWeapon { get; private set; }
    private int currentWeaponIndex;

    private Timer swapTimer;

    private bool canSwap = true;

    private void Awake()
    {
        owner = GetComponent<Entity>();
        weapons = GetComponentsInChildren<Weapon>().ToList();
        swapTimer = new Timer(0.5f, OnSwapTimerFinished);

        SetUpWeapon();

        if (weapons.Count < 1)
        {
            Debug.LogError(owner.gameObject.name + " has no Weapons Assigned");
        }
        else
        {
            currentWeaponIndex = 0;
            CurrentWeapon = weapons[currentWeaponIndex];
        }
    }

    private void Update()
    {
        if (swapTimer != null && canSwap == false)
        {
            swapTimer.UpdateClock();
        }
        if (owner.entityType == Entity.EntityType.Player && Input.GetButton(CurrentWeapon.weaponData.inputButton) == true)
        {
            FireCurrentWeapon();
        }

        SwitchActiveWeapon();
    }

    private void SetUpWeapon()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].SetUp();
        }
    }

    private void SwitchActiveWeapon()
    {
        if (owner.entityType != Entity.EntityType.Player)
        {
            return;
        }

        float scrollDirection = Input.mouseScrollDelta.y;

        if (scrollDirection != 0f)
        {
            if (canSwap == false || weapons.Count == 1)
            {
                return;
            }

            if (scrollDirection < 0f)
            {
                if (currentWeaponIndex == 0)
                {
                    currentWeaponIndex = weapons.Count - 1;
                    CurrentWeapon = weapons[currentWeaponIndex];
                }
                else
                {
                    currentWeaponIndex -= 1;
                    CurrentWeapon = weapons[currentWeaponIndex];
                }
            }

            if (scrollDirection > 0f)
            {
                if (currentWeaponIndex == weapons.Count - 1)
                {
                    currentWeaponIndex = 0;
                    CurrentWeapon = weapons[currentWeaponIndex];
                }
                else
                {
                    currentWeaponIndex += 1;
                    CurrentWeapon = weapons[currentWeaponIndex];
                }
            }

            canSwap = false;
        }
    }

    private void OnSwapTimerFinished()
    {

        canSwap = true;
    }

    public void FireCurrentWeapon()
    {
        //Debug.Log(CurrentWeapon.canAttack);
        if (CurrentWeapon.canAttack == true)
        {
            if (CurrentWeapon.weaponData.payload != null)
            {
                StartCoroutine(CurrentWeapon.FireWithDelay());
                CurrentWeapon.canAttack = false;
            }
        }
    }

    public void AddWeapon(Weapon weapon)
    {
        if (weapons.Contains(weapon) == false)
        {
            weapons.Add(weapon);
        }
    }

    public void RemoveWeapon(Weapon weapon)
    {
        if (weapons.Contains(weapon) == true)
        {
            weapons.Remove(weapon);
        }
    }
}
