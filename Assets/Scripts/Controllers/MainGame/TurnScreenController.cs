using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryLock : FGLock {
	TurnScreenController turnScreenController;

	public GalleryLock(TurnScreenController _turnScreenController) {
		turnScreenController = _turnScreenController;
	}

	public override void action() {
		turnScreenController.goTo ("TouchWeirdPeopleAtTheBottom");
	}

	public new void reset() {
		base.reset ();
	}
}

public class CloudLock : FGLock {

	TurnScreenController turnScreenController;

	public CloudLock(TurnScreenController _turnScreenController) {
		turnScreenController = _turnScreenController;
	}

	public override void action() {
		turnScreenController.goTo ("TouchCloud");
	}

	public new void reset() {
		base.reset ();
		turnScreenController.blasterToggle.reset ();
	}

}

public class BlasterLock : FGLock {

	TurnScreenController turnScreenController;

	public BlasterLock(TurnScreenController _turnScreenController) {
		turnScreenController = _turnScreenController;
	}

	public override void action() {
		turnScreenController.goTo ("TouchBlaster");
	}

}

public class EndLock : FGLock {

	TurnScreenController turnScreenController;

	public EndLock(TurnScreenController _turnScreenController) {
		turnScreenController = _turnScreenController;
	}

	public override void action() {
		turnScreenController.goTo ("TouchEnd");
	}

}

public class TurnScreenController : FGProgram {

	public UIFader fader;
	public static TurnScreenController singleton;
	public UIGlow sabreGlow;
	public UIImageToggle blasterToggle;

	public Color[] colors;

	CloudLock cloudLock;
	GalleryLock galleryLock;
	BlasterLock blasterLock;
	EndLock endLock;

	void Awake() {
		singleton = this;
	}

	// Use this for initialization
	void Start () {

		initialize ();

		execute (UICanvasSelector.singleton, "ActivateCanvas", "TurnScreen");
		execute (fader, "fadeToTransparent");


		createSubprogram ("TouchCloud");
		waitForTask (fader, "fadeToOpaqueTask", this);
		waitForProgram (EmocionarioController.singleton);
		execute (cloudLock, "reset");
		execute (galleryLock, "reset");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "TurnScreen");
		execute (fader, "fadeToTransparent");


		createSubprogram ("TouchBlaster");
		delay (0.25f);
		waitForTask (fader, "fadeToOpaqueTask", this);
		waitForProgram (RuletaController.singleton);
		execute (blasterLock, "reset");
		execute (blasterToggle, "reset");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "TurnScreen");
		execute (fader, "fadeToTransparent");


		createSubprogram ("TouchEnd");
		waitForTask (fader, "fadeToOpaqueTask", this);
		waitForProgram (FinalScreenController.singleton);
		execute (UICanvasSelector.singleton, "ActivateCanvas", "TurnScreen");
		execute (fader, "fadeToTransparent");


		createSubprogram ("TouchWeirdPeopleAtTheBottom");
		waitForTask (fader, "fadeToOpaqueTask", this);
		waitForProgram (YodaGalleryController.singleton);
		execute (UICanvasSelector.singleton, "ActivateCanvas", "TurnScreen");
		execute (fader, "fadeToTransparent");


		run ();

	}

	public void initialize() {
		cloudLock = new CloudLock (this);
		galleryLock = new GalleryLock (this);
		blasterLock = new BlasterLock (this);
		endLock = new EndLock (this);
		sabreGlow.setBaseColor (FGUtils.randomElementFromArray<Color> (colors));
	}


	// UI Events

	public void touchOnCloud() {
		cloudLock.attempt ();
	}

	public void touchOnBlaster() {
		blasterLock.attempt ();
	}

	public void touchOnEnd() {
		endLock.attempt ();
	}

	public void touchOnWeirdPeople() {
		galleryLock.attempt ();
	}

}
