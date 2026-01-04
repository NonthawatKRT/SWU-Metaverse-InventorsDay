using UnityEngine;
using UnityEngine.SceneManagement;
using PurrNet;
using PurrLobby;

public class SceneManagement : NetworkBehaviour
{
    [Header("Scene")]
    [PurrScene] public string sceneToChange;

    [Header("UI")]
    public GameObject waitingForPlayersUI;

    private LobbyManager lobby;

    private bool localConfirmed = false;

    private bool sceneChanging = false;



    // --------------------------------------------------
    // Lifecycle
    // --------------------------------------------------

    private void Awake()
    {
        lobby = FindObjectOfType<LobbyManager>();
        if (lobby == null)
        {
            Debug.LogError("[SceneManagement] LobbyManager not found!");
            return;
        }

        // 🔥 RESET READY FOR CHARACTER SELECT PHASE
        lobby.SetLocalReady(false);

        lobby.OnRoomUpdated.AddListener(OnLobbyUpdated);
        lobby.OnAllReady.AddListener(OnAllPlayersReady);
    }

    public void ConfirmCharacterSelection()
    {
        // SaveCharacterData();

        localConfirmed = true; // 🔥 important
        lobby.SetLocalReady(true);

        UpdateWaitingUI();
    }


    private bool IsSinglePlayer()
    {
        return lobby.CurrentLobby.IsValid && lobby.CurrentLobby.Members.Count == 1;
    }

    private void OnEnable()
    {
        if (lobby != null)
            lobby.SetLocalReady(false);
    }




    private void OnDestroy()
    {
        if (lobby == null) return;

        lobby.OnRoomUpdated.RemoveListener(OnLobbyUpdated);
        lobby.OnAllReady.RemoveListener(OnAllPlayersReady);
    }

    // --------------------------------------------------
    // Lobby callbacks
    // --------------------------------------------------

    private void OnLobbyUpdated(Lobby currentLobby)
    {
        if (!currentLobby.IsValid || currentLobby.Members == null)
            return;

        int total = currentLobby.Members.Count;
        int ready = currentLobby.Members.FindAll(m => m.IsReady).Count;

        Debug.Log($"[SceneManagement] Ready {ready}/{total}");

        UpdateWaitingUI();
    }

    private void UpdateWaitingUI()
    {
        if (waitingForPlayersUI == null)
            return;

        // Show ONLY if:
        // - I have confirmed
        // - Not everyone is ready yet
        bool show = localConfirmed && !AreAllPlayersReady();
        waitingForPlayersUI.SetActive(show);
    }

    private bool AreAllPlayersReady()
    {
        var lobbyData = lobby.CurrentLobby;
        if (!lobbyData.IsValid || lobbyData.Members == null)
            return false;

        return lobbyData.Members.TrueForAll(m => m.IsReady);
    }


    private void OnAllPlayersReady()
    {
        if (sceneChanging)
            return;

        Debug.Log("[SceneManagement] Lobby reports ALL READY");

        if (!isServer && !IsSinglePlayer())
            return;

        sceneChanging = true;
        ChangeSceneForAllPlayersRpc();
    }



    // --------------------------------------------------
    // Scene transition
    // --------------------------------------------------

    [ObserversRpc]
    private void ChangeSceneForAllPlayersRpc()
    {
        if (SceneManager.GetActiveScene().name == sceneToChange)
        {
            Debug.Log("[SceneManagement] Scene already loaded — ignoring");
            return;
        }

        Debug.Log("[SceneManagement] Loading scene for all players");
        SceneManager.LoadSceneAsync(sceneToChange);
    }


    // --------------------------------------------------
    // UI helpers
    // --------------------------------------------------

    public void ToggleReady()
    {
        if (lobby == null)
            return;

        lobby.ToggleLocalReady();
    }

    [ContextMenu("Force Scene Change (Server Only)")]
    private void ForceSceneChange()
    {
        if (isServer)
            ChangeSceneForAllPlayersRpc();
    }
}
