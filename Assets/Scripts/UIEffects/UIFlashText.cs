using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlashText : MonoBehaviour {

	Text theText;

	// Use this for initialization
	void Start () {
		theText = this.GetComponent<Text> ();
		theText.text = "";
	}

	float remainingTime;
	public float delay = 1.5f;

	// Update is called once per frame
	void Update () {
		if (remainingTime > 0.0f) {
			remainingTime -= Time.deltaTime;
			if (remainingTime <= 0.0f) {
				theText.text = "";
			}
		}
	}

	public void flash(string s) {
		theText.text = s;
		remainingTime = delay;
	}
}
