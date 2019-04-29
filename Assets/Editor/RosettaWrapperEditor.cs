
#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(RosettaWrapper))]
public class RosettaWrapperEditor : Editor  {


	public override void OnInspectorGUI() {

		DrawDefaultInspector ();



		RosettaWrapper wrapRef = (RosettaWrapper)target;


		if (wrapRef.rosetta == null) {
			wrapRef.rosetta = GameObject.Find ("Rosetta").GetComponent<Rosetta> ();
		}

		if (GUILayout.Button ("Oh, no")) {
			if (wrapRef.rosetta == null) {
				wrapRef.rosetta = GameObject.Find ("Rosetta").GetComponent<Rosetta> ();
			}
		}

	}


}


#endif