using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour {
	public static CardManager instance = null;
	public int initalCardCount = 12;
	public int maxCardCount = 21;
	public int rows = 3;
	public float padding = 0.2f;

	public Deck deck;
	public Card cardPrefab;

	public GameObject playerPanel;
	public GameObject playerOpenButton;
	public GameObject playerCloseButton;

	[HideInInspector]
	public bool waiting;

	private Card select1;
	private Card select2;
	private Card select3;

	private Vector3 cardSize;

	private class CardSlot {
		public Vector3 pos;
		public Card card = null;
		public CardSlot(Vector3 p) {
			pos = p;
		}
		public void AssignCard(Card c) {
			c.targetPos = pos;
			card = c;
		}
	}
	private List<CardSlot> cardSlots = new List<CardSlot>();
	private List<Card> cards = new List<Card>();
	private bool setupStarted;
	private bool setupDone;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}
	}

	void Start() {
		Card tmpCard = Instantiate(cardPrefab);
		tmpCard.SetID(0);
		cardSize = tmpCard.GetComponent<SpriteRenderer>().sprite.bounds.size;
		Destroy(tmpCard.gameObject);

		InitCardSlots(cardSize);
	}

	void Update() {
		if (!setupStarted && deck.Ready()) {
			setupStarted = true;
			StartCoroutine("DealInitialCards");
		}
		if (waiting) {
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePos.Set(mousePos.x + cardSize.x/2, mousePos.y - cardSize.y/2, 0);
			Vector3 offset = new Vector3(cardSize.x*0.25f, -cardSize.y*0.25f, 0f);
			select1.targetPos = mousePos;
			select2.targetPos = mousePos + offset;
			select3.targetPos = mousePos + offset * 2;
		}
	}

	public bool SetupDone() {
		return setupDone;
	}

	private void InitCardSlots(Vector3 cardSize) {
		float width = cardSize.x + padding;
		float height = cardSize.y + padding;
		for (int i = 0; i < maxCardCount; i++) {
			float x = (i / rows) * width;
			float y = (-i % rows) * height;
			Vector3 pos = transform.position + new Vector3(x, y);
			cardSlots.Add(new CardSlot(pos));
		}
	}

	public int SlotsLeft() {
		return maxCardCount - cards.Count;
	}

	public void DrawCard() {
		int nextID = deck.DrawCard();

		CardSlot nextSlot = null;
		foreach (CardSlot slot in cardSlots) {
			if (slot.card == null) {
				nextSlot = slot;
				break;
			}
		}
		if (nextSlot == null) {
			return;
		}

		Card card = Instantiate(cardPrefab, deck.transform.position, Quaternion.identity);
		card.SetID(nextID);

		nextSlot.AssignCard(card);

		cards.Add(card);
	}

	IEnumerator DealInitialCards() {
		while (cards.Count < initalCardCount) {
			DrawCard();
			yield return new WaitForSeconds(0.1f);
		}
		setupDone = true;
		yield return null;
	}

	IEnumerator Deal3Cards() {
		for (int i = 0; i < 3; i++) {
			DrawCard();
			yield return new WaitForSeconds(0.1f);
		}
		CheckGameOver();
		yield return null;
	}

	public void Draw() {
		StartCoroutine("Deal3Cards");
	}

	public void OnCardClick(Card card) {
		if (card.Selected()) {
			if (select1 == null) {
				select1 = card;
			} else if (select2 == null) {
				select2 = card;
			} else {
				bool set = IsSet(select1.ID(), select2.ID(), card.ID());
				Debug.Log("Check SET: " + select1.ID() + " " + select2.ID() + " " + card.ID() + " " + set);
				if (set) {
					select3 = card;
					select1.Unselect();
					select2.Unselect();
					select3.Unselect();
					// Wait for points to be assigned by the user, then Resume will be called
					if (GameManager.instance.players.Count == 1) {
						GameManager.instance.players[0].SetScore(GameManager.instance.players[0].score + 1);
						Resume();
					} else if (GameManager.instance.players.Count  > 1) {
						waiting = true;
						select1.GetComponent<SpriteRenderer>().sortingLayerName = "GrabbedCards";
						select2.GetComponent<SpriteRenderer>().sortingLayerName = "GrabbedCards";
						select3.GetComponent<SpriteRenderer>().sortingLayerName = "GrabbedCards";
						select1.GetComponent<SpriteRenderer>().sortingOrder = 3;
						select2.GetComponent<SpriteRenderer>().sortingOrder = 2;
						select3.GetComponent<SpriteRenderer>().sortingOrder = 1;
						playerPanel.SetActive(true);
						playerOpenButton.SetActive(false);
						playerCloseButton.SetActive(false);
					}
				} else {
					Debug.Log("NOT SET");
					select1.Unselect(true);
					select2.Unselect(true);
					card.Unselect(true);
					select1 = null;
					select2 = null;
					select3 = null;
				}
			}
		} else {
			if (select1 == card) {
				select1 = null;
			} else if (select2 == card) {
				select2 = null;
			} else {
				Debug.LogError("Deselecting bad card: " + card.ID() + " " + select1.ID() + " " + select2.ID());
			}
		}
	}

	public void Resume() {
		DestroyCard(select1);
		DestroyCard(select2);
		DestroyCard(select3);
		playerPanel.SetActive(false);
		playerOpenButton.SetActive(true);
		playerCloseButton.SetActive(false);
		waiting = false;
		if (deck.CardsLeft() > 0 && cards.Count < initalCardCount) {
			StartCoroutine("Deal3Cards");
		} else {
			SlideCards();
		}
		select1 = null;
		select2 = null;
		select3 = null;
	}

	private bool IsSet(int a, int b, int c) {
		return c == (mod(-(a+b), 3) + (mod(-((a/3)+(b/3)), 3))*3 + (mod(-((a/9)+(b/9)), 3))*9 + (mod(-((a/27) + (b/27)), 3))*27);
	}

	private int mod(int a, int b) {
		return ((a % b) + b) % b;
	}

	private void DestroyCard(Card c) {
		foreach (CardSlot s in cardSlots) {
			if (s.card == c) {
				s.card = null;
				break;
			}
		}
		cards.Remove(c);
		Destroy(c.gameObject);
	}

	private void SlideCards() {
		// Slide cards in the same column
		for (int i = 0; i < cardSlots.Count; i++) {
			CardSlot s = cardSlots[i];
			if (s.card == null) {
				// Find first card in same row and move over
				for (int j = i; j < cardSlots.Count; j += rows) {
					CardSlot s2 = cardSlots[j];
					if (s2.card != null) {
						s.AssignCard(s2.card);
						s2.card = null;
						break;
					}
				}
			}
		}
		// Go backwards and fill any other holes
		for (int col = maxCardCount / rows - 1; col >= 0; col--) {
			List<CardSlot> inCol = new List<CardSlot>();
			for (int row = 0; row < rows; row++) {
				CardSlot s = cardSlots[col * rows + row];
				if (s.card != null) {
					inCol.Add(s);
				}
			}
			if (inCol.Count < rows) {
				foreach (CardSlot s in inCol) {
					foreach (CardSlot s2 in cardSlots) {
						if (s2.card == null) {
							s2.AssignCard(s.card);
							s.card = null;
							break;
						}
					}
				}
			}
		}
	}

	private void CheckGameOver() {
		Debug.Log("Check for Game Over " + deck.CardsLeft());
		if (deck.CardsLeft() > 0) {
			return;
		}

		foreach (Card c1 in cards) {
			foreach (Card c2 in cards) {
				if (c2 == c1) {
					continue;
				}
				foreach (Card c3 in cards) {
					if (c3 == c1 || c3 == c2) {
						continue;
					}
					if (IsSet(c1.ID(), c2.ID(), c3.ID())) {
						Debug.Log("Not game over: " + c1.ID() + " " + c2.ID() + " " + c3.ID());
						return;
					}
				}
			}
		}

		Debug.Log("Game Over");
		GameManager.instance.GameOver();
	}
}
