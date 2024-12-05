using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//from Muddy Wolf's tutorial on making a grapple hook: https://www.youtube.com/watch?v=idiq5WjCAD8 , https://github.com/TylerPottsDev/yt-unity-2d-topdown-grapple/blob/master/Assets/GrappleHook.cs
public class GrappleHook : MonoBehaviour
{
    LineRenderer line;

    [SerializeField] LayerMask grapplableMask;
    [SerializeField] float maxDistance = 10f;
    [SerializeField] float grappleSpeed = 10f;
    [SerializeField] float grappleShootSpeed = 40f;
    float endLength = 0.5f;
    Vector3 direction;
    [SerializeField] GameObject grappleEnd;
    bool isGrappling = false;
    [HideInInspector] public bool retracting = false;

    Vector2 target;
    private Rigidbody2D playerRigid;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        playerRigid = GetComponent<Rigidbody2D>();
        grappleEnd.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isGrappling)
        {
            StartGrapple();
        }

        if (retracting) //move player towards the end point
        {
            Vector2 grapplePos = transform.position;
            if (Vector2.Distance(transform.position, target) > endLength)
            {
                grapplePos = Vector2.Lerp(transform.position, target, grappleSpeed * Time.deltaTime); 
            }

            transform.position = grapplePos;

            line.SetPosition(0, transform.position);

            if (Input.GetKeyDown(KeyCode.K)) //end grapple
            {
                retracting = false;
                playerRigid.gravityScale = 3f;
                GetComponent<PlayerMovement>().enabled = true;
                isGrappling = false;
                line.enabled = false;
                grappleEnd.SetActive(false);
            }
        }
    }

    private void StartGrapple()
    {
        GetAimDirection();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask); // check if grapple is possible

        if (hit.collider != null)
        {
            isGrappling = true; //restrict actions available while grappling
            target = hit.point; 
            line.enabled = true;
            line.positionCount = 2;
            playerRigid.gravityScale = 0;       //make sure player does not fall during grapple
            playerRigid.velocity = Vector3.zero;//stop player momentum
            GetComponent<PlayerMovement>().enabled = false; //make player unable to do normal movement during grapple
            grappleEnd.SetActive(true);
            StartCoroutine(Grapple());
        }
    }

    private void GetAimDirection() //check which way the player wants to grapple
    {
        if (Input.GetKey(KeyCode.A))
        {
            endLength = 0.6f;
            direction = -transform.right;
            grappleEnd.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            endLength = 0.6f;
            direction = transform.right;
            grappleEnd.transform.eulerAngles = new Vector3(0, 0, 0);

        }
        if (Input.GetKey(KeyCode.W))
        {
            endLength = 1.1f;
            direction = transform.up;
            grappleEnd.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        //Debug.Log(grappleEnd.transform.up);
    }

    IEnumerator Grapple()
    {
        float t = 0;
        float time = 10;

        //set initial line location
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 newPos;
        //grow grapple towards the end point
        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            newPos = Vector2.Lerp(transform.position, target, t / time);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);
            grappleEnd.transform.position = newPos;
            yield return null;
        }

        line.SetPosition(1, target);
        grappleEnd.transform.position = target;
        retracting = true; //end point reached, begin retract
    }
}
