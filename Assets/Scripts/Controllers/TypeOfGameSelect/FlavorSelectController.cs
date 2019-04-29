using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlavorKidsLock : FGLock {

	FlavorSelectController controller;

	public FlavorKidsLock(FlavorSelectController _controller) {
		controller = _controller;
	}

	public override void action () {
		PlayerPrefs.SetString ("TypeOfGame", "Kids");
		controller.teensLock.Lock ();
		controller.eligeVersionFader.fadeToTransparent ();
		controller.teensFader.fadeToTransparent ();
		controller.goTo ("FinishSelection");
	}

}

public class FlavorTeensLock : FGLock {

	FlavorSelectController controller;

	public FlavorTeensLock(FlavorSelectController _controller) {
		controller = _controller;
	}

	public override void action() {
		PlayerPrefs.SetString ("TypeOfGame", "Teens");
		controller.kidsLock.Lock ();
		controller.eligeVersionFader.fadeToTransparent ();
		controller.kidsFader.fadeToTransparent ();
		controller.goTo ("FinishSelection");
	}

}

public class FlavorSelectController : FGProgram {

	public UIFader fader;

    public GameObject loadingMosca;

	public UIGeneralFader teensFader;
	public UIGeneralFader kidsFader;

	public UITextFader eligeVersionFader;

	[HideInInspector]
	public FlavorKidsLock kidsLock;
	[HideInInspector]
	public FlavorTeensLock teensLock;

	// Use this for initialization
	void Start () {

        loadingMosca.SetActive(false);

		kidsLock = new FlavorKidsLock (this);
		teensLock = new FlavorTeensLock (this);

		execute (fader, "fadeToTransparent");

		createSubprogram ("FinishSelection");
		delay (2.0f);
		waitForTask (fader, "fadeToOpaqueTask", this);
        delay(0.5f);
		execute (this, "LoadNextScene");

		run ();

	}
	
	public void LoadNextScene() {
        loadingMosca.SetActive(true);
		SceneManager.LoadSceneAsync ("MainGame");
	}


	// UI Events

	public void TouchOnTeens() {
		teensLock.attempt ();
	}

	public void TouchOnKids() {
		kidsLock.attempt ();
	}


}
