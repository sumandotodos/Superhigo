using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGAnimatorController : MonoBehaviour {

	public Animator AnimatorController_A = null;

	// Use this for initialization
	void Start () {
		if (AnimatorController_A == null)
			AnimatorController_A = GetComponent<Animator> ();
	}

	public void SetTrigger(string name) {
		if (AnimatorController_A != null) {
			AnimatorController_A.SetTrigger (name);
		}
	}

}
