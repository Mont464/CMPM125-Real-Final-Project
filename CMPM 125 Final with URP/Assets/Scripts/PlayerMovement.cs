using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    private KeyCode attackKey = KeyCode.J;

    [Header("Player Size")]
    private float startScaleY = 1f;
    private float crouchScaleY = 0.5f;

    [Header("Check Ground and Platform")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    private float castDistance = 0.95f;
    private Collider2D _collider;

    [Header("Check Sides")]
    [SerializeField] private Vector2 sideSize;
    private float sideCastDistance = 0.6f;

    [Header("Attack")]
    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
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
            _collider.enabled = false; //Lo: Disabling the player collider may make them temporarily invincible
            StartCoroutine(EnableCollider());
        }

         else if (Input.GetKeyDown(jumpKey) && (IsGrounded(groundLayer) || IsGrounded(platformLayer))) //Jump
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
    }

    private void FixedUpdate()
    {
        if ((Input.GetAxis("Horizontal") >= 0 && !blockedOnSide(transform.right)) || (Input.GetAxis("Horizontal") < 0 && !blockedOnSide(-transform.right)))
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y); //Left and right movement
        }
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        _collider.enabled = true;
    }

    public bool IsGrounded(LayerMask layer)
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, layer)) {
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

    public void Attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);
        foreach (Collider2D enemyGameObject in enemy)
        {
            //UnityEngine.Debug.Log("Hit enemy"); //Test attack
        }
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
