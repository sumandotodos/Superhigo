using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour {

	AudioSource aSource;
	static AudioManager instance;

	// Use this for initialization
	void Start () {
		aSource = this.GetComponent<AudioSource> ();
		instance = this;
	}

	public static AudioManager Singleton() {
		return instance;
	}
	
	public static void PlaySound_N(AudioClip sound) {
		if (sound != null)
			instance.aSource.PlayOneShot (sound);
	}

}
