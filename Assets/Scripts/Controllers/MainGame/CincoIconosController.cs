using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TextScrollArea {
	public Text textComponent;
	public UITextFader faderComponent;
}

public class CincoIconosOKLock : FGLock {

	CincoIconosController controller;

	public CincoIconosOKLock(CincoIconosController _controller) {
		controller = _controller;
		controller.okButtonPress.setEnabled (true);
	}

	public override void action () {
		controller.okButtonPress.setEnabled (false);
		controller.goTo ("Exit");
	}

}

enum CincoIconosMode { normal, insultos };

public class CincoIconosController : FGProgram {

	public static CincoIconosController singleton;

	public int nextIndex;

	public FGTable tablaInsultos;

	public FGTable tabla;

	public TextScrollArea textScrollArea;

	public UIButtonPress okButtonPress;

	CincoIconosOKLock okLock;

    public UIGeneralFader allIconsFader;
    public UIScaleFader ruletaScaler;
    public UIFader ruletaFader;
    public UIScaleFader[] iconScaler;

    public UIButtonPress bocina;
    public UIButtonPress okButton;

    string ChangeTextSubprogram = "ChangeText";

	string insultos;

	public UIFader fader;

	void Awake() {
		singleton = this;
	}

	// Use this for initialization
	void Start () {
	
		execute (UICanvasSelector.singleton, "ActivateCanvas", "EmocionarioIcons");
		execute (fader, "fadeToTransparent");
        execute(this, "chooseItemAtRandom");
        execute (this, "initialize");
        delay(0.35f);
        execute (ruletaScaler, "setEasyType", EaseType.boingOut);
        execute (ruletaScaler, "scaleIn");

        delay (4.0f, this, "checkScreenTouch");
        execute (ruletaFader, "fadeToTransparent");
        delay (0.35f);
        execute (this, "scaleInChosenItem");
        delay (5.0f, this, "checkScreenTouch");
        execute (this, "scaleOutChosenItem");
        delay(0.15f);
        execute(this, "SetPressButtonsEnabled", true);
        execute(this.allIconsFader, "fadeToOpaque");
        execute(this, "showInsultosIfSet");

        createSubprogram("ChangeText");
		waitForTask (textScrollArea.faderComponent, "fadeToTransparentTask", this);
		execute (this, "changeText");
		execute (textScrollArea.faderComponent, "fadeToOpaque");


		createSubprogram ("ChangeTextTacos");
		delay (0.25f);
		waitForTask (textScrollArea.faderComponent, "fadeToTransparentTask", this);
		execute (this, "changeText");
		execute (textScrollArea.faderComponent, "fadeToOpaque");
		delay (10.0f);
		waitForTask (textScrollArea.faderComponent, "fadeToTransparentTask", this);
		execute (this, "restoreInsultos");


		createSubprogram ("RecoverTacos");
		delay (0.25f);
		waitForTask (textScrollArea.faderComponent, "fadeToTransparentTask", this);
		execute (this, "restoreInsultos");


		createSubprogram ("Exit");
		waitForTask (fader, "fadeToOpaqueTask", this);
		programNotifyFinish ();

	}

	public void changeText() {
		textScrollArea.textComponent.text = (string)tabla.getElement ("EXPLICACION", nextIndex);
	}

    public void checkScreenTouch()
    {
        if (FGInput.IsTouchingScreenOnce())
            cancelDelay();
    }

    public void SetPressButtonsEnabled(bool en) {
        bocina.executeOnPress = en;
        bocina.enabled = en;
        okButton.executeOnPress = en;
        bocina.enabled = en;
    }

    public void restoreInsultos() {
		textScrollArea.textComponent.text = insultos;
		textScrollArea.faderComponent.fadeToOpaque();
		nextIndex = -1;
	}

    public void scaleInChosenItem()
    {
        iconScaler[RuletaController.singleton.chosenItem].setEasyType(EaseType.boingOut);
        iconScaler[RuletaController.singleton.chosenItem].scaleIn();
    }

    public void scaleOutChosenItem()
    {
        iconScaler[RuletaController.singleton.chosenItem].setEasyType(EaseType.cubicIn);
        iconScaler[RuletaController.singleton.chosenItem].scaleOut();
    }

    public void chooseItemAtRandom()
    {
        RuletaController.singleton.chooseItemAtRandom();
    }

    public void showInsultosIfSet()
    {
        if(insultos!="")
        {
            textScrollArea.faderComponent.fadeToOpaqueImmediately();
        }
    }

    public void initialize() {
		insultos = "";

        SetPressButtonsEnabled(false);

        allIconsFader.Start();
        allIconsFader.fadeToTransparentImmediately();

        ruletaScaler.Start();
        ruletaScaler.scaleOutImmediately();
        ruletaFader.Start();
        ruletaFader.fadeToOpaqueImmediately();
        for (int i = 0; i < iconScaler.Length; ++i)
        {
            iconScaler[i].Start();
            iconScaler[i].scaleOutImmediately();
        }

        if (RuletaController.singleton.chosenItem == RuletaController.PruebaTacos) {
			for (int i = 0; i < 5; ++i) {
				int row = tablaInsultos.getNextRowIndex ();
				insultos += ((string)tablaInsultos.getElement ("TACO", row) + "\n");
			}
			textScrollArea.textComponent.text = insultos;
            textScrollArea.faderComponent.Start();
            textScrollArea.faderComponent.fadeToTransparentImmediately ();
			ChangeTextSubprogram = "ChangeTextTacos";
		} else {
			textScrollArea.faderComponent.fadeToTransparentImmediately ();
			ChangeTextSubprogram = "ChangeText";
		}
		okLock = new CincoIconosOKLock (this);
	}

	// UI Events

	public void touchOnIcon(int index) {
        if (!bocina.executeOnPress) return;
		cancelDelay ();
		if (RuletaController.singleton.chosenItem == RuletaController.PruebaTacos) {
			if ((index - 1) == nextIndex) {
				goTo ("RecoverTacos");
			}
			else {
				goTo ("ChangeTextTacos");
			}
		} else {
			

			goTo ("ChangeText");
		}
		nextIndex = index - 1;
	}

	public void HitOK() {
		okLock.attempt ();
	}
}
