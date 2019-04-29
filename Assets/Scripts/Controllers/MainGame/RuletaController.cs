using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SituationType {
	public AlienType alienType;
	public IconType icon1Type;
	public IconType icon2Type;
	public SituationType(AlienType _a, IconType _i1, IconType _i2) {
		alienType = _a;
		icon1Type = _i1;
		icon2Type = _i2;
	}
}

public class OKButtonLock : FGLock {

	RuletaController controller;

	public int stage;

	public OKButtonLock(RuletaController _controller) {
		controller = _controller;
		controller.OKButtonScaler.setEasyType (EaseType.boingOut);
		controller.activateIcons ();
		stage = 0;
	}

	private void assignIconsSelection() {
		controller.assignIconsSelection ();
	}

	public override void action () {
		if (stage == 0) {
			controller.goTo ("SwitchIcons");
			stage = 1;
		} else {
			assignIconsSelection ();
			controller.goTo ("NextScreen");
		}
	}

}

public class RuletaController : FGProgram {

	public static RuletaController singleton;

	public UIImageToggle[] togglesKids;
	public UIImageToggle[] toggesTeens;
	public UIImageToggle[] alienToggles;
	public UIScaleFader OKButtonScaler;
	public UIGeneralFader iconsFaderKids;
	public UIGeneralFader iconsFaderTeens;
	public UIGeneralFader iconsAliens;
	public UITextFader EligeDosIconosFader;
	public UITextFader EligeUnAlienFader;
	//public UIScaleFader ruletaScaler;
	//public UIFader ruletaFader;
	//public UIScaleFader[] iconScaler;
	public UIFader fader;
	[HideInInspector]
	public UIGeneralFader iconsFader;
	OKButtonLock okButtonLock;
	UIImageToggle[] toggles;
	int activatedIndex1 = -1;
	int activatedIndex2 = -1;
	AlienType chosenAlien = AlienType.none;
	const int AlienMale = 0;
	const int AlienFemale = 1;
	const int AlienGirl = 2;
	const int AlienBoy = 3;
	public int chosenItem = -1;
	public const int PruebaTacos = 3;
	IconType chosenIcon1 = IconType.none;
	IconType chosenIcon2 = IconType.none;
	const int Friends = 0;
	const int Sister = 1;
	const int Brother = 2;
	const int MamasPapas = 3;
	const int Couple = 4;
	const int Children = 5;
	public UIGeneralFader textos;

	public int ForceTest = -1;

	void Awake() {
		singleton = this;
	}

	private void selectIconsByTypeOfGame() {
		if (PlayerPrefs.GetString ("TypeOfGame") == "Kids") {
			toggles = togglesKids;
			iconsFader = iconsFaderKids;
		} else {
			toggles = toggesTeens;
			iconsFader = iconsFaderTeens;
		}
	}

	// Use this for initialization
	void Start () {
		
		selectIconsByTypeOfGame ();
		OKButtonScaler.Start ();
		OKButtonScaler.scaleOutImmediately ();
		okButtonLock = new OKButtonLock (this);

		execute (this, "initialize");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "RuletaScreen");
		execute (fader, "fadeToTransparent");
		delay (0.25f);
		execute (iconsFader, "fadeToOpaque");
		delay (0.5f);
		execute (EligeDosIconosFader, "fadeToOpaque");
		delay (0.35f);
		execute (textos, "fadeToOpaque");


		createSubprogram ("SwitchIcons");
		execute (textos, "fadeToTransparent");
		execute (EligeDosIconosFader, "fadeToTransparent");
		execute (this, "deactivateIcons");
		execute (iconsFader, "fadeToTransparent");
		execute (OKButtonScaler, "setEasyType", EaseType.cubicOut);
		execute (OKButtonScaler, "scaleOut");
		delay (0.75f);
		execute (iconsAliens, "fadeToOpaque");
		execute (okButtonLock, "reset");
		delay (0.5f);
		execute (EligeUnAlienFader, "fadeToOpaque");



		createSubprogram ("NextScreen");
		execute (iconsAliens, "fadeToTransparent");
		execute (OKButtonScaler, "setEasyType", EaseType.cubicOut);
		execute (OKButtonScaler, "scaleOut");
		execute (EligeUnAlienFader, "fadeToTransparent");
		//delay (0.35f);
		//execute (ruletaScaler, "setEasyType", EaseType.boingOut);
		//execute (ruletaScaler, "scaleIn");
		//execute (this, "chooseItemAtRandom");
		//delay (4.0f, this, "checkScreenTouch");
		//execute (ruletaFader, "fadeToTransparent");
		//delay (0.35f);
		//execute (this, "scaleInChosenItem");
		//delay (5.0f, this, "checkScreenTouch");
		//execute (this, "scaleOutChosenItem");


		delay (1.5f);
		waitForTask (fader, "fadeToOpaqueTask", this);
		waitForProgram (SituationScreenController.singleton);
		waitForProgram (CincoIconosController.singleton);
		waitForProgram (YodaGalleryController.singleton);
		waitForProgram (PreviousSelectionController.singleton);

