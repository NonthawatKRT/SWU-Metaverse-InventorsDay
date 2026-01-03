using UnityEngine;
using PurrLobby;

public class LobbyEventBinder : MonoBehaviour
{
    void Start()
    {
        RewireEvents();
    }

    public void RewireEvents()
    {
        var lobbyManager = FindObjectOfType<PurrLobby.LobbyManager>();
        if (lobbyManager == null) return;

        var viewManager = FindObjectOfType<ViewManager>();
        var memberList = FindObjectOfType<LobbyMemberList>();
        var lobbyList = FindObjectOfType<LobbyList>();
        var sceneSwitcher = FindObjectOfType<SceneSwitcher>();

        // Clear all listeners first
        lobbyManager.OnRoomJoined.RemoveAllListeners();
        lobbyManager.OnRoomJoinFailed.RemoveAllListeners();
        lobbyManager.OnRoomLeft.RemoveAllListeners();
        lobbyManager.OnRoomUpdated.RemoveAllListeners();
        lobbyManager.OnRoomSearchResults.RemoveAllListeners();
        lobbyManager.OnAllReady.RemoveAllListeners();

        // On Room Joined (Lobby)
        if (viewManager != null)
            lobbyManager.OnRoomJoined.AddListener(lobby => viewManager.OnRoomJoined());

        // On Room Join Failed (String)
        if (viewManager != null)
            lobbyManager.OnRoomJoinFailed.AddListener(message => viewManager.OnRoomLeft());

        // On Room Left ()
        if (memberList != null)
            lobbyManager.OnRoomLeft.AddListener(memberList.OnLobbyLeave);
        if (viewManager != null)
            lobbyManager.OnRoomLeft.AddListener(viewManager.OnRoomLeft);

        // On Room Updated (Lobby)
        if (memberList != null)
            lobbyManager.OnRoomUpdated.AddListener(memberList.LobbyDataUpdate);

        // On Room Search Results (List<Lobby>)
        if (lobbyList != null)
            lobbyManager.OnRoomSearchResults.AddListener(lobbyList.Populate);

        // On All Ready ()
        if (sceneSwitcher != null)
            lobbyManager.OnAllReady.AddListener(sceneSwitcher.SwitchScene);
            
        PurrNet.Logging.PurrLogger.Log("[LobbyEventBinder] Events rewired successfully");
    }
}