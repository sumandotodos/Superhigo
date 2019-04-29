using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISineScale : MonoBehaviour {

	public float minScale;
	public float maxScale;

	public float angularSpeed;

	float phase = 0;


	// Update is called once per frame
	void Update () {
	
		phase += angularSpeed * Mathf.PI * 2.0f * Time.deltaTime;
		float scale = FGUtils.RangeRemap (Mathf.Sin (phase), -1.0f, 1.0f, minScale, maxScale);
		this.transform.localScale = Vector3.one * scale;

	}
}
