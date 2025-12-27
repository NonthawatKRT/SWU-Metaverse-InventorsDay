using UnityEngine;
using TMPro;
using PurrNet;

public class Updatenametext : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private PlayerIdentity playerIdentity;

    private NetworkIdentity identity;

    private void OnEnable()
    {
        if (nameText == null)
            nameText = GetComponent<TMP_Text>();

        if (playerIdentity == null)
            playerIdentity = GetComponentInParent<PlayerIdentity>();

        identity = GetComponentInParent<NetworkIdentity>();

        Refresh();

        if (playerIdentity != null)
        {
            playerIdentity.OnPlayerNameChanged += HandleNameChanged;
        }
    }

    private void OnDisable()
    {
        if (playerIdentity != null)
        {
            playerIdentity.OnPlayerNameChanged -= HandleNameChanged;
        }
    }

    private void HandleNameChanged(string newName)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (playerIdentity == null || nameText == null)
            return;

        // 🔹 Hide name for local player
        if (identity != null && identity.isOwner)
        {
            nameText.gameObject.SetActive(false);
            return;
        }

        nameText.gameObject.SetActive(true);
        nameText.text = playerIdentity.PlayerName;
    }
}
