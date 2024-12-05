using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

//Crouch code found at: https://www.youtube.com/watch?v=xCxSjgYTw9c
//IsGrounded code found at: https://www.youtube.com/watch?v=P_6W-36QfLA
//Attack code found at: https://www.youtube.com/watch?v=rwO3TE1G3ag
//Platform effector code found at: https://www.youtube.com/watch?v=Lyeb7c0-R8c

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
    //private KeyCode attackKey = KeyCode.J;
    private KeyCode distractKey = KeyCode.K;


    [Header("Player Size")]
    private float startScaleY = 1f;
    private float crouchScaleY = 0.5f;

    [Header("Check Ground and Platform")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    private float castDistance = 0.95f;
    private Collider2D _playerCollider;

    [Header("Check Sides")]
    [SerializeField] private Vector2 sideSize;
    private float sideCastDistance = 0.6f;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    /*
    [Header("Attack")]
    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;
    */

    [Header("Distract")]
    public bool facingLeft = false;
    public float distractCooldown = 5.0f;

    [Header("Crouch")]
    public bool isCrouched = false;


    private void Start()
    {
        _playerCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; //Keep player upright
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        if (Input.GetKey(crouchKey) && Input.GetKeyDown(jumpKey) && IsGrounded(platformLayer)) //Jump down from platform
        {
            _playerCollider.enabled = false; //Lo: Disabling the player collider may make them temporarily invincible
            StartCoroutine(EnableCollider());
        }
        else if (Input.GetKeyDown(jumpKey) && (IsGrounded(groundLayer) || IsGrounded(platformLayer))) //Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            animator.SetBool("Jumping", true);
        }
        if (Input.GetKeyDown(crouchKey))
        {
            Crouch();
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            Crouch();
        }
    }

    private void FixedUpdate()
    {
        float horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;
        animator.SetFloat("Moving", Mathf.Abs(horizontalMove));

        float verticalMove = rb.velocity.y;
        if (verticalMove < 0 && (!IsGrounded(groundLayer) && !IsGrounded(platformLayer)))
        {
            animator.SetBool("Falling", true);
            animator.SetBool("Jumping", false);
        }

        else if ((IsGrounded(groundLayer) || IsGrounded(platformLayer)))
        {
            animator.SetBool("Falling", false);
            animator.SetBool("Jumping", false);
        }

        if ((Input.GetAxis("Horizontal") >= 0 && !blockedOnSide(transform.right)) || (Input.GetAxis("Horizontal") < 0 && !blockedOnSide(-transform.right)))
        {
            rb.velocity = new Vector2(horizontalMove, rb.velocity.y); //Left and right movement

            
            if (Input.GetAxis("Horizontal") > 0 && facingLeft)
            {
                Flip();
            }
            else if (Input.GetAxis("Horizontal") < 0 && !facingLeft)
            {
                Flip();
            }
            /*
            transform.localScale = new Vector2(transform.localScale.x, startScaleY);
            moveSpeed = walkSpeed;

            sideSize = sideSize * 2f;
            */
        }
        /*
        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }
        */
        if (Input.GetKeyDown(distractKey))
        {
            if (distractCooldown <= 0)
            {
                Distract();
            }
        }
    }

    private IEnumerator EnableCollider()
    {
        distractCooldown -= Time.deltaTime;
        /*
        if ((Input.GetAxis("Horizontal") >= 0 && !blockedOnSide(transform.right)) || (Input.GetAxis("Horizontal") < 0 && !blockedOnSide(-transform.right)))
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y); //Left and right movement
        }
        */
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
        yield return new WaitForSeconds(0.5f);
        _playerCollider.enabled = true;
    }

    private bool IsGrounded(LayerMask layer)
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, layer))
        {
            return true;
        }
        return false;
    }

    public bool blockedOnSide(Vector2 transformDirection)
    {
        if (Physics2D.BoxCast(transform.position, sideSize, 0, transformDirection, sideCastDistance, groundLayer))
        {
            return true;
        }
        return false;
    }

    public void Crouch()
    {
        if (isCrouched)  //Crouch released
        {
            transform.localScale = new Vector2(transform.localScale.x, startScaleY);
            moveSpeed = walkSpeed;

            sideSize = sideSize * 2f;
            isCrouched = false;
        }
        else   //Crouch
        {
            transform.localScale = new Vector2(transform.localScale.x, crouchScaleY);
            rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
            moveSpeed = crouchSpeed;

            sideSize = sideSize / 2f;
            isCrouched = true;
        }
    }

    /*
    public void Attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);
        foreach (Collider2D enemyGameObject in enemy)
        {
            EnemyAI enemyAI = enemyGameObject.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.GetStunned(3f); // Stun the enemy for 3 seconds
            }
            //UnityEngine.Debug.Log("Hit enemy"); //Test attack
        }
    }
    */

    public void Distract()
    {
        distractCooldown = 5.0f;
        UnityEngine.Debug.Log("Distract"); //Test distract
        // Set coin to active, set position to player attack position
        // coin.SetActive(true);
        // coin.transform.position = attackPoint.transform.position;
    }

    public void Flip()
    {
        if (facingLeft)
        {
            //transform.eulerAngles = new Vector3(0, 0, 0);
            GetComponent<SpriteRenderer>().flipX = false;
            facingLeft = false;
        }

        else
        {
            //transform.eulerAngles = new Vector3(0, 180, 0);
            GetComponent<SpriteRenderer>().flipX = true;
            facingLeft = true;
        }
    }

    private void OnDrawGizmos() //Testing
    {

        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize); //Display BoxCast
        Gizmos.DrawWireCube(transform.position + transform.right * sideCastDistance, sideSize); //Display BoxCast
        Gizmos.DrawWireCube(transform.position - transform.right * sideCastDistance, sideSize); //Display BoxCast
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize); //Display BoxCast for IsGrounded
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize); //Display BoxCast
        //Gizmos.DrawWireCube(transform.position + transform.right * sideCastDistance, sideSize); //Display BoxCast
        //Gizmos.DrawWireCube(transform.position - transform.right * sideCastDistance, sideSize); //Display BoxCast
        //Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize); //Display BoxCast for IsGrounded
    }
}