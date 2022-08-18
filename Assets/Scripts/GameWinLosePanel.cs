using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameWinLosePanel : MonoBehaviour
{
	[SerializeField]
    TextMeshProUGUI killsText;

	private void OnEnable()
	{
        killsText.text = $"Kills: {EnemySpawner.totalKills}";
	}
}
