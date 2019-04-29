using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGInput : MonoBehaviour {

	public static bool IsTouchingScreen() {
		return Input.GetMouseButton (0);
	}

	public static bool IsTouchingScreenOnce() {
		return Input.GetMouseButtonDown (0);
	}

}
