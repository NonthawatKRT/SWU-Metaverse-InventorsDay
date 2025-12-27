using UnityEngine;

namespace PurrLobby
{
    public class LobbyView : View
    {
        [SerializeField] private CodeButton codeButton;
        [SerializeField] private LobbyManager lobbyManager;

        public void RebindSceneObjects()
        {
            if (lobbyManager == null)
            {
                lobbyManager = FindObjectOfType<LobbyManager>();
            }
            
            // Find Code button by GameObject name in scene
            GameObject codeObj = GameObject.Find("Code");
            if (codeObj != null)
            {
                codeButton = codeObj.GetComponent<CodeButton>();
            }
            
            PurrNet.Logging.PurrLogger.Log("[LobbyView] Rebinding complete - LobbyManager: " + (lobbyManager != null) + ", CodeButton: " + (codeButton != null));
        }

        public override void OnShow()
        {
            RebindSceneObjects();
            
            if (lobbyManager != null && codeButton != null)
            {
                codeButton.Init(lobbyManager.CurrentLobby.LobbyId);
            }
        }
    }
}
