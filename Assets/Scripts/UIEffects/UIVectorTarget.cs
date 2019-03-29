using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVectorTarget : MonoBehaviour {

	SoftVector2 position = null;



	// Use this for initialization
	void Start () {
		
	}

	public void init(Vector2 initialPos) {
		if (position == null)
			position = new SoftVector2 ();
		position.setValueImmediate (initialPos);
	}

	public void targetTo(Vector2 target) {
		position.setValue (target);
	}
	
	// Update is called once per frame
	void Update () {
		position.update ();
		this.transform.position = position.getValue ();
	}
}
