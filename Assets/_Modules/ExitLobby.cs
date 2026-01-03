using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PurrLobby;
using UnityEngine.SceneManagement;

public class ExitLobby : MonoBehaviour
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
    public void Exitlobby()
    {
        lobbyManager.LeaveLobby();
        SceneManager.LoadScene("LobbySample");
    }
}
