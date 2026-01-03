using System.Collections;
using System.Collections.Generic;
using PurrLobby;
using UnityEngine;

public class FindLobbyManager : MonoBehaviour
{
    private LobbyManager lobbyManager;

    private void Awake()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        if (lobbyManager != null && lobbyManager.gameObject.scene.name == null)
        {
            // The LobbyManager is DontDestroyOnLoad
        }
    }

    public void teststirng()
    {
        Debug.Log("found lobby manager: " + (lobbyManager != null));
    }
    public void createroomrequest()
    {
        lobbyManager.CreateRoom();
    }

    public void togglereadyrequest()
    {
        lobbyManager.ToggleLocalReady();
    }
    public void leavelobbyrequest()
    {
        lobbyManager.LeaveLobby();
    }
}
