using UnityEngine;
using PurrLobby;

public class LobbyEventBinder : MonoBehaviour
{
    void Start()
    {
        var lobbyManager = FindObjectOfType<PurrLobby.LobbyManager>();
        if (lobbyManager == null) return;

        var viewManager = FindObjectOfType<ViewManager>();
        var memberList = FindObjectOfType<LobbyMemberList>();
        var lobbyList = FindObjectOfType<LobbyList>();
        var sceneSwitcher = FindObjectOfType<SceneSwitcher>();

        // On Room Joined (Lobby)
        lobbyManager.OnRoomJoined.RemoveAllListeners();
        if (viewManager != null)
            lobbyManager.OnRoomJoined.AddListener(lobby => viewManager.OnRoomJoined());

        // On Room Join Failed (String)
        lobbyManager.OnRoomJoinFailed.RemoveAllListeners();
        if (viewManager != null)
            lobbyManager.OnRoomJoinFailed.AddListener(message => viewManager.OnRoomLeft());

        // On Room Left ()
        lobbyManager.OnRoomLeft.RemoveAllListeners();
        if (memberList != null)
            lobbyManager.OnRoomLeft.AddListener(memberList.OnLobbyLeave);
        if (viewManager != null)
            lobbyManager.OnRoomLeft.AddListener(viewManager.OnRoomLeft);

        // On Room Updated (Lobby)
        lobbyManager.OnRoomUpdated.RemoveAllListeners();
        if (memberList != null)
            lobbyManager.OnRoomUpdated.AddListener(memberList.LobbyDataUpdate);

        // On Player List Updated (List<LobbyUser>)
        lobbyManager.OnPlayerListUpdated.RemoveAllListeners();
        // Add listeners as needed

        // On Room Search Results (List<Lobby>)
        lobbyManager.OnRoomSearchResults.RemoveAllListeners();
        if (lobbyList != null)
            lobbyManager.OnRoomSearchResults.AddListener(lobbyList.Populate);

        // On Friend List Pulled (List<FriendUser>)
        lobbyManager.OnFriendListPulled.RemoveAllListeners();
        // Add listeners as needed

        // On All Ready ()
        lobbyManager.OnAllReady.RemoveAllListeners();
        if (sceneSwitcher != null)
            lobbyManager.OnAllReady.AddListener(sceneSwitcher.SwitchScene);
    }
}