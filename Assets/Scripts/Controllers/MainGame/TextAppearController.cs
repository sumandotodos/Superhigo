using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAppearController : FGProgram {

	public GameObject ilusion;
	public GameObject habito;
	public GameObject fruta;
	public GameObject no;
	public GameObject es;

	public AudioClip redobleTambores;

	// Use this for initialization
	void Start () {
	
		delay (1.0f);
		execute (ilusion.GetComponent<UITextFader> (), "fadeToOpaque");
		delay (1.5f);
		execute (habito.GetComponent<UITextFader> (), "fadeToOpaque");
		delay (1.5f);
		execute (fruta.GetComponent<UITextFader> (), "fadeToOpaque");
		execute(ilusion.GetComponent<UITextFader> (), "fadeToTransparent");
		delay (2.0f);
		execute (no.GetComponent<UITextFader> (), "fadeToOpaque");
		execute(habito.GetComponent<UITextFader> (), "fadeToTransparent");
		delay (1.5f);
		execute (this, "soundRedoble");
		execute (es.GetComponent<UITextFader> (), "fadeToOpaque");
		execute(fruta.GetComponent<UITextFader> (), "fadeToTransparent");
		delay (1.0f);
		execute (this, "fadeOutEverythingSlowly");
		programNotifyFinish ();

		createSubprogram ("End");
		programNotifyFinish ();

	}

	public void soundRedoble() {
		SoundController.playSound (redobleTambores);
	}

	public void fadeOutEverythingSlowly() {
		ilusion.GetComponent<UITextFader> ().speed = 0.4f;
		ilusion.GetComponent<UITextFader> ().fadeToTransparent ();
		habito.GetComponent<UITextFader> ().speed = 0.4f;
		habito.GetComponent<UITextFader> ().fadeToTransparent ();
		fruta.GetComponent<UITextFader> ().speed = 0.4f;
		fruta.GetComponent<UITextFader> ().fadeToTransparent ();
		no.GetComponent<UITextFader> ().speed = 0.4f;
		no.GetComponent<UITextFader> ().fadeToTransparent ();
		es.GetComponent<UITextFader> ().speed = 0.4f;
		es.GetComponent<UITextFader> ().fadeToTransparent ();
	}

	public void hideEverythingImmediately() {
		ilusion.SetActive (false);
		habito.SetActive (false);
		fruta.SetActive (false);
		no.SetActive (false);
		es.SetActive (false);
	}

}
