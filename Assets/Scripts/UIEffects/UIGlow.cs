using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGlow : MonoBehaviour {

	Color baseColor;
	float phase;
	public float amplitude;
	public float angSpeed;

	RawImage ri;

	public float globalOpacity = 1.0f;

	bool started = false;
	// Use this for initialization
	void Start () {
		if (started == true)
			return;
		started = true;
		ri = this.GetComponent<RawImage> ();
		baseColor = ri.color;
		phase = 0.0f;
	}

	public void setBaseColor(Color b) {
		Start ();
		baseColor = b;
		ri.GetComponent<RawImage> ().color = b;
	}
	
	// Update is called once per frame
	void Update () {
		phase += angSpeed * Time.deltaTime;
		float val = amplitude * Mathf.Sin (phase);
		Color newCol = baseColor + baseColor * val;
		newCol.a = globalOpacity;
		ri.color = newCol;
	}
}
