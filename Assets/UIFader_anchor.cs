using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIFader_anchor : MonoBehaviour {

	public MonoBehaviour parent;
	RawImage theImage;
	Color initialColor;

	// Use this for initialization
	void Start () {
		theImage = this.GetComponent<RawImage> ();
		initialColor = theImage.color;
	}

	private void setOpacity(float op) {
		theImage.color = new Color (initialColor.r, initialColor.g, initialColor.b, op);
	}
	
	// Update is called once per frame
	void Update () {
		setOpacity (((UITwoPointEffect)parent).getNormalizedParameter ());
	}
}
