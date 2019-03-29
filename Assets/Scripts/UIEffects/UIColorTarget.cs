using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorTarget : MonoBehaviour {

	SoftVector3 color = null;
	SoftFloat alpha = null;

	public RawImage theImage;
	public Text theText;



	// Use this for initialization
	void Start () {

	}

	public void init(Color initialColor) {
		if (color == null)
			color = new SoftVector3 ();
		if (alpha == null)
			alpha = new SoftFloat ();
		Vector3 col = new Vector3 (initialColor.r, initialColor.g, initialColor.b);
		float a = initialColor.a;
		color.setValueImmediate (col);
		alpha.setValueImmediate (a);
	}

	public void targetTo(Color targetColor) {
		Vector3 col = new Vector3 (targetColor.r, targetColor.g, targetColor.b);
		float a = targetColor.a;
		color.setValue (col);
		alpha.setValue (a);
	}

	// Update is called once per frame
	void Update () {
		color.update ();
		alpha.update ();
		Color newColor = new Color (color.getValue ().x, color.getValue ().y, color.getValue ().z, alpha.getValue ());
		//this.transform.position = position.getValue ();
		if(theImage != null) 
			theImage.color = newColor;
		if (theText != null)
			theImage.color = newColor;
	}
}
