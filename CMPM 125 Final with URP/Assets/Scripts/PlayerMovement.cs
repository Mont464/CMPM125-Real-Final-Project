using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

//Crouch code found at: https://www.youtube.com/watch?v=xCxSjgYTw9c
//IsGrounded code found at: https://www.youtube.com/watch?v=P_6W-36QfLA
//Attack code found at: https://www.youtube.com/watch?v=rwO3TE1G3ag

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Player Speed")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float crouchSpeed;
    private float moveSpeed;

    [Header("Keybinds")]
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode crouchKey = KeyCode.S;
    private KeyCode attackKey = KeyCode.J;
    private KeyCode distractKey = KeyCode.K;

    [Header("Player Size")]
    private float startScaleY = 1f;
    private float crouchScaleY = 0.5f;

    [Header("Check Ground")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask groundLayer;
    private float castDistance = 0.95f;

    [Header("Check Sides")]
    [SerializeField] private Vector2 sideSize;
    private float sideCastDistance = 0.6f;

    [Header("Attack")]
    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;

    [Header("Distract")]
    public bool facingLeft = false;
    public float distractCooldown = 5.0f;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; //Keep player upright
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(jumpKey) && IsGrounded()) //Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        if (Input.GetKeyDown(crouchKey)) //Crouch
        {
            transform.localScale = new Vector2(transform.localScale.x, crouchScaleY);
            rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
            moveSpeed = crouchSpeed;

            sideSize = sideSize / 2f;
        }

        if (Input.GetKeyUp(crouchKey)) //Crouch released
        {
            transform.localScale = new Vector2(transform.localScale.x, startScaleY);
            moveSpeed = walkSpeed;

            sideSize = sideSize * 2f;
        }

        if(Input.GetKeyDown(attackKey))
        {
            Attack();
        }
        if (Input.GetKeyDown(distractKey))
        {
            if (distractCooldown <= 0){
                Distract();
            }
        }
    }

    private void FixedUpdate()
    {
        distractCooldown -= Time.deltaTime;
        if ((Input.GetAxis("Horizontal") >= 0 && !blockedOnSide(transform.right)) || (Input.GetAxis("Horizontal") < 0 && !blockedOnSide(-transform.right)))
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y); //Left and right movement
        }
        if (Input.GetAxis("Horizontal") > 0 && facingLeft) //Flip sprite
        {
            facingLeft = false;
            //Flip();
        }
        else if (Input.GetAxis("Horizontal") < 0 && !facingLeft) //Flip sprite
        {
            facingLeft = true;
            //Flip();
        }
    }

    public bool IsGrounded()
    {
        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer)) { return true; }
        else { return false; }
    }

    public bool blockedOnSide(Vector2 transformDirection)
    {
        if (Physics2D.BoxCast(transform.position, sideSize, 0, transformDirection, sideCastDistance, groundLayer))
        {
            return true;
        }
        return false;
    }

    public void Attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);
        foreach (Collider2D enemyGameObject in enemy)
        {
            //UnityEngine.Debug.Log("Hit enemy"); //Test attack
        }
    }

    public void Distract()
    {
        distractCooldown = 5.0f;
        UnityEngine.Debug.Log("Distract"); //Test distract
        // Set coin to active, set position to player attack position
        // coin.SetActive(true);
        // coin.transform.position = attackPoint.transform.position;
    }

    private void OnDrawGizmos() //Testing
    {

        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize); //Display BoxCast
        Gizmos.DrawWireCube(transform.position + transform.right * sideCastDistance, sideSize); //Display BoxCast
        Gizmos.DrawWireCube(transform.position - transform.right * sideCastDistance, sideSize); //Display BoxCast
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize); //Display BoxCast for IsGrounded
        //Gizmos.DrawWireSphere(attackPoint.transform.position, radius); //Display attack radius
    }
}