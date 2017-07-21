using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance = null;
	public Sprite[] images;
	public List<Player> players = new List<Player>();

	void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

		Player p = new Player();
		if (PlayerPrefs.HasKey("Player1")) {
			p.name = PlayerPrefs.GetString("Player1");
		} else {
			p.name = "Player1";
		}
		players.Add(p);
	}

	public void GameOver() {
		FindObjectOfType<UIManager>().ShowElements();
	}
}
