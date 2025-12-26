using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PurrLobby;

public class UpdateLobbyRequest : MonoBehaviour
{
    void Start()
    {
        // Find all lobby-related components in DontDestroyOnLoad
        var lobbyManager = FindObjectOfType<LobbyManager>();
        var memberList = FindObjectOfType<LobbyMemberList>();
        var lobbyList = FindObjectOfType<LobbyList>();
        var viewManager = FindObjectOfType<ViewManager>();
        
        // Force reassignment of content references
        if (memberList != null)
        {
            memberList.EnsureContentAssigned();
        }
        
        if (lobbyList != null)
        {
            lobbyList.EnsureContentAssigned();
        }
        
        // Force reassignment of view references
        if (viewManager != null)
        {
            viewManager.EnsureViewsAssigned();
        }
    }
}
