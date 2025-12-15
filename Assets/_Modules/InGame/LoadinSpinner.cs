using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadinSpinner : MonoBehaviour
{
    public GameObject loadingspinner;

    void Start()
    {
        loadingspinner.SetActive(true);
    }

    public void HideSpinner()
    {
        loadingspinner.SetActive(false);
    }

    void Update()
    {
        if (loadingspinner.activeSelf)
        {
            loadingspinner.transform.Rotate(0f, 0f, -200f * Time.deltaTime);
        }
    }
}
