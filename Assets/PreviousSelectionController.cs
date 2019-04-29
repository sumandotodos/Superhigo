using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreviousSelectionController : FGProgram {

	public static PreviousSelectionController singleton;
	public UIScaleFader scaler0;
	public UIScaleFader scaler1;
	public UIFader fader0;
	public UIFader fader1;
	public UITextFader hintFader;

	public GameObject higoObject;
	public GameObject neuronObject;

	public UIFader fader;

	void Awake() {
		singleton = this;
		scaler0.Start ();
		scaler1.Start ();
		fader0.Start ();
		fader1.Start ();
		hintFader.Start ();
	}

	public float remaining = 10.0f;

	// Use this for initialization
	void Start () {
		

		execute (this, "initialize");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "PreviousSelectionScreen");
		execute (fader, "fadeToTransparent");
		programGoTo ("BGLoop");

		createSubprogram ("BGLoop");
		execute (scaler1, "scaleIn");
		delay (0.75f);
		execute (scaler0, "scaleIn");
		execute (fader1, "fadeToTransparent");
		delay (1.0f);
		execute (this, "exchangeLayers");
		programGoTo ("BGLoop");


		createSubprogram ("Leave");
		waitForTask (fader, "fadeToOpaqueTask", this);
		programNotifyFinish ();

	}

	public void exchangeLayers() {
		Vector3 localPosition0 = fader0.transform.localPosition;
		float scale0 = scaler0.value;
		float linSpaceScale0 = scaler0.linSpaceValue;


		fader0.transform.localPosition = 
			fader1.transform.localPosition;
		fader1.transform.localPosition = localPosition0;

		scaler1.value = scale0;
		scaler1.linSpaceValue = linSpaceScale0;
		scaler1.scale.setValueImmediate (scale0);


		scaler0.scaleOutImmediately ();
		fader1.fadeToOpaqueImmediately ();
	}

	public void breakpoint() {
		Debug.Log ("...");
	}

	public void initialize() {
		scaler0.scaleOutImmediately ();
		scaler1.scaleOutImmediately ();
		remaining = 10.0f;
		hintFader.fadeToTransparentImmediately ();
		SituationSelection selection = SituationScreenController.singleton.getSelection ();
		higoObject.SetActive (selection == SituationSelection.higo);
		neuronObject.SetActive (selection == SituationSelection.neurons);
	}

	void Update() {
		// TODO someday: implement slots
		update();

		// TODO the following is rather ugly time-slice code:
		/*
		 *  should be replaced by FGProgram code:
		 * 
		 *    delay(10.0f);
		 *    execute(hintFader, "fadeToOpaque");
		 * 
		 * 
		 *   but the only slot in the controller FGProgram 
		 *    is busy running the background animation
		 * 
		 */
		if (isRunning ()) {
			if (remaining > 0.0f) {
				remaining -= Time.deltaTime;
				if (remaining <= 0.0f) {
					hintFader.fadeToOpaque ();
				}
			} 

			if (FGInput.IsTouchingScreenOnce ()) {
				remaining = 10.0f;
				goTo ("Leave");
			}
			
		}

	}

}
