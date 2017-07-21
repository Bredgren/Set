using UnityEngine;

public class Loader : MonoBehaviour {
    public GameObject gameManager;
	public Deck deck;

	private int prevWidth;

    void Awake() {
        if (GameManager.instance == null) {
            Instantiate(gameManager);
        }

		PositionCamera();
	}

	void Update() {
		PositionCamera();
	}

	void PositionCamera() {
		if (Screen.width == prevWidth) return;
		prevWidth = Screen.width;

		transform.position = new Vector3(0f, 0f, transform.position.z);
		Vector3 windowSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
		Vector3 deckSize = deck.GetComponent<SpriteRenderer>().sprite.bounds.size;
		Vector3 deckPos = deck.transform.position;

		deckSize.Scale(new Vector3(0.5f, -0.5f, 0f));
		windowSize.Scale(new Vector3(1f, -1f, 1f));
		Vector3 topLeft = deckPos - deckSize + new Vector3(-0.4f, 0.4f, 0f);
		transform.position = topLeft + windowSize;

	}
}
