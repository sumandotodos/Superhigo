using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaleFader_anchor : MonoBehaviour {

	public float minScale;
	public float maxScale;

	public MonoBehaviour parent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float scale = Mathf.Lerp (minScale, maxScale, ((UITwoPointEffect)parent).getNormalizedParameter ());
		this.transform.localScale = scale * Vector3.one;
	}
}
