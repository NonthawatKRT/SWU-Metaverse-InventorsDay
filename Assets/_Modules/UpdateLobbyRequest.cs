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
        var eventBinder = FindObjectOfType<LobbyEventBinder>();
        var browseView = FindObjectOfType<BrowseView>();
        var lobbyView = FindObjectOfType<LobbyView>();
        
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
        
        // Rewire all event listeners
        if (eventBinder != null)
        {
            eventBinder.RewireEvents();
        }
        
        // Rebind scene objects in views
        if (browseView != null)
        {
            browseView.RebindSceneObjects();
        }
        
        if (lobbyView != null)
        {
            lobbyView.RebindSceneObjects();
        }
        
        PurrNet.Logging.PurrLogger.Log("[UpdateLobbyRequest] Scene objects reassigned and events rewired");
    }
}
