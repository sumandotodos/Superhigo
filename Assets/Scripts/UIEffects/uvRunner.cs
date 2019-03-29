using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uvRunner : MonoBehaviour {

	public float uSpeed;
	public float vSpeed;
	float u = 0.0f, v = 0.0f;

	public RawImage theRawImage;
	
	// Update is called once per frame
	void Update () {
		u += uSpeed * Time.deltaTime;
		v += vSpeed * Time.deltaTime;
		theRawImage.uvRect = new Rect (u, v, 1, 1);
	}
}
