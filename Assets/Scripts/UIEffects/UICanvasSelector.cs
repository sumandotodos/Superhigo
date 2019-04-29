using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Canvas {

	public abstract void ActivateCanvas ();
	public abstract void DeactivateCanvas ();

}

public class NullCanvas : Canvas {

	public override void ActivateCanvas() {
		throw new System.Exception("Canvas not found exception");
	}

	public override void DeactivateCanvas() {
		throw new System.Exception("Canvas not found exception");
	}

}

public class NoCanvas : Canvas {

	public override void ActivateCanvas() {
		
	}

	public override void DeactivateCanvas() {
		
	}

}

public class ValidCanvas : Canvas {

	private GameObject gameObject;

	public ValidCanvas(GameObject go) {
		gameObject = go;
	}

	public override void ActivateCanvas() {
		gameObject.SetActive (true);
	}

	public override void DeactivateCanvas() {
		gameObject.SetActive (false);
	}

}

public class UICanvasSelector : MonoBehaviour {

	public GameObject[] canvases;
	public Canvas previousCanvas;
	public static UICanvasSelector singleton;

	void Awake() {
		singleton = this;
	}

	void Start() {
		previousCanvas = new NoCanvas ();
	}

	protected Canvas getCanvasByName(string name) {
		for (int i = 0; i < canvases.Length; ++i) {
			if (canvases [i].name == name)
				return new ValidCanvas (canvases [i]);
		}
		return new NullCanvas ();
	}

	public void ActivateCanvas(string name) {
		//Canvas canvasToActivate = instance.getCanvasByName (name);
		//instance.previousCanvas.DeactivateCanvas ();
		//canvasToActivate.ActivateCanvas ();
		//instance.previousCanvas = canvasToActivate;
		for (int i = 0; i < singleton.canvases.Length; ++i) {
			canvases [i].SetActive (canvases [i].name == name);
		}
	}

	public static void _DeactivateCanvas(string name) {
		Canvas canvasToActivate = singleton.getCanvasByName (name);
		canvasToActivate.DeactivateCanvas ();
	}
}
