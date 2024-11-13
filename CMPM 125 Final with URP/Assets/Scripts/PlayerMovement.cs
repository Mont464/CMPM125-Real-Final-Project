using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

//Crouch code found at: https://www.youtube.com/watch?v=xCxSjgYTw9c
//isGrounded code found at: https://www.youtube.com/watch?v=P_6W-36QfLA

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float crouchSpeed;
    private float moveSpeed;

    [Header("Keybinds")]
    private KeyCode jumpKey = KeyCode.Space;
    private KeyCode crouchKey = KeyCode.S;

    private float startScaleY = 1f;
    private float crouchScaleY = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; //Keeps player upright
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, rb.velocity.y); //Left and right movement

        if (Input.GetKeyDown(jumpKey)) //Jump
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        if (Input.GetKeyDown(crouchKey)) //Crouch
        {
            transform.localScale = new Vector2(transform.localScale.x, crouchScaleY);
            rb.AddForce(Vector2.down * 5f, ForceMode2D.Impulse);
            moveSpeed = crouchSpeed;
        }

        if (Input.GetKeyUp(crouchKey)) //Crouch released
        {
            transform.localScale = new Vector2(transform.localScale.x, startScaleY);
            moveSpeed = walkSpeed;
        }
    }
}
