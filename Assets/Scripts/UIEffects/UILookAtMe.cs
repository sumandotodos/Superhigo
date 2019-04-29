using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAtMe : MonoBehaviour {

	public float amplitude;
	public float speed;
	public int nTimes;
	public float repeatDelay = 0f;
	public float delayRemain;
	public bool autoGo = false;

	public int timesRemaining;
	public float angle = 0;
	bool going = false;

	// Use this for initialization
	public void Start () {
		reset ();	
		if (autoGo)
			go ();
	}

	public void reset() {
		timesRemaining = nTimes;
		angle = 0;
		going = false;
	}

	public void go() {
		going = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!going) {
			if (delayRemain > 0f) {
				delayRemain -= Time.deltaTime;
				if (delayRemain <= 0f) {
					timesRemaining = nTimes;
					going = true;
				}
			}
			return;
		}
		if (timesRemaining > 0) {
			angle += Time.deltaTime * speed;
			if (angle > 2.0f * Mathf.PI) {
				--timesRemaining;
				if (timesRemaining == 0) {
					going = false;
					delayRemain = repeatDelay;
				}
				angle = 0;
			}
			this.transform.rotation = Quaternion.Euler (0, 0, amplitude * Mathf.Sin (angle));
		}
	}
}
