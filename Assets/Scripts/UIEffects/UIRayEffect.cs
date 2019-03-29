using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRayEffect : MonoBehaviour {

	public float angle;
	public float initialPhase;
	public float angleSpeed;

	RawImage theImage = null;

	// Use this for initialization
	void Start () {
		angle = initialPhase;
		theImage = this.GetComponent<RawImage> ();
	}
	
	// Update is called once per frame
	void Update () {

		angle += (angleSpeed) * Time.deltaTime;

		float factor = 0.5f + (1 + Mathf.Cos (angle)) / 4.0f;

		Vector3 newScale = new Vector3 (factor, factor, factor);
		this.transform.localScale = newScale;

		Color newColor = new Color (1, 1, 1, factor);

		if (theImage != null) {
			theImage.color = newColor;
		}

	}
}
