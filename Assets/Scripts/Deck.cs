using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour {
	public const int CARDS = 81;

	private int[] cards = new int[CARDS];
	private int topCard = 0;

	private Text cardsLeftText;
	private bool ready;

	private void Awake() {
		cardsLeftText = GameObject.Find("CardsLeftText").GetComponent<Text>();
		cardsLeftText.text = "Click to\nbegin";
	}

	void Start() {
		for (int i = 0; i < CARDS; i++) {
			cards[i] = i;
		}

		// Shuffle deck
		for (int i = 0; i < 10000; i++) {
			int a = Random.Range(0, CARDS - 1);
			int b = a;
			while (a == b) {
				b = Random.Range(0, CARDS - 1);
			}
			int t = cards[a];
			cards[a] = cards[b];
			cards[b] = t;
		}
	}

	public bool Ready() {
		return ready;
	}

	public int CardsLeft() {
		return CARDS - topCard;
	}

	public int DrawCard() {
		int c = cards[topCard];
		topCard++;
		cardsLeftText.text = "" + CardsLeft();
		if (CardsLeft() <= 0) {
			GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
			cardsLeftText.color = new Color(0.6f, 0.6f, 0.6f);
		}
		return c;
	}

	private void OnMouseEnter() {
		if (CardsLeft() <= 0 ||
			CardManager.instance.SlotsLeft() <= 0 ||
			UIManager.instance.PlayerPanelOpen() ||
			CardManager.instance.waiting) {
			return;
		}
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
		transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);
#endif
	}

	private void OnMouseExit() {
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

	private void OnMouseDown() {
		if (CardManager.instance.SetupDone()) {
			if (CardsLeft() <= 0 ||
				CardManager.instance.SlotsLeft() <= 0 ||
				UIManager.instance.PlayerPanelOpen() ||
				CardManager.instance.waiting) {
				return;
			}
			GameObject.FindObjectOfType<CardManager>().Draw();
		} else {
			if (!ready && !UIManager.instance.PlayerPanelOpen()) {
				ready = true;
				return;
			}
		}
	}
}
