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

    bool isGrappling = false;
    [HideInInspector] public bool retracting = false;

    Vector2 target;
    private Rigidbody2D playerRigid;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        playerRigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && !isGrappling)
        {
            StartGrapple();
        }

        if (retracting)
        {
            Vector2 grapplePos = transform.position;
            if (Vector2.Distance(transform.position, target) > endLength)
            {
                grapplePos = Vector2.Lerp(transform.position, target, grappleSpeed * Time.deltaTime);
            }

            transform.position = grapplePos;

            line.SetPosition(0, transform.position);

            if (Input.GetKeyDown(KeyCode.K))
            {
                retracting = false;
                playerRigid.gravityScale = 1.5f;
                GetComponent<PlayerMovement>().enabled = true;
                isGrappling = false;
                line.enabled = false;
            }
        }
    }

    private void StartGrapple()
    {
        GetAimDirection();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistance, grapplableMask);

        if (hit.collider != null)
        {
            isGrappling = true;
            target = hit.point;
            line.enabled = true;
            line.positionCount = 2;
            playerRigid.gravityScale = 0;
            playerRigid.velocity = Vector3.zero;
            GetComponent<PlayerMovement>().enabled = false;
            StartCoroutine(Grapple());
        }
    }

    private void GetAimDirection()
    {
        if (Input.GetKey(KeyCode.A))
        {
            endLength = 0.6f;
            direction = -transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            endLength = 0.6f;
            direction = transform.right;
            
        }
        if (Input.GetKey(KeyCode.W))
        {
            endLength = 1.1f;
            direction = transform.up;
        }
    }

    IEnumerator Grapple()
    {
        float t = 0;
        float time = 10;

        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);

        Vector2 newPos;

        for (; t < time; t += grappleShootSpeed * Time.deltaTime)
        {
            newPos = Vector2.Lerp(transform.position, target, t / time);
            line.SetPosition(0, transform.position);
            line.SetPosition(1, newPos);
            yield return null;
        }

        line.SetPosition(1, target);
        retracting = true;
    }
}
