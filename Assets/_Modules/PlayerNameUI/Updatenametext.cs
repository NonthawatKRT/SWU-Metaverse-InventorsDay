using UnityEngine;
using TMPro;

public class Updatenametext : MonoBehaviour
{
	[SerializeField] private TMP_Text nameText;

	private void OnEnable()
	{
		if (nameText == null)
			nameText = GetComponent<TMP_Text>();

		Refresh();

		if (PlayerData.Instance != null)
		{
			PlayerData.Instance.OnNameChanged += HandleNameChanged;
		}
	}

	private void OnDisable()
	{
		if (PlayerData.Instance != null)
		{
			PlayerData.Instance.OnNameChanged -= HandleNameChanged;
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
		if (PlayerData.Instance != null && nameText != null && !string.IsNullOrEmpty(PlayerData.Instance.UserName))
		{
			nameText.text = PlayerData.Instance.UserName;
		}
	}
}
