using System;
using UnityEngine;
using PurrLobby;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }
    public string UserName;
    public LobbyManager LobbyManager;

    public event Action<string> OnNameChanged;

    private bool _isUpdatingName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EnsureLobbyManager();
        SubscribeLobbyEvents(true);
    }

    private void OnDisable()
    {
        SubscribeLobbyEvents(false);
    }

    private async void Start()
    {
        EnsureLobbyManager();
        await TryUpdateNameFromLobbyAsync();
    }

    private void EnsureLobbyManager()
    {
        if (LobbyManager == null)
        {
            LobbyManager = FindFirstObjectByType<LobbyManager>();
        }
    }

    private void SubscribeLobbyEvents(bool subscribe)
    {
        if (LobbyManager == null) return;

        if (subscribe)
        {
            LobbyManager.OnRoomUpdated.AddListener(OnLobbyUpdated);
            LobbyManager.OnRoomJoined.AddListener(OnLobbyUpdated);
        }
        else
        {
            LobbyManager.OnRoomUpdated.RemoveListener(OnLobbyUpdated);
            LobbyManager.OnRoomJoined.RemoveListener(OnLobbyUpdated);
        }
    }

    private void OnLobbyUpdated(Lobby lobby)
    {
        _ = TryUpdateNameFromLobbyAsync();
    }

    private async System.Threading.Tasks.Task TryUpdateNameFromLobbyAsync()
    {
        if (_isUpdatingName) return;
        _isUpdatingName = true;

        try
        {
            EnsureLobbyManager();
            if (LobbyManager == null) return;

            // Wait until lobby is valid and members are present
            var attempts = 0;
            while ((LobbyManager.CurrentLobby.Members == null || !LobbyManager.CurrentLobby.IsValid) && attempts < 60)
            {
                await System.Threading.Tasks.Task.Delay(200);
                attempts++;
            }

            if (LobbyManager.CurrentLobby.Members == null || !LobbyManager.CurrentLobby.IsValid) return;

            var localUserId = await LobbyManager.CurrentProvider.GetLocalUserIdAsync();
            if (string.IsNullOrEmpty(localUserId)) return;

            var user = LobbyManager.CurrentLobby.Members.Find(x => x.Id == localUserId);
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
                SetName(user.DisplayName);
            }
        }
        finally
        {
            _isUpdatingName = false;
        }
    }

    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        if (UserName == name) return;
        UserName = name;
        OnNameChanged?.Invoke(name);
    }
}
