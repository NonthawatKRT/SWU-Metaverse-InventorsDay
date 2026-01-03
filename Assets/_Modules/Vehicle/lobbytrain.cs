using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lobbytrain : MonoBehaviour
{
    public GameObject train;
    public List<Transform> stations;
    public float speed = 5f;
    private int currentStationIndex = 0;
    private Transform targetStation;
    private bool movingForward = true;

    void Start()
    {
        if (stations.Count > 0)
        {
            targetStation = stations[currentStationIndex];
        }
    }
    void Update()
    {
        if (targetStation != null)
        {
            float step = speed * Time.deltaTime;
            train.transform.position = Vector3.MoveTowards(train.transform.position, targetStation.position, step);

            if (Vector3.Distance(train.transform.position, targetStation.position) < 0.1f)
            {
                // Reached current station, move to next
                if (movingForward)
                {
                    currentStationIndex++;
                    if (currentStationIndex >= stations.Count - 1)
                    {
                        currentStationIndex = stations.Count - 1;
                        movingForward = false;
                    }
                }
                else
                {
                    currentStationIndex--;
                    if (currentStationIndex <= 0)
                    {
                        currentStationIndex = 0;
                        movingForward = true;
                    }
                }
                targetStation = stations[currentStationIndex];
            }
        }
    }
}
