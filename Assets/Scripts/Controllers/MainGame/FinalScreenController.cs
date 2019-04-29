using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalScreenController : FGProgram {

	public static FinalScreenController singleton;
	public Transform backgroundTransform;
	public UITextFader queTeAcompagneFader;
	public UIScaleFader higoScaler;
	public UIFader fader;
	public float checkVerticalOffset;

	public float verticalOffset;

	SoftFloat backgroundOffset;

	bool initialized = false;

	void Awake() {
		singleton = this;
	}


	// Use this for initialization
	void Start () {

		execute (this, "initialize");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "FinalScreen");
		execute (fader, "fadeToTransparent");
		delay (0.25f);
		execute (this, "setVerticalOffset");
		delay (4.0f);
		execute (queTeAcompagneFader, "fadeToOpaque");
		delay (6.0f);
		execute (higoScaler, "scaleIn");
		delay (2.0f);
		waitForTask (fader, "fadeToOpaqueTask", this);
		execute (this, "resetGame");

	}

	void Update() {
		update ();
		if (initialized) {
			backgroundOffset.update ();
			backgroundTransform.localPosition = new Vector3 (0, backgroundOffset.getValue (), 0);
			checkVerticalOffset = backgroundOffset.getValue ();
		}
	}

	public void setVerticalOffset() {
		backgroundOffset.setValue (verticalOffset);
	}

	public void initialize() {
		higoScaler.Start ();
		backgroundOffset = new SoftFloat (0.0f);
		backgroundOffset.setValueImmediate (0.0f);
		backgroundOffset.setEasyType (EaseType.sigmoid);
		backgroundOffset.setSpeed (105.0f);
		initialized = true;
	}

	public void resetGame() {
		SceneManager.LoadScene ("Logo");
	}
	

}
