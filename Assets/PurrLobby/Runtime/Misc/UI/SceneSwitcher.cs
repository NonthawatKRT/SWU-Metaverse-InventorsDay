using PurrNet;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PurrLobby
{
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        [PurrScene, SerializeField] private string nextScene;

        public void SwitchScene()
        {
            if (lobbyManager == null || !lobbyManager.CurrentLobby.IsValid)
                return;

            // 🔒 Only allow switching from the Lobby scene
            if (SceneManager.GetActiveScene().name != "LobbySample")
            {
                Debug.Log("[SceneSwitcher] Not in Lobby scene, ignoring SwitchScene()");
                return;
            }

            Debug.Log($"[SceneSwitcher] Switching from Lobby to {nextScene}");

            lobbyManager.SetLobbyStarted();
            SceneManager.LoadSceneAsync(nextScene);
        }
    }
}
