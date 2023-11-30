using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiningLaser : MonoBehaviour
{
    [Header("Laser Components")]
    public float maxLaserDistance = 20f;
    public LineRenderer lineRenderer;
    public float laserDamage = 1f;

    //[Header("Vacum Components")]
    //public float maxVacumDistance = 5f;
    //public float vacumWidth = 0.5f;
    ////public LineRenderer vacumLineRenderer;
    //public float vacumForce = 1f;
    //private List<RaycastHit2D> currentVacumedLoot = new List<RaycastHit2D>();

    public Transform firePoint;

    [Header("Digging Fields")]
    public LayerMask diggingMask;
    //public LayerMask lootMask;
    private Timer digTimer;
    public float digCooldown = 0.5f;
    private bool canDig;

    private void Start()
    {
        lineRenderer.enabled = false;
        digTimer = new Timer(digCooldown, OnDigCooldownFinish);
    }

    public void Update()
    {
        if (Input.GetMouseButton(0) && GameManager.minerDocked == false)
        {
            ShootLaser();
        }
        if (Input.GetMouseButtonUp(0) && GameManager.minerDocked == false)
        {
            lineRenderer.enabled = false;
        }
        if (digTimer != null && canDig == false)
        {
            digTimer.UpdateClock();
        }

        //if (Input.GetMouseButtonDown(1) && GameManager.minerDocked == false)
        //{
        //    ShootVacum();
        //}
        //if (Input.GetMouseButtonUp(1) && GameManager.minerDocked == false)
        //{
        //    lineRenderer.enabled = false;
        //    currentVacumedLoot.Clear();
        //}

        if (digTimer != null && canDig == false)
        {
            digTimer.UpdateClock();
        }

    }

    private void OnDigCooldownFinish()
    {
        canDig = true;
    }

    private void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.up, maxLaserDistance, diggingMask);
        if (hit.collider != null)
        {
            DrawLine(firePoint.position, hit.point);
            if (canDig == true)
            {
                Dig(hit);
                canDig = false;
            }

        }
        else
        {
            DrawLine(firePoint.position, firePoint.position + firePoint.up * maxLaserDistance);
        }
    }

    //private void ShootVacum()
    //{
    //    RaycastHit2D[] hits = Physics2D.CircleCastAll(firePoint.position, vacumWidth, firePoint.up, maxVacumDistance, lootMask);
    //    List<RaycastHit2D> hitDiggable = new List<RaycastHit2D>();

    //    for (int i = 0; i < hits.Length; i++)
    //    {
    //        if (LayerMask.LayerToName(hits[i].collider.gameObject.layer) == "Diggable")
    //        {
    //            hitDiggable.Add(hits[i]);
    //        }
    //        if (LayerMask.LayerToName(hits[i].collider.gameObject.layer) == "Ore")
    //        {
    //            currentVacumedLoot.Add(hits[i]);
    //        }
    //    }
    //    if (hitDiggable.Count > 0)
    //    {
    //        RaycastHit2D closestDigHit = TargetUtilities.FindNearestTarget(hitDiggable.ToArray(), firePoint);
    //        //Debug.Log(closestDigHit.gameObject.name);
    //        DrawLine(firePoint.position, closestDigHit.point);
    //    }
    //    else
    //    {
    //        DrawLine(firePoint.position, firePoint.position + firePoint.up * maxVacumDistance);
    //        Debug.Log("Hit Nothing");
    //    }
    //}


    private void DrawLine(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

    private void Dig(RaycastHit2D hit)
    {
        Tilemap tilemap = hit.collider.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            Vector3 hitPosition = Vector3.zero;

            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;

            Vector3Int tilePos = tilemap.WorldToCell(hitPosition);
            AdvancedRuleTile tile = tilemap.GetTile(tilePos) as AdvancedRuleTile;
            if (tile != null)
            {
                switch (tile.type)
                {
                    case AdvancedRuleTile.TileType.Wall:
                        return;
                    case AdvancedRuleTile.TileType.Loot:
                        
                    case AdvancedRuleTile.TileType.Dirt:
                    case AdvancedRuleTile.TileType.Door:
                        TileManager.ChangeHealthAtPos(tilePos, -laserDamage);
                        //tilemap.SetTile(tilePos, null);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
