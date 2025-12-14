using System.Collections;
using UnityEngine;// FishNet NetworkBehaviour

public class SkyTrainMovement : MonoBehaviour
{
    public Transform[] checkpoints; // Assign the square checkpoints
    public float speed = 5f; // Train movement speed
    public GameObject[] detailObjects; // Objects to hide/show when rotating
    private int currentCheckpointIndex = 0;

    private void Update()
    {
        //if (!IsServer) return; // Server-authoritative movement
        MoveTrain();
    }

    private void MoveTrain()
    {
        if (checkpoints.Length == 0) return;

        Transform targetCheckpoint = checkpoints[currentCheckpointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetCheckpoint.position, speed * Time.deltaTime);

        // Rotate towards the next checkpoint with +90 Y offset
        Vector3 direction = targetCheckpoint.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1f);
        }

        // Check if train reached the checkpoint
        if (Vector3.Distance(transform.position, targetCheckpoint.position) < 0.1f)
        {
            currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Length;

            RpcHideDetails(true); // Hide details for all clients
            StartCoroutine(ShowDetailsAfterDelay(9f)); // Schedule reveal
        }
    }

    //[ObserversRpc]
    private void RpcHideDetails(bool hide)
    {
        foreach (GameObject detail in detailObjects)
        {
            if (detail != null)
                detail.SetActive(!hide); // Toggle visibility
        }
    }

    private IEnumerator ShowDetailsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RpcHideDetails(false); // Show details again
    }
}
