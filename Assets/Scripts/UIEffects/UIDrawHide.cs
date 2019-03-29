using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Axis { X, Y };

public class UIDrawHide : FGProgram {

	public bool midScreenFix = true;

	public float hiddenPosition = 1080.0f;
	public float showingPosition = 0.0f;

	public float parametricSpeed = 200.0f;

	public Axis axis;

	SoftFloat coord;

	float f;

	bool started = false;

	public bool startHidden = true;
	bool working = false;

	private Vector3 coordToVector() {
		if (midScreenFix) {
			if (axis == Axis.X) {
				return new Vector3 (coord.getValue () + Screen.width / 2.0f, Screen.height / 2.0f, 0);
			} else
				return new Vector3 (Screen.width / 2.0f, coord.getValue () + Screen.height / 2.0f, 0);
		} else {
			if (axis == Axis.X) {
				return new Vector3 (coord.getValue (),0, 0);
			} else
				return new Vector3 (0, coord.getValue (), 0);
		}
	}

	public void reset() {
		f = Screen.height / 1920.0f;
		if (startHidden) {
			coord.setValueImmediate (hiddenPosition * f);
		} else
			coord.setValueImmediate (showingPosition * f);
		this.transform.position = coordToVector ();
	}

	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;
		coord = new SoftFloat ();
		coord.setTransformation (TweenTransforms.cubicOut);
		coord.setSpeed(parametricSpeed);
		reset ();

	}

	public void showImmediately() {
		coord.setValueImmediate (showingPosition * f);
		this.transform.position = coordToVector ();
	}

	public void hideImmediately() {
		coord.setValueImmediate (hiddenPosition * f);
		this.transform.position = coordToVector ();
	}

	public void show() {
		coord.setValue (showingPosition * f);
		working = true;
	}
	public void hide() {
		coord.setValue (hiddenPosition * f);
		working = true;
	}
	
	// Update is called once per frame
	void Update () {
		if ((!coord.update ()) && working) {
			notifyFinish ();
			working = false;
		}
		else this.transform.position = coordToVector ();
	}
}
