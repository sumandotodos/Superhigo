using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGeneralFader : FGProgram {

	UIFader[] imageFaders;
	UITextFader[] textFaders;

	// Use this for initialization
	bool started = false;
	public void Start () {
		if (started)
			return;
		started = true;
		imageFaders = this.GetComponentsInChildren<UIFader> ();
		textFaders = this.GetComponentsInChildren<UITextFader> ();
        foreach (UIFader f in imageFaders) f.Start();
        foreach (UITextFader f in textFaders) f.Start();
	}
	


	public void fadeToOpaque() {
		Start ();
		foreach (UIFader f in imageFaders) {
			f.Start ();
			f.fadeToOpaque ();
		}
		foreach (UITextFader f in textFaders) {
			f.Start ();
			f.fadeToOpaque ();
		}
	}

	public void fadeToTransparent() {
		Start ();
		foreach (UIFader f in imageFaders) {
			f.Start ();
			f.fadeToTransparent ();
		}
		foreach (UITextFader f in textFaders) {
			f.Start ();
			f.fadeToTransparent ();
		}
	}

	public void fadeToTransparentImmediately() {
		Start ();
		foreach (UIFader f in imageFaders) {
			f.Start ();
			f.fadeToTransparentImmediately ();
		}
		foreach (UITextFader f in textFaders) {
			f.Start ();
			f.fadeToTransparentImmediately ();
		}
	}

	public void fadeToOpaqueImmediately() {
		Start ();
		foreach (UIFader f in imageFaders) {
			f.Start ();
			f.fadeToOpaqueImmediately ();
		}
		foreach (UITextFader f in textFaders) {
			f.Start ();
			f.fadeToOpaqueImmediately ();
		}
	}
}
