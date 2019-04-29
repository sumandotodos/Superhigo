using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour {

	AudioSource audioSource;

	public AudioClip[] staticClip;

	public static SoundController singleton;

	void Awake() {
		singleton = this;
	}

	public static void playSound(AudioClip clip_N) {
		if (clip_N != null) {
			singleton.playSound_instance (clip_N);
		}
	}

	public static void playSound(int n) {
		singleton.playSound_instance (n);
	}

	private void playSound_instance(int index) {
		if (staticClip.Length > index) {
			audioSource.PlayOneShot (staticClip [index]);
		}
	}

	private void playSound_instance(AudioClip clip) {
		audioSource.PlayOneShot (clip);
	}

	// Use this for initialization
	void Start () {
		audioSource = this.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
