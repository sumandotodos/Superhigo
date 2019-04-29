using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotateFader_anchor : MonoBehaviour {

	public float minRotation;
	public float maxRotation;

	public MonoBehaviour parent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float rot = Mathf.Lerp (minRotation, maxRotation, ((UITwoPointEffect)parent).getNormalizedParameter ());
		this.transform.rotation = Quaternion.Euler (0, 0, rot);
	}
}
