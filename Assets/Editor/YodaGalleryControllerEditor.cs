using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(YodaGalleryController))]
public class YodaGalleryControllerEditor : Editor {

	public override void OnInspectorGUI() {

		DrawDefaultInspector ();

		YodaGalleryController targetRef = (YodaGalleryController)target;

		if (GUILayout.Button ("Rebuild atlas")) {
			targetRef.rebuildAtlas ();
		}

	}

}
