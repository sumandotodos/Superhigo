using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGInputManager : MonoBehaviour {

	const int LeftMouseButtonId = 0;

	public bool isTouchingScreen() {
		return Input.GetMouseButton (LeftMouseButtonId);
	}

	public bool touchScreen() {
		return Input.GetMouseButtonDown (LeftMouseButtonId);
	}

}
