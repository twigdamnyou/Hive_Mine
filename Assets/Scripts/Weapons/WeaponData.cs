using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Required Components")]
    public Projectile payload;


    [Tooltip("This is an optional component")]
    //public KeyCode keyBinding;
    public Entity entityPayload;
    public string inputButton;
    public string weaponName;
    public string weaponDescription;

    [Header("Weapon Propertie")]
    [Tooltip("Is this weapon a lobed weapon")]
    public bool lobedWeapon = false;
    [Tooltip("Adjust the Weapons delay to fire again")]
    public float weaponCooldown;
    [Tooltip("Adjust the Weapons spread when fired")]
    public float weaponAccuracy = 1f;
    [Tooltip("Adjust the Weapons delay for next bullet")]
    public float shotDelay;
    public float shotCount;
    public float projectileDamage = 1f;
}
