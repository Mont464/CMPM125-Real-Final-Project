using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//Attack code found at: https://www.youtube.com/watch?v=rwO3TE1G3ag

public class PlayerHide : MonoBehaviour
{
    private Rigidbody2D rb;

    public Transform respawnPoint;

    public Canvas canvas;

    [Header("Keybinds")]
    private KeyCode attackKey = KeyCode.J;
    private KeyCode interactKey = KeyCode.E;

    [Header("Attack")]
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemies;

    [Header("Interact")]
    [SerializeField] private GameObject interactPoint;
    [SerializeField] private float interactRadius;
    [SerializeField] private LayerMask objects;

    [Header("Hide")]
    private bool isHidden = false;
    private Vector2 prevPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(attackKey))
        {
            Attack();
            //gameObject.transform.position = gameObject.GetComponent<PlayerHide>().respawnPoint.position;
        }
        else if (Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }
    public void Hide(Vector2 hidePosition) //Hide behind object
    {
        if (!isHidden)
        {
            if (GetComponent<PlayerMovement>().isCrouched) { return; }  //Player can only hide if not crouched
            rb.velocity = new Vector2(0, 0); //Stop player velocity
            prevPosition = transform.position; //Save player location
            gameObject.GetComponent<PlayerMovement>().Crouch();
            transform.position = hidePosition; //Move player behind object
            isHidden = true;
            GetComponent<PlayerMovement>().enabled = false; //Disable player movement
            
        } else {
            gameObject.GetComponent<PlayerMovement>().Crouch(); //Player stands up
            transform.position = prevPosition; //Revert player to original position
            isHidden = false;
            GetComponent<PlayerMovement>().enabled = true;
        }       
    }

    private void Attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, enemies);
        foreach (Collider2D enemyGameObject in enemy)
        {
            //UnityEngine.Debug.Log("Hit enemy"); //Test
        }
    }

 
    public void Interact()  //Check objects in range
    {
        Collider2D[] obj = Physics2D.OverlapCircleAll(interactPoint.transform.position, interactRadius, objects);
        foreach (Collider2D objGameObject in obj)
        {
            if (objGameObject.gameObject.name == "Door") //Lo: Could turn this into a switch statement
            {
                objGameObject.gameObject.GetComponent<DoorController>().CheckDoor();
            }
            else if (objGameObject.gameObject.name == "Crate" || objGameObject.gameObject.name == "Chest")
            {
                gameObject.GetComponent<PlayerHide>().Hide(objGameObject.transform.position);
            }
            else if (objGameObject.gameObject.tag == "Objective")
            {
                canvas.GetComponent<ObjectiveHandler>().completeObjective(objGameObject.gameObject);
            }
            else if(objGameObject.gameObject.name == "RespawnCandle")
            {
                respawnPoint = objGameObject.gameObject.transform;
                Transform current = objGameObject.gameObject.transform.GetChild(0).GetChild(0); //Get Light2D component from nested object
                current.GetComponent<Light2D>().intensity = 1.5f;
            }
        }
    }

    private void OnDrawGizmos() //Testing
    {
        //Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius); //Display attack radius
        Gizmos.DrawWireSphere(interactPoint.transform.position, interactRadius); //Display interact radius
    }
}
