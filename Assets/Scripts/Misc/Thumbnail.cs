using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thumbnail : MonoBehaviour {

	public float topRotation = 65f;
	public float frontRotation = 0f;
	public float displacement = 2f;
	float opacityLimit = 4.5f;
	float rotationLimit = 1.5f;

	SpriteRenderer sr;

	public float checkOffset;

	SoftFloat xPos;
	SoftFloat xOffset;

	public void setSpeed(float s) {
		xPos.setSpeed (s);
		xOffset.setSpeed (s);
	}


	bool started = false;
	// Use this for initialization
	public void Start () {
		if (started)
			return;
		started = true;


		sr = this.GetComponent<SpriteRenderer> ();
		xPos = new SoftFloat ();
		xPos.setSpeed (10f);
		xPos.setEasyType (EaseType.cubicOut);

		xOffset = new SoftFloat ();
		xOffset.setSpeed (10f);
		xOffset.setEasyType (EaseType.cubicOut);
	}

	float offset = 0f;

	public float getOffset() {
		return xOffset.getValue ();
	}

	public void setOffset(float o) {
		xOffset.setValue (o);
	}

	public void setOffsetImmediately(float o) {
		xOffset.setValueImmediate (o);
	}
	
	// Update is called once per frame
	void Update () {
		
		xPos.update ();
		xOffset.update ();

		checkOffset = xOffset.getValue ();
	
		this.transform.localPosition = new Vector3 (xPos.getValue () + xOffset.getValue(), 0, 0);
		float op = Mathf.Abs (xPos.getValue () + xOffset.getValue()) / opacityLimit;
		if (op > 1f)
			op = 1f;
		op = 1f - op; // invert;
		sr.color = new Color(1, 1, 1, op);
		float angle = topRotation * (xPos.getValue() + xOffset.getValue()) / rotationLimit;
		if (angle > topRotation)
			angle = topRotation;
		if (angle < -topRotation)
			angle = -topRotation;
		this.transform.localRotation = Quaternion.Euler (0, angle, 0);




	}

	public void SetSprite(Sprite s) {
		sr.sprite = s;
	}

	public int position;

	public void displaceTo(int p) {
		position = p;
		xPos.setValueImmediate (displacement * p);
	}

	public void displaceToPosition() {
		xPos.setValue (displacement * position);
	}

	public void immediatelyDisplaceTo(int p) {
		position = p;
		xPos.setValueImmediate (displacement * p);
		float angle = topRotation * (displacement * p) / rotationLimit;
		if (angle > topRotation)
			angle = topRotation;
		if (angle < (-topRotation))
			angle = -topRotation;
		
		this.transform.localRotation = Quaternion.Euler (0, angle, 0);
		this.transform.localPosition = new Vector3 (displacement * p, 0, 0);
		float op = Mathf.Abs (displacement * p) / opacityLimit;
		if (op > 1f)
			op = 1f;
		op = 1f - op; // invert;
		sr.color = new Color(1, 1, 1, op);
	}

	public int getPosition() {
		return position;
	}
}
