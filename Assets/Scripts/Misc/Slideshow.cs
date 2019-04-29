using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slideshow : FGProgram {

	public Texture[] slides;
	int index = 0;
	public RawImage frontRI;
	public RawImage backRI;
	public UIFader frontFader;
	public UIFader backFader;
	public float delay;
	public float xFadeSpeed;

	// Use this for initialization
	void Start () {

		execute (this, "initialize");


		createSubprogram ("StartAllOver");
		debug ("...");
		delay (delay);
		execute (frontFader, "fadeToTransparent");
		waitForTask (backFader, "fadeToOpaqueTask", this);
		execute (this, "updateImages");
		programGoTo ("StartAllOver");

		run ();

	}

	public void initialize() {
		
		if (slides.Length < 2) 
			throw new System.Exception ("Not enough images for slideshow");
		
		backFader.speed = xFadeSpeed;
		frontFader.speed = xFadeSpeed;
		frontRI.texture = slides [0];
		backRI.texture = slides [1];
		frontFader.Start ();
		backFader.Start ();
		frontFader.fadeToOpaqueImmediately ();
		backFader.fadeToTransparentImmediately ();

		goTo ("StartAllOver");

	}


	public void updateImages() {
		Debug.Log ("updating...");
		var indexplusone = ((index + 1)) % slides.Length;
		var indexplustwo = ((indexplusone + 1)) % slides.Length;
		frontRI.texture = slides [indexplusone];
		backRI.texture = slides [indexplustwo];
		frontFader.fadeToOpaqueImmediately ();
		backFader.fadeToTransparentImmediately ();
		index++;
	}
}
