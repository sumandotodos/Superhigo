using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFeather : MonoBehaviour {

	public Vector3 initialSpeed;
	public Vector3 gravity;
	public float friction;
	float phase;
	public float rockAmplitude;

	Vector3 position;
	Vector3 dampableVelocity;
	Vector3 undampableVelocity;

	float resFactor;

	public float initialPhase;

	// Use this for initialization
	void Start () {
		phase = initialPhase;
		resFactor = Screen.width / 220f;
		position = this.transform.position;
		dampableVelocity = initialSpeed;
		undampableVelocity = gravity;
	}

	public bool going = false;

	public void go() {
		going = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!going)
			return;
		Vector3 velocity = dampableVelocity + undampableVelocity;
		dampableVelocity *= (1.0f - friction * Time.deltaTime * 10f);
		Vector3 rocking = new Vector3 (Mathf.Sin (phase), Mathf.Abs (Mathf.Sin (phase)), 0) * friction * rockAmplitude;
		position += velocity * resFactor * Time.deltaTime;
		this.transform.position = position + rocking;
		phase += friction * 6.28f * Time.deltaTime;
	}
}
