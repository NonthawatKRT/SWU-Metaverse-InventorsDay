using UnityEngine;

namespace PurrLobby
{
    public class BrowseView : View
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private LobbyList lobbyList;

        private bool _isActive;
        private float _lastSearchTime;
        
        public void RebindSceneObjects()
        {
            if (lobbyManager == null)
            {
                lobbyManager = FindObjectOfType<LobbyManager>();
            }
            
            // Find LobbyList by GameObject name in scene
            GameObject lobbyListObj = GameObject.Find("LobbyList");
            if (lobbyListObj != null)
            {
                lobbyList = lobbyListObj.GetComponent<LobbyList>();
            }
            
            PurrNet.Logging.PurrLogger.Log("[BrowseView] Rebinding complete - LobbyManager: " + (lobbyManager != null) + ", LobbyList: " + (lobbyList != null));
        }
        
        public override void OnShow()
        {
            RebindSceneObjects();
            
            if (lobbyManager != null)
            {
                lobbyManager.SearchLobbies();
                _lastSearchTime = Time.time;
            }
            _isActive = true;
        }

        public override void OnHide()
        {
            _isActive = false;
        }

        private void Update()
        {
            if(!_isActive)
                return;

            if (_lastSearchTime + 5f < Time.time)
            {
                _lastSearchTime = Time.time;
                lobbyManager.SearchLobbies();
            }
        }
    }
}
