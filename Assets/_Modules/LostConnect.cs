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
    private Coroutine waitingToExitCoroutine = null;

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
            // If not already waiting to exit, start the delay
            if (waitingToExitCoroutine == null)
            {
                PurrNet.Logging.PurrLogger.LogWarning("[LostConnect] No cameras found in scene - waiting 2 seconds before exiting lobby");
                waitingToExitCoroutine = StartCoroutine(WaitBeforeExit());
            }
        }
        else
        {
            // Cameras are back, cancel the exit if we were waiting
            if (waitingToExitCoroutine != null)
            {
                PurrNet.Logging.PurrLogger.Log("[LostConnect] Cameras detected again - cancelling exit");
                StopCoroutine(waitingToExitCoroutine);
                waitingToExitCoroutine = null;
            }
        }
    }

    private IEnumerator WaitBeforeExit()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Check again if cameras are still missing
        Camera[] cameras = FindObjectsOfType<Camera>();
        if (cameras.Length == 0)
        {
            PurrNet.Logging.PurrLogger.LogWarning("[LostConnect] Still no cameras after 2 seconds - exiting lobby");
            Exitlobby();
        }
        
        waitingToExitCoroutine = null;
    }

    public void Exitlobby()
    {
        if (hasExited) return;
        hasExited = true;
        
        StartCoroutine(ExitLobbyRoutine());
    }

    private IEnumerator ExitLobbyRoutine()
    {
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
