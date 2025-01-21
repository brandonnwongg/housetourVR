using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    public Animator doorAnimator; // Reference to the Animator component
    public string openDoorTrigger = "FrontDoorOpen"; // Name of the trigger in the Animator
    public string closeDoorTrigger = "FrontDoorClose"; // Name of the trigger in the Animator
    public bool isAutomatic = true; // Automatically close the door when the player exits

    private bool isDoorOpen = false; // Track the door's state

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player or relevant object entered the trigger zone
        if (other.CompareTag("Player") && !isDoorOpen)
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player or relevant object exited the trigger zone
        if (other.CompareTag("Player") && isDoorOpen && isAutomatic)
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        isDoorOpen = true;
        doorAnimator.SetTrigger(openDoorTrigger); // Trigger the open door animation
    }

    public void CloseDoor()
    {
        isDoorOpen = false;
        doorAnimator.SetTrigger(closeDoorTrigger); // Trigger the close door animation
    }
}
