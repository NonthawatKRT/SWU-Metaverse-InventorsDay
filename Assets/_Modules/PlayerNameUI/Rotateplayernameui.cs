using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotateplayernameui : MonoBehaviour
{
    private Camera mainCamera;

    void OnEnable()
    {
        FindMainCamera();
    }

    void LateUpdate()
    {
        if (mainCamera == null || !mainCamera.isActiveAndEnabled)
        {
            FindMainCamera();
            if (mainCamera == null)
            {
                return; // Nothing to face toward yet.
            }
        }

        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }

    private void FindMainCamera()
    {
        if (mainCamera != null && mainCamera.isActiveAndEnabled)
        {
            return;
        }

        // First try the tagged main camera.
        mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.isActiveAndEnabled)
        {
            return;
        }

        // Look for any enabled camera named or tagged MainCamera.
        foreach (var cam in Camera.allCameras)
        {
            if (!cam.isActiveAndEnabled)
            {
                continue;
            }

            if (cam.CompareTag("MainCamera") || cam.name == "MainCamera")
            {
                mainCamera = cam;
                return;
            }
        }

        // Fallback: take the first enabled camera in the scene.
        foreach (var cam in Camera.allCameras)
        {
            if (cam.isActiveAndEnabled)
            {
                mainCamera = cam;
                return;
            }
        }
    }
}
