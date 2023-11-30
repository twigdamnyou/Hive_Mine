using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MineEntrance : MonoBehaviour
{
    private Coroutine snapToMine;
    public Entity target;
    private bool Snapped;
    public TileManager mine;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && target != null && GameManager.minerDocked == true)
        {
            //Debug.Log("Attempting Snap");
            ToggleMovement();
            SnapToPoint();

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            target = collision.GetComponent<Entity>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            target = null;
        }
    }

    private void ToggleMovement()
    {
        DomeShield domeShield = InventoryManager.instance.domeShieldUpgade;
        if (domeShield != null && domeShield.IsActive == true)
           return;
        
        Snapped = !Snapped;
        mine.gameObject.SetActive(Snapped);
        target.Movement.CanMove = !Snapped;
        GameManager.ticDocked = !GameManager.ticDocked;
        target.GetComponent<Player>().Snapped = !Snapped;
        target.Movement.MyBody.isKinematic = Snapped;
        target.Movement.MyBody.velocity = Vector2.zero;
        target.Movement.MyBody.freezeRotation = Snapped;

        EventData eventData = new EventData();
        eventData.AddMineEntrance("MineEntrance", this);

        if (Snapped == true)
        {
            EventManager.SendEvent(EventManager.GameEvent.TICDocked, eventData);
        }
        else
        {
            EventManager.SendEvent(EventManager.GameEvent.TICUndocked, eventData);
        }
    }

    public void SnapToPoint()
    {
        target.transform.position = this.transform.position;
    }

}
