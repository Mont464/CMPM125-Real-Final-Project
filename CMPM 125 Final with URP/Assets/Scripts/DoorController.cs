using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private bool IsOpen = false;

    public void CheckDoor()
    {
        if ( IsOpen ) { CloseDoor(); }
        else { OpenDoor(); }
    }

    private void CloseDoor()
    {
        UnityEngine.Debug.Log("DOOR CLOSED");
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, (gameObject.transform.position.y - 2.0f));
        IsOpen = false;
    }

    private void OpenDoor()
    {
        UnityEngine.Debug.Log("DOOR OPENED");
        gameObject.transform.position = new Vector2(gameObject.transform.position.x, (gameObject.transform.position.y + 2.0f));
        IsOpen = true;
    }
}
