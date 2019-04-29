using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(UIAnimatedImage))]
public class SuperHigoTitle : MonoBehaviour {

	Animator anim;
	UIAnimatedImage animatedImage;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		animatedImage = this.GetComponent<UIAnimatedImage> ();
	}
	
	public void showTitle() {
		anim.SetTrigger ("Appear");
		animatedImage.go ();
	}

	public void showTitleEndPose() {
		anim.SetTrigger ("End");
		animatedImage.go ();
	}
}
