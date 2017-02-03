using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
	public Vector3 targetPos;

    private int id;
	private CardManager manager;
	private bool selected;

	public void SetID(int newID) {
		id = newID;
		manager = FindObjectOfType<CardManager>();

		Sprite img = GameManager.instance.images[id];
		GetComponent<SpriteRenderer>().sprite = img;

		Vector3 cardSize = img.bounds.size;
		BoxCollider2D box = GetComponent<BoxCollider2D>();
		box.size = new Vector2(cardSize.x, cardSize.y);
	}

	public int ID() {
		return id;
	}
	
	void Update () {
		Vector3 dir = targetPos - transform.position;
		transform.position += dir * 0.5f;
	}

	private void OnMouseEnter() {
		if (!manager.SetupDone() || UIManager.instance.PlayerPanelOpen() || CardManager.instance.waiting) {
			return;
		}
#if UNITY_STANDALONE || UNITY_EDITOR
		transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);
#endif
	}

	private void OnMouseExit() {
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

	private void OnMouseDown() {
		if (!manager.SetupDone() || UIManager.instance.PlayerPanelOpen() || CardManager.instance.waiting) {
			return;
		}
		selected = !selected;
		if (selected) {
			GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
		} else {
			Unselect();
		}
		manager.OnCardClick(this);
	}

	public void Unselect(bool error=false) {
		selected = false;
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
		if (error) {
			StartCoroutine("Error");
		}
	}

	IEnumerator Error() {
		GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.4f, 0.4f);
		yield return new WaitForSeconds(0.2f);
		GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
		yield return null;
	}

	public bool Selected() {
		return selected;
	}
}
