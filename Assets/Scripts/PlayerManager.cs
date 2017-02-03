using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {
	public static PlayerManager instance = null;

	public GameObject playerPanel;
	public Button playerIDCardPrefab;
	public Button newPlayerButtonPrefab;

	public GameObject playerEditMenu;
	public Button playerEditOkButton;
	public Button playerEditDelButton;
	public InputField playerNameField;

	private Button newPlayerButton;
	private Player editingPlayer;

	private int maxPlayers;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		Vector2 cellSize = playerPanel.GetComponent<GridLayoutGroup>().cellSize;
		Vector2 size = playerPanel.GetComponent<RectTransform>().rect.size;
		maxPlayers = (int)(size.x / cellSize.x) * (int)(size.y / cellSize.y);
	}

	void Start () {
		playerEditMenu.SetActive(false);
		playerEditOkButton.onClick.AddListener(OnDoneEdit);
		playerEditDelButton.onClick.AddListener(OnPlayerDelete);

		foreach (Player p in GameManager.instance.players) {
			NewPlayer(p);
			p.SetScore(0);
		}

		ReplaceNewPlayerBtn();
	}

	private void ReplaceNewPlayerBtn() {
		// TODO: Limit max players by room on the screen
		if (GameManager.instance.players.Count < maxPlayers) {
			newPlayerButton = Instantiate<Button>(newPlayerButtonPrefab);
			RectTransform rt = newPlayerButton.GetComponent<RectTransform>();
			rt.SetParent(playerPanel.transform, false);
			newPlayerButton.onClick.AddListener(OnNewPlayer);
		}
	}

	private void NewPlayer(Player p) {
		p.idCard = Instantiate<Button>(playerIDCardPrefab);
		p.SetName(p.name);
		p.idCard.onClick.AddListener(OnPlayerEdit(p));
		RectTransform rt = p.idCard.GetComponent<RectTransform>();
		rt.SetParent(playerPanel.transform, false);
	}

	public void OnNewPlayer() {
		if (editingPlayer != null) return;
		playerEditMenu.SetActive(true);
		editingPlayer = new Player();
		if (newPlayerButton != null) {
			Destroy(newPlayerButton.gameObject);
		}
		NewPlayer(editingPlayer);
		playerNameField.text = "";
		GameManager.instance.players.Add(editingPlayer);
	}

	public UnityAction OnPlayerEdit(Player p) {
		return () => {
			if (CardManager.instance.waiting) {
				p.SetScore(p.score + 1);
				CardManager.instance.Resume();
				return;
			}
			if (editingPlayer != null) return;
			playerEditMenu.SetActive(true);
			editingPlayer = p;
			playerNameField.text = p.name;
		};
	}

	public void OnDoneEdit() {
		editingPlayer.SetName(playerNameField.text);

		if (editingPlayer == GameManager.instance.players[0]) {
			PlayerPrefs.SetString("Player1", editingPlayer.name);
		}

		editingPlayer = null;
		playerEditMenu.SetActive(false);
		if (newPlayerButton == null) {
			ReplaceNewPlayerBtn();
		}
	}

	public void OnPlayerDelete() {
		GameManager.instance.players.Remove(editingPlayer);
		Destroy(editingPlayer.idCard.gameObject);

		editingPlayer = null;
		playerEditMenu.SetActive(false);
		if (newPlayerButton == null) {
			ReplaceNewPlayerBtn();
		}
	}
}