		programNotifyFinish ();



	}

	public SituationType getSituationType() {
		return new SituationType (chosenAlien, chosenIcon1, chosenIcon2);
	}

	public void checkScreenTouch() {
		if (FGInput.IsTouchingScreenOnce ())
			cancelDelay ();
	}

	public void chooseItemAtRandom() {
		if (ForceTest != -1)
			chosenItem = ForceTest;
		else {
			chosenItem = Random.Range (0, 5);
		}
	}

	


	// TODO this function can be cleaned up
	//      suggestion: create a class and use a switch statement in the constructor, so as to translate int => IconType, AlienType
	public void assignIconsSelection () {
		
		if (activatedIndex1 == Brother) {
			chosenIcon1 = IconType.Hermano;
		} else if (activatedIndex1 == Sister) {
			chosenIcon1 = IconType.Hermana;
		} else if (activatedIndex1 == MamasPapas) {
			chosenIcon1 = IconType.MamasPapas;
		} else if (activatedIndex1 == Couple) {
			chosenIcon1 = IconType.Pareja;
		} else if (activatedIndex1 == Children) {
			chosenIcon1 = IconType.Hijos;
		} else if (activatedIndex1 == Friends) {
			chosenIcon1 = IconType.Amigos;
		}

		if (activatedIndex2 == Brother) {
			chosenIcon2 = IconType.Hermano;
		} else if (activatedIndex2 == Sister) {
			chosenIcon2 = IconType.Hermana;
		} else if (activatedIndex2 == MamasPapas) {
			chosenIcon2 = IconType.MamasPapas;
		} else if (activatedIndex2 == Couple) {
			chosenIcon2 = IconType.Pareja;
		} else if (activatedIndex2 == Children) {
			chosenIcon2 = IconType.Hijos;
		} else if (activatedIndex2 == Friends) {
			chosenIcon2 = IconType.Amigos;
		}

	}


	public void initialize() {
		
		okButtonLock.reset ();

		initializeVariables ();

		resetToggles ();

		textos.Start ();
		textos.fadeToTransparentImmediately ();

		
		okButtonLock.stage = 0;
		updateOKButtonState ();

	}
	
	private void resetToggles() {
		for (int i = 0; i < toggles.Length; ++i) {
			toggles [i].reset ();
		}
		for (int i = 0; i < alienToggles.Length; ++i) {
			alienToggles [i].reset ();
		}
	}

	private void initializeVariables() {
		chosenItem = -1;
		activatedIndex1 = -1;
		activatedIndex2 = -1;
		chosenAlien = AlienType.none;

	}


	// UI Events

	public void ActivateIcon(int index) {
		if (activatedIndex1 == -1)
			activatedIndex1 = index;
		else {
			if (activatedIndex2 == -1)
				activatedIndex2 = index;
			else {
				toggles [activatedIndex1].off ();
				activatedIndex1 = activatedIndex2;
				activatedIndex2 = index;
			}
		}
		updateOKButtonState ();
	}

	public void DeactivateIcon(int index) {
		if (activatedIndex2 == index) {
			activatedIndex2 = -1;
		} 
		else {
			activatedIndex1 = activatedIndex2;
			activatedIndex2 = -1;
		}
		updateOKButtonState ();
	}



	public bool enoughSelectedIcons() {
		if (okButtonLock.stage == 1) {
			return chosenAlien != AlienType.none;
		}
		else return ((activatedIndex1!=-1) && (activatedIndex2!=-1));
	}

	public void updateOKButtonState() {
		if (enoughSelectedIcons ()) {
			OKButtonScaler.setEasyType (EaseType.boingOut);
			OKButtonScaler.scaleIn ();
		}
		else {
			OKButtonScaler.setEasyType (EaseType.cubicOut);
			OKButtonScaler.scaleOut ();
		}
	}

	public void deactivateIcons() {
		foreach (UIImageToggle toggle in toggles) {
			toggle.setEnabled (false);
		}
	}

	public void activateIcons() {
		foreach (UIImageToggle toggle in toggles) {
			toggle.setEnabled (true);
		}
	}

	// UI Events

	public void touchOKButton() {
		if (enoughSelectedIcons ()) {
			okButtonLock.attempt ();
		}
	}




	public void toggleOn_Friends() {
		ActivateIcon (Friends);
	}
	public void toggleOff_Friends() {
		DeactivateIcon (Friends);
	}

	public void toggleOn_Sister() {
		ActivateIcon (Sister);
	}
	public void toggleOff_Sister() {
		DeactivateIcon (Sister);
	}

	public void toggleOn_Brother() {
		ActivateIcon (Brother);
	}
	public void toggleOff_Brother() {
		DeactivateIcon (Brother);
	}

	public void toggleOn_MamasPapas() {
		ActivateIcon (MamasPapas);
	}
	public void toggleOff_MamasPapas() {
		DeactivateIcon (MamasPapas);
	}

	public void toggleOn_Couple() {
		ActivateIcon (Couple);
	}
	public void toggleOff_Couple() {
		DeactivateIcon (Couple);
	}

	public void toggleOn_Children() {
		ActivateIcon (Children);
	}
	public void toggleOff_Childre() {
		DeactivateIcon (Children);
	}




	private void updateAlienSelection(AlienType s) {
		if (chosenAlien == s) {
			chosenAlien = AlienType.none;
		} else
			chosenAlien = s;
	}

	public void touchAlienAdultMale() {
		updateAlienSelection (AlienType.AdultoMale);
		updateOKButtonState ();
	}
	public void touchAlienAdultFemale() {
		updateAlienSelection (AlienType.AdultoFemale);
		updateOKButtonState ();
	}
	public void touchAlienKidFemale() {
		updateAlienSelection (AlienType.KidFemale);
		updateOKButtonState ();
	}
	public void touchAlienKidMale() {
		updateAlienSelection (AlienType.KidMale);
		updateOKButtonState ();
	}
}
