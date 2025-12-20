using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    public Transform doorFrame;            // The door frame transform (parent)
    public Transform door1;                // First door transform
    public Transform door2;                // Second door transform
    public Transform door1StopPoint;       // Stop point for the first door
    public Transform door2StopPoint;       // Stop point for the second door
    public float doorSpeed = 2f;           // Speed of the door opening and closing

    private Vector3 door1OriginalPos;      // Original position of door1 (relative to door frame)
    private Vector3 door2OriginalPos;      // Original position of door2 (relative to door frame)

    private int playersInTrigger = 0;      // Track how many players are in the trigger

    private void Start()
    {
        door1OriginalPos = door1.localPosition;
        door2OriginalPos = door2.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInTrigger = Mathf.Max(0, playersInTrigger - 1); // Prevent going below 0
        }
    }

    private void Update()
    {
        if (playersInTrigger > 0)
        {
            door1.localPosition = Vector3.MoveTowards(door1.localPosition, door1StopPoint.localPosition, doorSpeed * Time.deltaTime);
            door2.localPosition = Vector3.MoveTowards(door2.localPosition, door2StopPoint.localPosition, doorSpeed * Time.deltaTime);
        }
        else
        {
            door1.localPosition = Vector3.MoveTowards(door1.localPosition, door1OriginalPos, doorSpeed * Time.deltaTime);
            door2.localPosition = Vector3.MoveTowards(door2.localPosition, door2OriginalPos, doorSpeed * Time.deltaTime);
        }
    }
}
