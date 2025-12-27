using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PurrLobby;
public class LostConnect : MonoBehaviour
{
    private LobbyManager lobbyManager;
    [SerializeField] private GameObject lostconnectui;
    [SerializeField] private GameObject lostconnectcamera;
    
    private bool hasExited = false;

    private void Awake()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        if (lobbyManager != null && lobbyManager.gameObject.scene.name == null)
        {
            // The LobbyManager is DontDestroyOnLoad
        }
    }

    private void Update()
    {
        if (hasExited) return;

        // Check if there are any cameras in the scene
        Camera[] cameras = FindObjectsOfType<Camera>();
        
        if (cameras.Length == 0)
        {
            PurrNet.Logging.PurrLogger.LogWarning("[LostConnect] No cameras found in scene - exiting lobby");
            Exitlobby();
        }
    }

    public void Exitlobby()
    {
        if (hasExited) return;
        hasExited = true;
        
        StartCoroutine(ExitLobbyRoutine());
    }

    private IEnumerator ExitLobbyRoutine()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Enable UI and camera
        if (lostconnectui != null)
            lostconnectui.SetActive(true);
        if (lostconnectcamera != null)
            lostconnectcamera.SetActive(true);
        
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Leave lobby and change scene
        if (lobbyManager != null)
        {
            lobbyManager.LeaveLobby();
        }
        SceneManager.LoadScene("LobbySample");
    }
}
