using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBlinker : MonoBehaviour {

	public RawImage image;
	public Text text;
	public float frequency;
	public bool startVisible = false;
	public bool autoStart = true;
	public bool framePerfect = false;

	Color baseColor;

	int state;
	float timer;

	private void showImage() {
		Color c = baseColor;
		c.a = 1.0f;
		image.color = c;
	}

	private void hideText() {
		Color c = baseColor;
		c.a = 0.0f;
		text.color = c;
	}

	private void showText() {
		Color c = baseColor;
		c.a = 1.0f;
		text.color = c;
	}

	private void hideImage() {
		Color c = baseColor;
		c.a = 0.0f;
		image.color = c;
	}

	// Use this for initialization
	public void Start () {
		state = 0;
		timer = 0.0f;
		baseColor = Color.white;
		if (image != null) {
			baseColor = image.color;
			if (startVisible)
				showImage ();
			else
				hideImage ();
			
		}
		if (text != null) {
			baseColor = text.color;
			if (startVisible)
				showText ();
			else
				hideText ();

		}
		if (autoStart)
			startBlinking ();
	}

	public void startBlinking() {
		state = 1;
	}

	public void stopBlinking(bool visible) {
		state = 0;
		if(image != null)
		image.enabled = visible;
		if(text != null)
			text.enabled = visible;
	}

	// Update is called once per frame
	void Update () {
		if (state == 0) {

		}

		if (state == 1) {
			timer += Time.deltaTime;
			if (framePerfect)
				timer = (2.0f / frequency);
			if (timer > (1.0f / frequency) / 2.0f) {
				state = 2;
				if (image != null)
					showImage ();
				if (text != null)
					showText ();
				timer = 0.0f;
			}
		}

		if (state == 2) {
			timer += Time.deltaTime;
			if (framePerfect)
				timer = (2.0f / frequency);
			if (timer > (1.0f / frequency) / 2.0f) {
				state = 1;
				if (image != null)
					hideImage ();
				if (text != null)
					hideText ();
				timer = 0.0f;
			}
		}
	}
}
