using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoController : FGProgram {

	public UIFader globalFader;
	public UIScaleFader mascotScaler;

	public UIFeather[] feathers;


	public void setFeathersHidden(bool en) {
		foreach (UIFeather f in feathers) {
			f.gameObject.SetActive (!en);
		}
	}

	public void feathersGo() {
		foreach (UIFeather f in feathers) {
			f.go ();
		}
	}

	// Use this for initialization
	void Start () {

		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		execute (new FGPMethodCall (this, "setFeathersHidden", true));
		execute (new FGPMethodCall (globalFader, "fadeToTransparent"));
		delay (0.75f);
		execute (new FGPMethodCall (this, "setFeathersHidden", false));
		execute (new FGPMethodCall (mascotScaler, "scaleIn"));
		execute (new FGPMethodCall (this, "feathersGo"));
		delay (3.0f, new FGPMethodCall (this, "checkTouch"));
		waitForTask (new FGPMethodCall (globalFader, "fadeToOpaqueTask", this));
		execute (new FGPMethodCall (this, "nextScene"));

		run ();

	}

	public void nextScene() {
		SceneManager.LoadScene ("Title");
	}

	public void checkTouch() {
		if (Input.GetMouseButtonDown (0))
			cancelDelay ();
	}

	public void loginWithFacebook() {

	}

}
