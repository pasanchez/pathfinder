using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour {

	public GameController gController;
	public GameObject battery;
	private Text t;

	void Start () {
		t = battery.GetComponent<Text> ();
	}
	
	public void RedrawBats(int energyleft) {
		if (t == null)
			t = battery.GetComponent<Text> ();
		t.text = energyleft.ToString();
	}
}
