using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float sprintMultiplier = 2f;
    public float acceleration = 10f;
    public float deceleration = 10f;
    
    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public bool enableMouseLook = true;
    
    [Header("Camera Switching")]
    public Camera[] cameras;
    public KeyCode nextCameraKey = KeyCode.E;
    public KeyCode previousCameraKey = KeyCode.Q;
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 currentVelocity = Vector3.zero;
    private bool isSprinting = false;
    private int currentCameraIndex = 0;
    
    void Start()
    {
        // Don't lock cursor on start
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Initialize cameras
        if (cameras == null || cameras.Length == 0)
        {
            // If no cameras assigned, use all Camera components in children
            cameras = GetComponentsInChildren<Camera>(true);
        }
        
        // Initialize rotation values from current transform
        Vector3 currentRotation = transform.eulerAngles;
        rotationX = currentRotation.y;
        rotationY = currentRotation.x;
        
        // Enable only the first camera
        SetActiveCamera(0);
    }

    void Update()
    {
        // Camera switching
        if (Input.GetKeyDown(nextCameraKey))
        {
            SwitchToNextCamera();
        }
        
        if (Input.GetKeyDown(previousCameraKey))
        {
            SwitchToPreviousCamera();
        }
        
        // Only apply controls to the active camera
        if (cameras.Length == 0 || cameras[currentCameraIndex] == null)
            return;
        // Mouse look - hold right mouse button
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        if (enableMouseLook && Input.GetMouseButton(1))
        {
            rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);
            
            // Rotate the parent (this GameObject), not individual cameras
            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
        
        // Toggle cursor lock with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        // Sprint toggle (hold Ctrl for sprint like in Minecraft)
        isSprinting = Input.GetKey(KeyCode.LeftControl);
        float currentSpeed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        
        // Get input direction
        Vector3 inputDirection = Vector3.zero;
        
        // WASD movement (relative to parent transform direction)
        if (Input.GetKey(KeyCode.W))
            inputDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            inputDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            inputDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            inputDirection += transform.right;
        
        // Vertical movement (Space = up, Shift = down, like Minecraft creative)
        if (Input.GetKey(KeyCode.Space))
            inputDirection += Vector3.up;
        if (Input.GetKey(KeyCode.LeftShift))
            inputDirection -= Vector3.up;
        
        // Normalize to prevent faster diagonal movement
        if (inputDirection.magnitude > 1f)
            inputDirection.Normalize();
        
        // Target velocity
        Vector3 targetVelocity = inputDirection * currentSpeed;
        
        // Smooth acceleration/deceleration
        if (inputDirection.magnitude > 0.1f)
        {
            // Accelerate
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            // Decelerate
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }
        
        // Apply smooth movement to parent (this GameObject)
        transform.position += currentVelocity * Time.deltaTime;
    }
    
    void SetActiveCamera(int index)
    {
        if (cameras == null || cameras.Length == 0)
            return;
        
        // Disable all cameras
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != null)
                cameras[i].enabled = (i == index);
        }
        
        currentCameraIndex = index;
        
        // Reset rotation to current parent's rotation
        Vector3 currentRotation = transform.eulerAngles;
        rotationX = currentRotation.y;
        rotationY = currentRotation.x;
        
        // Reset velocity when switching cameras
        currentVelocity = Vector3.zero;
    }
    
    void SwitchToNextCamera()
    {
        if (cameras == null || cameras.Length <= 1)
            return;
        
        int nextIndex = (currentCameraIndex + 1) % cameras.Length;
        SetActiveCamera(nextIndex);
        Debug.Log($"Switched to camera {nextIndex + 1}/{cameras.Length}: {cameras[nextIndex].name}");
    }
    
    void SwitchToPreviousCamera()
    {
        if (cameras == null || cameras.Length <= 1)
            return;
        
        int prevIndex = currentCameraIndex - 1;
        if (prevIndex < 0)
            prevIndex = cameras.Length - 1;
        
        SetActiveCamera(prevIndex);
        Debug.Log($"Switched to camera {prevIndex + 1}/{cameras.Length}: {cameras[prevIndex].name}");
    }
}
