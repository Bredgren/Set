using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player {
	public string name;
	public int score;
	public Button idCard;

	public void SetName(string n) {
		name = n;
		idCard.transform.GetChild(0).GetComponent<Text>().text = name;
	}

	public void SetScore(int n) {
		score = n;
		idCard.transform.GetChild(1).GetComponent<Text>().text = "" + score;
	}
}
