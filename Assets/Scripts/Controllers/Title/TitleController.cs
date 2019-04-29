using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipLock : FGLock {

	TitleController controller;

	public SkipLock(TitleController _controller) {
		controller = _controller;
	}

	public override void action() {
		controller.skipSequence ();
	}

}

public class PlayLock : FGLock {

	TitleController controller;

	public PlayLock(TitleController _controller) {
		controller = _controller;
	}

	public override void action() {
		controller.goTo ("Leave");
	}

}

public class TitleController : FGProgram {

	public UIFader fader;
	public FGAnimatorController higoAnimationController;
	public FGInputManager fgInputManager;
	public SuperHigoTitle superHigoTitle;
	public TextAppearController textAppearController;
	public UIScaleFader buttonScaler;
	public UITextFader copyrightFader;
	public UITextFader versionFader;
	public UIFader whiteFader;

	SkipLock skipLock; 
	PlayLock playLock;

	// Use this for initialization
	void Start () {

		skipLock = new SkipLock(this);
		playLock = new PlayLock (this);
		whiteFader.Start ();
		whiteFader.speed = 3.0f;

		execute (fader, "fadeToTransparent");
		waitForProgram (textAppearController);
		delay(1.0f);
		execute(higoAnimationController, "SetTrigger", "Fly");
		delay (0.35f);
		execute (superHigoTitle, "showTitle");
		delay (0.75f);
		execute (whiteFader, "setSpeed", 8.0f);
		execute (whiteFader, "fadeToOpaque");
		delay (0.08f);
		execute (whiteFader, "setSpeed", 0.5f);
		execute (whiteFader, "fadeToTransparent");
		execute (this, "disableCanSkip");
		delay (1.5f);
		execute (buttonScaler, "scaleIn");
		delay (0.5f);
		execute (versionFader, "fadeToOpaque");
		delay (1.0f);
		execute (copyrightFader, "fadeToOpaque");

	
		createSubprogram("Leave");
		waitForTask (fader, "fadeToOpaqueTask", this);
		execute (this, "loadFlavourSelectorGame");


		createSubprogram ("Skip");
		waitForTask (fader, "fadeToOpaqueTask", this);
		execute (this, "setUpSkippedSequence");
		execute (fader, "fadeToTransparent");

		run ();

	}

	void Update() {
		update ();
		if (FGInput.IsTouchingScreenOnce ()) {
			skipLock.attempt ();
		}
	}

	public void disableCanSkip() {
		skipLock.Lock();
	}

	public void loadFlavourSelectorGame() {
		SceneManager.LoadScene ("FlavorSelect");
	}

	public void skipSequence() {
		goTo ("Skip");
	}

	public void setUpSkippedSequence() {
		higoAnimationController.SetTrigger("End");
		copyrightFader.fadeToOpaqueImmediately ();
		versionFader.fadeToOpaqueImmediately ();
		buttonScaler.scaleInImmediately ();
		superHigoTitle.showTitleEndPose ();
		textAppearController.cancelDelay ();
		textAppearController.goTo ("End");
		textAppearController.hideEverythingImmediately ();
		whiteFader.fadeToOpaqueImmediately ();
		whiteFader.speed = 0.2f;
		whiteFader.fadeToTransparent ();
	}

	// UI Events

	public void HitPlay() {
		playLock.attempt ();
	}

}
