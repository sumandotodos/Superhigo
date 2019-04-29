using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : MonoBehaviour {

	public float factor = 1;
	public float xSpeed, ySpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 t = this.transform.position;
		t.x += xSpeed * factor * Time.deltaTime;
		t.y += ySpeed * factor * Time.deltaTime;
		this.transform.position = t;
	}
}
