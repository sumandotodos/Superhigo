using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIOpacityWiggle : MonoBehaviour {

	float opacity;

	public Color baseColor = Color.white;

	public float minOpacity = 0.25f;
	public float maxOpacity = 0.75f;

	public float angSpeed = 6.0f;

	float angle;

	//public float tuMadre = 2;
	public bool isActive;

	RawImage img;

	public void reset() {
		opacity = 0.0f;
		isActive = false;
		angle = 0.0f;
		img = this.GetComponent<RawImage> ();
		img.color = new Color (baseColor.r, baseColor.g, baseColor.b, 0);
		isActive = true;
	}

	// Use this for initialization
	void Start () {
		reset ();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (!isActive)
			return;

		angle += angSpeed * Time.deltaTime;
		opacity = FGUtils.RangeRemap (Mathf.Sin (angle), -1.0f, 1.0f, minOpacity, maxOpacity);
		img.color = new Color (baseColor.r, baseColor.g, baseColor.b, opacity);

	}

	public void go() {
		isActive = true;
	}
}
