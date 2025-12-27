using UnityEngine;

namespace PurrLobby
{
    public class LobbyView : View
    {
        [SerializeField] private CodeButton codeButton;
        [SerializeField] private LobbyManager lobbyManager;

        protected override void Awake()
        {
            base.Awake();
            
            if (lobbyManager == null)
            {
                lobbyManager = FindObjectOfType<LobbyManager>();
            }
        }

        public override void OnShow()
        {
            codeButton.Init(lobbyManager.CurrentLobby.LobbyId);
        }
    }
}
