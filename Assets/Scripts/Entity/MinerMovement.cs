using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerMovement : EntityMovement
{

    [Header("Movement Properties")]
    public float verticalForce = 200f;
    public float horizontalForce = 200f;

    [Header("Required Components")]
    public BoxCollider2D myCollider;

    private Vector2 direction;

    private void Update()
    {
        GetInput();
    }


    protected override void Move()
    {
        base.Move();
        HorizontalMove();
        VerticleMove();
    }


    private void HorizontalMove()
    {
        Vector2 wantedPosition = transform.right * direction.x * horizontalForce * Time.fixedDeltaTime;
        MyBody.AddForce(wantedPosition, ForceMode2D.Force);
    }

    private void VerticleMove()
    {
        Vector2 wantedPosition = transform.up * direction.y * verticalForce * Time.fixedDeltaTime;
        MyBody.AddForce(wantedPosition, ForceMode2D.Force);
    }

    private void GetInput()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
    }

}
