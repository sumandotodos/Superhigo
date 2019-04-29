using UnityEngine;
using System.Collections;

public class UIButtonPress : MonoBehaviour {

//	public ControllerHub controllerHub_N;
//	public MonoBehaviour buttonPressListener_N;
//	public AudioController audioController;
	public AudioClip sound_N;
	public MonoBehaviour targetScript_N;
	public string eventToRun;


	public float maxScale = 1.0f;
	public float minScale = 0.8f;
	float scale;

	new public bool enabled = true;

	bool pressed = false;

	public bool executeOnPress = true;

	// Use this for initialization
	void Start () {
		//audioController = GameObject.Find ("AudioController").GetComponent<AudioController> ();
		scale = this.transform.localScale.x;
		UIScaleFader scaleFader = this.GetComponent<UIScaleFader> ();
		pressed = false;
		if (scaleFader != null) {
			scale = scaleFader.maxScale;
		} else {
			this.transform.localScale = scale * new Vector3 (maxScale, maxScale, maxScale);
		}

	}


	public void setEnabled(bool en) {
		enabled = en;
	}


	public void onPress() {
		if (!enabled)
			return;
		this.transform.localScale = scale * new Vector3 (minScale, minScale, minScale);
		if (executeOnPress) {
			execute();
		}
		if (sound_N != null) {
			SoundController.playSound (sound_N);
		}
		pressed = true;
	}

	public void onRelease() {
		//if (!enabled)
		//	return;
		if (!executeOnPress) {
			execute ();
		}
		this.transform.localScale = scale * new Vector3 (maxScale, maxScale, maxScale);
		pressed = false;
	}

	public void toggle() {
		if (pressed)
			onRelease();
		else
			onPress ();
	}

	private void execute() {
		if (targetScript_N != null) {
			targetScript_N.Invoke (eventToRun, 0.0f);
		}
	}


}
