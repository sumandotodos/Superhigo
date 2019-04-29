using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToggleMode { toggle, justOnce };

public class UIImageToggle : MonoBehaviour {

	RawImage rawImage;

	public MonoBehaviour EventReceiver_N;
	public string executeOnToggleOn;
	public string executeOnToggleOff;

	public AudioClip sound_N;
	public bool mustTurnOffOthers = true;
	public bool mustDisableOthers = true;

	public UIImageToggle[] offOthers_N;
	public UIImageToggle[] disableOthers_N;

	public Texture offImg;
	public Texture onImg;

	public bool defaultOn;
	public ToggleMode toggleMode;

	bool started = false;
	bool enabled = true;

	public void reset() {

		Start ();
		
		if (defaultOn) {
			rawImage.texture = onImg;

		} else
			rawImage.texture = offImg;

		enabled = true;

		//Debug.Log ("RESET DE TOGGLE!!!!");
	}

	public void off() {
		rawImage.texture = offImg;
	}

	public void setEnabled(bool en) {
		enabled = en;
	}

	public void toggle() {

		if (!enabled)
			return;

		SoundController.playSound (sound_N);

		if (toggleMode == ToggleMode.toggle) {
			if (rawImage.texture == onImg) {
				rawImage.texture = offImg;
				execute (false);
			} else {
				rawImage.texture = onImg;
				execute (true);
			}
		} else {
			rawImage.texture = onImg;
			execute (true);
		}

		if (rawImage.texture == onImg) {
			if (mustTurnOffOthers) {
				for (int i = 0; i < offOthers_N.Length; ++i) {
					offOthers_N [i].off ();
				}
			}
			if (mustDisableOthers) {
				for (int i = 0; i < disableOthers_N.Length; ++i) {
					disableOthers_N [i].setEnabled (false);
				}
			}
		}



	}

	public bool status() {
		return rawImage.texture == onImg;
	}

	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;
		rawImage = this.GetComponent<RawImage> ();
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void execute(bool on) {
		Debug.Log ("Execute " + on);
		if (EventReceiver_N != null) {
			EventReceiver_N.Invoke (on? executeOnToggleOn : executeOnToggleOff, 0.0f);
		}
	}
}
