using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {

	public string sceneToLoad;

	// Use this for initialization
	IEnumerator Start () {
		AsyncOperation loadAll = SceneManager.LoadSceneAsync ("Scenes/" + sceneToLoad);
		yield return loadAll;
	}

}
