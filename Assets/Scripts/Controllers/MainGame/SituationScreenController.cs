using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HigoOKLock : FGLock {

	SituationScreenController controller;

	public HigoOKLock(SituationScreenController _controller) {
		controller = _controller;
		controller.higoPress.setEnabled (true);
		controller.changePress.setEnabled (true);
	}

	public override void action () {
		controller.higoPress.setEnabled (false);
		controller.neuronPress.setEnabled (false);
		controller.changePress.setEnabled (false);
		controller.selection = SituationSelection.higo;
		controller.goTo ("NextScreen");
	}

}

public class NeuronOKLock : FGLock {

	SituationScreenController controller;

	public NeuronOKLock(SituationScreenController _controller) {
		controller = _controller;
		controller.neuronPress.setEnabled (true);
		controller.changePress.setEnabled (true);
	}

	public override void action () {
		controller.higoPress.setEnabled (false);
		controller.neuronPress.setEnabled (false);
		controller.changePress.setEnabled (false);
		controller.selection = SituationSelection.neurons;
		controller.goTo ("NextScreen");
	}
		
}

public enum SituationSelection { higo, neurons, none };



public class SituationScreenController : FGProgram {


	public static SituationScreenController singleton;
	public UIFader situationImageFader;
	public UITextFader situationTextFader;
	public Text situationText;
	public RawImage situationImage;
	public UIFader fader;
	public UIButtonPress higoPress;
	public UIButtonPress neuronPress;
	public UIButtonPress changePress;
	HigoOKLock higoOKLock;
	NeuronOKLock neuronOKLock;


	public SituationSelection selection;

	bool showingText = true;

	void Awake() {
		singleton = this;
	}

	// Use this for initialization
	void Start () {

		execute (this, "initialize");
		execute (this, "prepareSituationTextAndImage");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "SituationScreen");
		execute (fader, "fadeToTransparent");


		createSubprogram ("NextScreen");
		waitForTask (fader, "fadeToOpaqueTask", this);
		programNotifyFinish ();
		//waitForProgram (PreviousSelectionController.singleton);
		//execute (this, "initialize");
		//execute (UICanvasSelector.singleton, "ActivateCanvas", "SituationScreen");
		//execute (fader, "fadeToTransparent");

		

	}

	public void prepareSituationTextAndImage() {
		Situation sit = SituationChooser.singleton.chooseSituation (RuletaController.singleton.getSituationType ());
		situationText.text = sit.text;
		situationImage.texture = sit.image;
	}

	public void initialize() {
		higoOKLock = new HigoOKLock (this);
		neuronOKLock = new NeuronOKLock (this);
		situationTextFader.Start ();
		situationImageFader.Start ();
		showingText = true;
		selection = SituationSelection.none;
		situationTextFader.fadeToOpaque ();
		situationImageFader.fadeToTransparentImmediately ();

	}

	// UI Events

	public void touchOnChange() {
		if (showingText) {
			situationTextFader.fadeToTransparent ();
			situationImageFader.fadeToOpaque ();
		} else {
			situationTextFader.fadeToOpaque ();
			situationImageFader.fadeToTransparent ();
		}
		showingText = !showingText;
	}

	public void touchOnHigoOKButton() {
		higoOKLock.attempt ();
	}

	public void touchOnNeuronsOKButton() {
		neuronOKLock.attempt ();
	}

	public SituationSelection getSelection() {
		return selection;
	}
	

}
