using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTES - Need to adjust for 2d movement, Tracking and Searching states are NYI, Raycasting is NYI
// Capturing assumes the existence of a function on a player-related script that is called when the player is captured
public class EnemyMachine : MonoBehaviour
{
    public int walkSpeed = 5;
    public int runSpeed = 10;
    public int captureRadius = 10;
    public float suspicionTimer = 5;
    public float patienceTimer = 3;
    public float searchTimer = 1;
    public enum State
    {
        Idle,
        Patrol,
        Tracking,
        Searching,
        Chasing,
    }
    public State currentState = State.Idle;
    public Transform target;
    public Transform waypoint;
    public Transform start;
    private Transform temp;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        start = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case State.Idle:
                Idle();
                break;
            case State.Patrol:
                Patrol();
                break;
            case State.Tracking:
                Tracking();
                break;
            case State.Searching:
                Searching();
                break;
            case State.Chasing:
                Chasing();
                break;
        }
    }
    void Idle()
    {
        //Do nothing (play idle animation?)
        // If location is not start, move to start
        if (Vector3.Distance(transform.position, start.position) > 1)
        {
            transform.LookAt(start);
            transform.position += transform.forward * walkSpeed * Time.deltaTime;
        }
        // If waypoint is set, change state to patrol
        if (waypoint != null)
        {
            currentState = State.Patrol;
        }
        // If player is in range, change state to tracking (done in collision handler)
    }
    void Patrol()
    {
        // Move towards waypoint
        if (waypoint != null)
        {
            if (Vector3.Distance(transform.position, waypoint.position) > 1)
            {
                transform.LookAt(waypoint);
                transform.position += transform.forward * walkSpeed * Time.deltaTime;
            }
            else
            {
                // If waypoint is reached, go back to the start, keeping the same waypoint
                temp = waypoint;
                waypoint = start;
                start = temp;
            }
        } 
        else
        {
            currentState = State.Idle;
        }
        // If player is in range, change state to tracking (done in collision handler)
    }
    void Tracking()
    {
        // Move towards target
        // If player is out of range, change state to searching
        // If player hasn't been seen for a while, change state to patrol
        // If target is reached, change state to searching
    }
    void Searching()
    {
        // Turn around, checking collisions
        // If player is visible, progress suspicionTimer
        // If suspicionTimer is up, change state to Chasing
        // If player is out of range, change state to patrol
    }
    void Chasing()
    {
        // Move towards player
        if (Vector3.Distance(transform.position, target.position) > 1)
        {
            transform.LookAt(target);
            transform.position += transform.forward * runSpeed * Time.deltaTime;
        }
        else
        {
            // Capture Player (NYI)
            currentState = State.Idle;
        }
        // If player is out of range, change state to searching
        // If player is captured, change state to idle
    }
    void OnTriggerEnter(Collider other)
    {
        if (currentState == State.Patrol)
        {
            if (other.tag == "Player" || other.tag == "Coin")
            {
                target = other.transform;
                currentState = State.Tracking;
            }
        }
        else if (currentState == State.Tracking)
        {
            if (other.tag == "Player")
            {
                target = other.transform;
                currentState = State.Chasing;
            }
        }
    }

    bool checkSurroundings()
    {
        // Use raytracing to check if player is in sight
        // If player is in sight, return true
        // If player is not in sight, return false
        return false;
    }
}
