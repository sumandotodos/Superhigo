using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : FGProgram {

	public UIFader fader;
	public FGAnimatorController higoAnimationController;
	public FGInputManager fgInputManager;

	// Use this for initialization
	void Start () {

		execute (fader, "fadeToTransparent");
		delay(1.0f);
		execute(higoAnimationController, "SetTrigger", "Fly");

		waitForCondition (fgInputManager, "isTouchingScreen");
		waitForTask (fader, "fadeToOpaqueTask", this);

		execute (this, "loadFlavourSelectorGame");

		run ();

	}

	public void loadFlavourSelector() {

	}
	

}
