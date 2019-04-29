using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITease : MonoBehaviour {

	Vector3 initialScale;
	public float minInterval = 2.0f;
	public float maxInterval = 6.0f;
	public float minAmplitude = 0.75f;
	public float maxAmplitude = 1.3f;
	public float k = 1.6f;
	public float b = 0.5f;
	float remainingTime;
	public Vector2 squash = Vector2.one;

	float burstTimeRemaining;
	public float burstTime = 0.5f;

	float targetSX;
	float targetSY;
	float SX;
	float SY;
	float SXSpeed;
	float SYSpeed;
	float SXAccel;
	float SYAccel;

	private void getNextInterval() {
		remainingTime = Random.Range (minInterval, maxInterval);
	}

	// Use this for initialization
	void Start () {
		remainingTime = 0.0f;
		burstTimeRemaining = 0.0f;
		getNextInterval ();
		initialScale = this.transform.localScale;
		targetSX = SX = initialScale.x;
		targetSY = SY = initialScale.y;
		SXSpeed = SYSpeed = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

		SXAccel = (targetSX - SX) * k - SXSpeed * b;
		SYAccel = (targetSY - SY) * k - SYSpeed * b;

		float deltatime = Time.deltaTime;
		if (deltatime > (1.0f / 30.0f)) {
			deltatime = (1.0f / 30.0f);
		}

		SXSpeed += SXAccel * deltatime;
		SYSpeed += SYAccel * deltatime;

		SX += SXSpeed * deltatime;
		SY += SYSpeed * deltatime;

		Vector3 sc = new Vector3 (1.0f*(1.0f-squash.x) + SX*squash.x, 1.0f*(1.0f-squash.y) + SY*squash.y, 1);
		this.transform.localScale = sc;

		if (burstTimeRemaining > 0.0f) {
			burstTimeRemaining -= deltatime;
			if (burstTimeRemaining <= 0.0f) {
				targetSX = 1.0f;
				targetSY = 1.0f;
			}
		}

		if (remainingTime > 0.0f) {
			remainingTime -= deltatime;
			if (remainingTime <= 0.0f) {
				if (Random.Range (0.0f, 1.0f) < 0.5f) {
					targetSX = maxAmplitude;
					targetSY = minAmplitude;
				} else {
					targetSX = minAmplitude;
					targetSY = maxAmplitude;
				}
//				targetSX = Random.Range (minAmplitude, maxAmplitude);
//				targetSY = Random.Range (minAmplitude, maxAmplitude);
//				while ((targetSX <= 1.0f) && (targetSY <= 1.0f)) {
//					targetSY = Random.Range (minAmplitude, maxAmplitude);
//				}
				burstTimeRemaining = burstTime;
				getNextInterval ();
			}

		} 

	}
}
