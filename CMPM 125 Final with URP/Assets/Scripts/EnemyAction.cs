using UnityEngine;
using System.Collections;

//https://www.youtube.com/watch?v=oD7akZVgT4I&ab_channel=MinaP%C3%AAcheux
//https://www.youtube.com/watch?v=ptLg-J67vIU&list=PLSR2vNOypvs72jRSvOEWv448Tle9nDw1Z&index=4&ab_channel=NightRunStudio
//https://www.youtube.com/watch?v=hkaysu1Z-N8&ab_channel=Brackeys
//https://zzzcode.ai/
//https://openai.com/index/chatgpt/
//Help from my friend

public class EnemyAI : MonoBehaviour
{
    public Animator Enemy_Animator;
    public Animator Stun_Animator;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float catchRange = 1.5f;
    public float patienceTimer = 1.5f; // Time to search before returning to patrol
    public float suspicionTimer = 3f; // Time needed to start chasing player
    public float flipInterval = 2f;
    public float chaseTimer;
    public float stunTimer;
    public bool isStunned;
    

    public enemyVision vision;
    public Transform[] waypoints; // Waypoints for patrolling

    public Vector2 enemyTarget;
    private Vector2 playerStartPoint;   // store the start point, could be modify during the future develop
    private int currentWaypoint = 0;
    private int flipCount;
    private float timer;
    private float flipTimer;
    private Transform player;
    public enum State { Patrolling, Searching, Chasing, Tracking, Stunning }
    public State currentState = State.Patrolling;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerStartPoint = player.position;     // set the start point
        timer = patienceTimer; 
        chaseTimer = suspicionTimer;
        flipTimer = flipInterval;
        flipCount = 0;
    }

    void Update()
    {
        // States for different mode of enemy action
        switch (currentState)
        {
            case State.Patrolling:
                Patrol();
                break;
            case State.Searching:
                Search();
                break;
            case State.Chasing:
                Chase();
                break;
            case State.Tracking:
                Track();
                break;
            case State.Stunning:
                Stunned();
                break;
        }
    }

    void Patrol()
    {
        if (vision.playerInVision)
        {
            setTarget(player);
            currentState = State.Searching;
            return;
        }
        
        // Determine the target position (only updating the x-axis)
        Vector2 target = new Vector2(waypoints[currentWaypoint].position.x, transform.position.y);
        MoveTowards(target, walkSpeed);

        // Flip enemy based on the direction towards the next waypoint
        FlipTowards(target);

        // Check if the enemy is close to the target waypoint
        if (Vector2.Distance(transform.position, target) < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
        if (Vector2.Distance(transform.position, player.position) <= catchRange)
        {
            //player.position = playerStartPoint;
        }
    }

    void Search()
    {
        MoveTowards(enemyTarget, walkSpeed);
        FlipTowards(enemyTarget);
        if (vision.playerInVision)
        {
            setTarget(player);
            // If the player is in vision, reset timers and check for chasing
            chaseTimer -= Time.deltaTime;
            if (chaseTimer <= 0)
            {
                currentState = State.Chasing;
                timer = patienceTimer;
                chaseTimer = suspicionTimer;
            }
        }
        else
        {
            // Player is not in sight
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                // Timer ran out, goes to the
                timer = patienceTimer;
                chaseTimer = suspicionTimer;
                currentState = State.Tracking;
            }
        }
        /*
        if (Vector2.Distance(transform.position, enemyTarget) <= catchRange)
        {
            Debug.Log("Player caught!");
            player.transform.position = player.GetComponent<PlayerHide>().respawnPoint.position;
        }
        */
    }


    void Chase()
    {
        MoveTowards(player.position, runSpeed);
        FlipTowards(player.position);

        // If can't see player, after timer runs out, enemy will set to searching state, then reset timer
        if(vision.playerInVision == false) 
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                currentState = State.Searching;
                timer = patienceTimer;
                chaseTimer = suspicionTimer;
            }
        }

        // This funtion use for Returns player to checkpoint on contact
        if (Vector2.Distance(transform.position, player.position) <= catchRange)
        {
            Debug.Log("Player caught!");        // Could add player respawn in future development
            player.transform.position = player.GetComponent<PlayerHide>().respawnPoint.position;
        }
    }

    void Track()
    {
        rb.velocity = Vector2.zero;
        Enemy_Animator.SetFloat("Enemy_speed", 0);

        // Decrement the flip timer
        flipTimer -= Time.deltaTime;
        if (flipTimer <= 0 && vision.playerInVision == false)
        {
            Flip(); // Flip the enemy's direction
            flipTimer = flipInterval; // Reset the timer
            flipCount++; // Increment the flip counter
        }
        if (vision.playerInVision == true)
        {
            currentState = State.Searching;
        }

        // If the enemy has flipped 3 times, return to patrolling
        if (flipCount >= 3)
        {
            currentState = State.Patrolling;
            flipCount = 0; // Reset the flip counter for the next time
        }
        /*
        if (Vector2.Distance(transform.position, player.position) <= catchRange)
        {
            Debug.Log("Player caught!");        // Could add player respawn in future development
        }
        */

    }

    void Stunned()
    {
        rb.velocity = Vector2.zero; // Stop enemy movement while stunned
        Stun_Animator.SetBool("Enemy_stun", true);

        stunTimer -= Time.deltaTime; // Decrement stun timer
        if (stunTimer <= 0)
        {
            Stun_Animator.SetBool("Enemy_stun", false);
            isStunned = false;         // Enemy is no longer stunned
            stunTimer = 0;             // Reset the timer
            currentState = State.Patrolling; // Transition back to patrolling
        }
    }

    public void setTarget (Transform newTarget)
    {
        if (!vision.playerInVision)
        {
            enemyTarget = new Vector2(newTarget.position.x, transform.position.y);
            if (currentState == State.Patrolling)
            {
                currentState = State.Searching;
            }
        } else {
            enemyTarget = new Vector2(newTarget.position.x, transform.position.y);
        }
    }
    public void GetStunned(float stunDuration)
    {
        if (currentState == State.Searching || currentState == State.Tracking || currentState == State.Patrolling)
        {
            currentState = State.Stunning; // Change state to Stunned
            stunTimer = stunDuration;     // Set the timer
            isStunned = true;             // Enemy is now stunned
        }
    }

    // Function that use for enemy move to target (player)
    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
        Enemy_Animator.SetFloat("Enemy_speed", rb.velocity.magnitude);
    }

    // Function to flip the enemy to face the target (player or waypoint)
    void FlipTowards(Vector2 target)
    {
        float direction = target.x - transform.position.x;
        if (direction > 0 && transform.localScale.x < 0) // Player/waypoint is to the right and enemy is flipped left
        {
            Flip();
        }
        else if (direction < 0 && transform.localScale.x > 0) // Player/waypoint is to the left and enemy is flipped right
        {
            Flip();
        }
    }

    // Function to flip the enemy sprite
    void Flip()
    {
        Vector2 scale = transform.localScale;
        scale.x *= -1; // Invert the x-axis scale to flip the sprite
        transform.localScale = scale;
    }

}