using UnityEngine;
using TMPro;

public class Updatenametext : MonoBehaviour
{
	[SerializeField] private TMP_Text nameText;
	[SerializeField] private PlayerIdentity playerIdentity;

	private void OnEnable()
	{
		if (nameText == null)
			nameText = GetComponent<TMP_Text>();
		
		if (playerIdentity == null)
			playerIdentity = GetComponentInParent<PlayerIdentity>();

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
		if (nameText != null)
		{
			nameText.text = newName;
		}
	}

	private void Refresh()
	{
		if (playerIdentity != null && nameText != null && !string.IsNullOrEmpty(playerIdentity.PlayerName))
		{
			nameText.text = playerIdentity.PlayerName;
		}
	}
}
