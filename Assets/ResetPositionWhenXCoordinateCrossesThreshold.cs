using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPositionWhenXCoordinateCrossesThreshold : MonoBehaviour {

	Vector3 originalPosition;


	public float thresholdX;

	// Use this for initialization
	void Start () {
		originalPosition = this.transform.localPosition;

	}

	// Update is called once per frame
	void Update () {
		if (this.transform.position.x < thresholdX) {
			this.transform.localPosition = originalPosition;
		}

	}
}
