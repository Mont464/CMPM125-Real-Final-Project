using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinCode : MonoBehaviour
{
    // NOTE: CoinCode relies on a bool facingLeft in PlayerMovement.cs to determine the direction of the player
    // NOTE: CoinCode relies on a function in the enemy control script to set the target of the enemy to the coin

    private bool playerFacingLeft = false;
    public int coinTimer = 3;
    public float throwForce = 10.0f;
    private float castDistance = 0.95f;
    private Vector2 boxSize;
    private LayerMask groundLayer;   
    public GameObject Player;
    private Rigidbody2D rb;

    void Awake()
    {
        Player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
    }
    void OnEnable()
    {
        playerFacingLeft = Player.GetComponent<PlayerMovement>().facingLeft;
        boxSize = gameObject.GetComponent<BoxCollider2D>().size;
        groundLayer = LayerMask.GetMask("Ground");
        if (playerFacingLeft)
        {
            Vector2 throwDirection = new Vector2(-1, 1);
            rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        }
        else
        {
            Vector2 throwDirection = new Vector2(1, 1);
            rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        }
    }
    void FixedUpdate()
    {
        if(IsGrounded()) 
        {
            coinTimer--;
            if(coinTimer <= 0)
            {
                gameObject.SetActive(false);
            }
        }   
    }
    // If coin is on ground and an enemy is nearby, set target to coin
    public void OnTriggerStay(Collider other)
    {
        if(IsGrounded() && other.tag == "Enemy")
        {
            //other.GetComponent<EnemyAction>().setTarget(transform);
        }
    }
    public bool IsGrounded()
    {
        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer)) { return true; }
        else { return false; }
    }
}
