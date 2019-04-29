using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIFader : FGProgram, UITwoPointEffect {

	public static UIFader singleton;

	public float prevValue;
	public float value;

	public float opacityValue;
	public float opacityTarget;

	public SoftFloat opacity;
	bool started = false;
	RawImage imageComponent;
	public float maxOpacity = 1.0f;
	public float minOpacity = 0.0f;
	public float speed = 1.0f;
	public Color opaqueColor = Color.black;
	public bool startOpaque = true;

	int state = 0; 	// 0: idle;
					// 1: fading

	private void updateColor() {
		Color newColor = opaqueColor;
		newColor.a = opacity.getValue ();
		imageComponent.color = newColor;
	}

	public static void singletonFadeToOpaque() {
		if (singleton)
			singleton.fadeToOpaque ();
	}

	public void setSpeed(float s) {
		speed = s;
		opacity.setSpeed (speed);
	}

	public static void singletonFadeToTransparent() {
		if (singleton)
			singleton.fadeToTransparent ();
	}

	public void Start() {
		if (started == true)
			return;
		started = true;

		if (this.name == "Fader")
			singleton = this;

		opacity = new SoftFloat ();
		//opacity.setTransformation (TweenTransforms.tanh);
		imageComponent = this.GetComponent<RawImage> ();
		if (startOpaque) {
			opacity.setValueImmediate (maxOpacity);
		} else {
			opacity.setValueImmediate (minOpacity);
		}
		opacity.setSpeed (speed);
		state = 0;
		updateColor ();
	}

	public void setOpacity(float op) {
		if (op > 0.0f) {
			imageComponent.enabled = true;
		} else
			imageComponent.enabled = false;
		opacity.setValueImmediate (op);
		opacityValue = op;
		updateColor ();
	}

	public void fadeToOpaque() {
		state = 1;
		imageComponent.enabled = true;
		opacity.setValue (maxOpacity);
	}

	public void fadeToTransparent() {
		state = 1;
		imageComponent.enabled = true;
		opacity.setValue (minOpacity);
	}

	public void fadeToOpaqueTask(FGProgram waiter) {
		registerWaiter (waiter);
		fadeToOpaque ();
	}

	public void fadeToTransparentTask(FGProgram waiter) {
		registerWaiter (waiter);
		fadeToTransparent ();
	}

	void Update() {

		opacityValue = opacity.getValue ();//value;
		opacityTarget = opacity.linSpaceTarget;

		//update (); // update program
		prevValue = opacity.prevValue;
		value = opacity.getValue ();//.value;
		if(state == 1) {
			if (!opacity.update ()) {
				if (opacity.getValue () == 0.0f) {
					imageComponent.enabled = false;
				}
				notifyFinishExternal ();
				state = 0;
			} 
			updateColor ();

		}

	}

	public void fadeToOpaqueImmediately() {
		state = 0;
		imageComponent.enabled = (maxOpacity > 0.0f);
		opacity.setValueImmediate (maxOpacity);
		updateColor ();
	}

	public void fadeToTransparentImmediately() {
		state = 0;
		imageComponent.enabled = (minOpacity == 0.0f);
		opacity.setValueImmediate (minOpacity);
		updateColor ();
	}

	public void notifyFinishExternal() {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].waitFinish ();
		}
		waiters = new List<FGProgram>();
	}

	public float getNormalizedParameter() {
		return (opacity.getValue () - minOpacity) / (maxOpacity - minOpacity);
	}
		
}
