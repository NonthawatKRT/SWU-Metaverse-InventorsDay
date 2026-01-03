using System.Collections.Generic;
using PurrNet.Logging;
using UnityEngine;

namespace PurrLobby
{
    public class LobbyList : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private LobbyEntry lobbyEntryPrefab;
        [SerializeField] private Transform content;

        private void Start()
        {
            EnsureContentAssigned();
        }

        public void EnsureContentAssigned()
        {
            if (content == null)
            {
                GameObject lobbyContentObj = GameObject.Find("LobbyContent");
                if (lobbyContentObj != null)
                {
                    content = lobbyContentObj.transform;
                }
            }
        }

        public void Populate(List<Lobby> rooms)
        {
            EnsureContentAssigned();
            if (content == null) return;
            
            foreach (Transform child in content)
                Destroy(child.gameObject);
            
            foreach (var room in rooms)
            {
                var entry = Instantiate(lobbyEntryPrefab, content);
                entry.Init(room, lobbyManager);
            }
        }
    }
}
