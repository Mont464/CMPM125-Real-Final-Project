using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyVision : MonoBehaviour
{
  public bool playerInVision;
  void Start()
  {
    playerInVision = false;
  }

  public void  OnTriggerEnter2D(Collider2D other) {
    if(other.gameObject.CompareTag("Player")) {
      playerInVision = true;
    }
  }

  public void  OnTriggerExit2D(Collider2D other) {
    if(other.gameObject.CompareTag("Player")) {
      playerInVision = false;
    }
  }
}
