using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionAfterTimeout : MonoBehaviour {

	Vector3 originalPosition;
	public float timeout = 1;
	float remaining;

	// Use this for initialization
	void Start () {
		originalPosition = this.transform.localPosition;
		remaining = timeout;
	}
	
	// Update is called once per frame
	void Update () {
		remaining -= Time.deltaTime;
		if (remaining < 0.0f) {
			remaining = timeout;
			this.transform.localPosition = originalPosition;
		}
	}
}
