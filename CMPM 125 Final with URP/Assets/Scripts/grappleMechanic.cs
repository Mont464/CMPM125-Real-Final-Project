using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refrenced to ThatOneUnityDev's video on 2D Grappling hooks: https://www.youtube.com/watch?v=Gx46xUgVXrQ
public class grappleMechanic : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject grapple;
    [SerializeField] private LayerMask grapplableLayer;
    [SerializeField] private LineRenderer line;
    [SerializeField] private float grappleRange;        //how far the grapple can shoot
    private float grappleLength;                        //how much is left when the player reaches the point
    private bool grappleActive;
    private Vector3 grappleDirection;
    private Vector3 grapplePoint;
    private Rigidbody2D playerRigid;

    private DistanceJoint2D joint;
    void Start()
    {
        playerRigid = GetComponent<Rigidbody2D>();
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        line.enabled = false;
        grappleActive = false;

    }

    // Update is called once per frame
    void Update()
    {
        fireGrapple();
    }

    public void fireGrapple()
    {
        getAimDirection();

        if (Input.GetKeyDown(KeyCode.K))
        {
            RaycastHit2D hit = Physics2D.Raycast(origin: grapple.transform.position, direction: grappleDirection, distance: grappleRange, layerMask: grapplableLayer);

            if (hit.collider != null)
            {
                grappleActive = true;
                playerRigid.gravityScale = 0;
                playerRigid.velocity = Vector3.zero;
                grapplePoint = hit.point;
                grapplePoint.z = 0;
                joint.connectedAnchor = grapplePoint;
                joint.enabled = true;
                joint.distance = grappleLength;

                line.enabled = true;
                line.SetPosition(0, grapplePoint);
                line.SetPosition(1, transform.position);
            }
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            grappleActive = false;
            playerRigid.gravityScale = 1.5f;
            joint.enabled = false;
            line.enabled = false;
        }

        if (line.enabled)
        {
            line.SetPosition(1, transform.position);
        }
    }

    private void getAimDirection()
    {
        if (Input.GetKey(KeyCode.A))
        {
            grappleDirection = -grapple.transform.right;
            if (!grappleActive)
            {
                grappleLength = 0.5f;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            grappleDirection = grapple.transform.right;
            if (!grappleActive)
            {
                grappleLength = 0.5f;
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            grappleDirection = grapple.transform.up;
            if (!grappleActive)
            {
                grappleLength = 1f;
            }
        }
    }
}
