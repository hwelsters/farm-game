using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestBuilding : MonoBehaviour
{
    private bool playerInRange = false;

    void OnTriggerOver2D (Collider2D other)
    {
        if (other.CompareTag("TileAim")) playerInRange = true;
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (other.CompareTag("TileAim")) playerInRange = false;
    }

    void Update()
    {
        if (playerInRange)
        {
            //if (Input.GetMouseButtonDown(1)) ChestUIM
        }
    }
}
