using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITextStimulus : MonoBehaviour {

	public UITextFader fader;

	public float stimulusDuration;
	public float elapsedTime;
	public int state = 0;
	bool started = false;

	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;
		fader.Start ();
		state = 0;
		elapsedTime = stimulusDuration * 2;	
	}
		

	public void stimulate() {
		elapsedTime = 0;
		state = 1;
		fader.setOpacity (1.0f);

	}
	
	// Update is called once per frame
	void Update () {
		if (state == 0) {
			return;
		}
		if (state == 1) {
			elapsedTime += Time.deltaTime;
			if (elapsedTime > stimulusDuration) {
				fader.fadeToTransparent ();
				state = 0;
			}
		}
	}
}
