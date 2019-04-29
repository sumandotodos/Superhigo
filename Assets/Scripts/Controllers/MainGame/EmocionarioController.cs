using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmocionarioController : FGProgram {

	public static EmocionarioController singleton;
	public UIScroller scroller;
	public UIFader fader;
	public UITextFader messageFader;

	public GameObject scrollerGameObject;

	public FGTable contentsTable;

	public Text contentsText;
	public UITextFader contentsFader;

	public int nextTextIndex;

	[HideInInspector]
	public bool firstTime = true;

	void Awake() {
		singleton = this;
	}

	// Use this for initialization
	void Start () {

		execute (this, "initialize");
	
		createSubprogram ("FirstTime");
		debug ("...");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "Emocionario");
		execute (fader, "fadeToTransparent");
		delay (5.25f, this, "checkTouch");
		execute (messageFader, "fadeToTransparent");
		delay (0.75f);
		execute (scroller, "setPage", 0);
		execute (scroller.leftArrow_N, "fadeToTransparent");


		createSubprogram ("NotFirstTime");
		debug ("...");
		execute (messageFader.gameObject, "SetActive", false);
		execute (UICanvasSelector.singleton, "ActivateCanvas", "Emocionario");
		execute (fader, "fadeToTransparent");
		delay (0.25f);
		execute (scroller, "setPage", 0);
		execute (scroller.leftArrow_N, "fadeToTransparent");

		createSubprogram ("Exit");
		waitForTask (fader, "fadeToOpaqueTask", this);
		programNotifyFinish ();

		createSubprogram ("ChangeText");
		debug ("...");
		waitForTask (contentsFader, "fadeToTransparentTask", this);
		execute (this, "setNextText");
		execute (contentsFader, "fadeToOpaque");


	}

	public void initialize() {
		if (firstTime) {
			messageFader.Start ();
			messageFader.fadeToOpaqueImmediately ();
			//scroller.initialize ();
			//scroller.setPage (0);
			initializeCloudTitles ();
			contentsFader.Start ();
			contentsFader.fadeToTransparentImmediately ();
			firstTime = false;
			goTo ("FirstTime");
		} else {
			goTo ("NotFirstTime");
		}
		contentsText.text = "";
	}

	public void checkTouch() {
		if (FGInput.IsTouchingScreenOnce ()) {
			cancelDelay ();
		}
	}

	private void initializeCloudTitles() {
		Text[] cloudTitle = scrollerGameObject.GetComponentsInChildren<Text> ();
		for (int i = 0; i < cloudTitle.Length; ++i) {
			cloudTitle [i].text = (string)contentsTable.getElement ("TITULO", i);
		}
	}

	public void setNextText() {
		contentsText.text = (string)contentsTable.getElement("EXPLICACION", nextTextIndex);
	}

	// UI Callbacks

	public void touchGoBackButton() {

		goTo ("Exit");

	}

	public void touchPrevPage() {

		scroller.previousPage ();

	}

	public void touchNextPage() {

		scroller.nextPage ();

	}

	public void touchCloud(int index) {

		nextTextIndex = index;
		goTo ("ChangeText");

	}



}
