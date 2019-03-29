using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFallingLeave : MonoBehaviour {

	public float VSpeed;
	float angle;
	public float radius;
	public float initialPhase;
	public float angleSpeed;
	Vector3 position;
	public Vector3 initialPosition;

	public bool going;

	// Use this for initialization
	void Start () {
		
		angle = initialPhase;
		//initialPosition = position = this.transform.position;
	}

	public void reset(Vector3 resetpos) {
		this.transform.position = position = resetpos;
		going = false;
	}

	public void go() {
		going = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (!going)
			return;
		angle += angleSpeed * Time.deltaTime;
		Vector3 deviation;
		deviation.x = radius * Mathf.Cos (angle);
		deviation.y = - radius * (Mathf.Sin (angle) * Mathf.Sin (angle));
		deviation.z = 0;

		position.y -= VSpeed * Time.deltaTime;

		this.transform.position = position + deviation;

	}
}
