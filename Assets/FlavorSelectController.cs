using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlavorSelectController : FGProgram {

	public UIFader fader;

	public UIGeneralFader teensFader;
	public UIGeneralFader kidsFader;

	// Use this for initialization
	void Start () {

		execute (fader, "fadeToTransparent");

		createSubprogram ("FinishSelection");
		delay (2.0f);
		waitForTask (fader, "fadeToOpaqueTask", this);

		run ();

	}
	



	// UI Events

	public void TouchOnTeens() {
		kidsFader.fadeToTransparent ();
		goTo ("FinishSelection");
	}

	public void TouchOnKids() {
		teensFader.fadeToTransparent ();
		goTo ("FinishSelection");
	}


}
