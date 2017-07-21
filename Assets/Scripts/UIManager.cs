using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	public static UIManager instance = null;

	public Text gameOverText;
	public GameObject playerPanel;

	private Button restartButton;

	private void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
	}

	void Start () {
		restartButton.onClick.AddListener(OnRestartButton);
		HideElements();
	}


	public void OnRestartButton() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void HideElements() {
		gameOverText.transform.parent.gameObject.SetActive(false);
		restartButton.gameObject.SetActive(false);
	}

	public void ShowElements() {
		int best = 0;
		List<string> winners = new List<string>();
		foreach (Player p in GameManager.instance.players) {
			if (p.score > best) {
				winners.Clear();
				winners.Add(p.name);
				best = p.score;
			} else if (p.score == best) {
				winners.Add(p.name);
			}
		}
		string text = "Winner";
		if (winners.Count > 1) {
			text += "s";
		}
		text += "\n";
		text += string.Join(", ", winners.ToArray());
		gameOverText.text = text;
		gameOverText.transform.parent.gameObject.SetActive(true);
		restartButton.gameObject.SetActive(true);
	}

	public bool PlayerPanelOpen() {
		return playerPanel.activeInHierarchy;
	}
}
