using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Situation {
	public string text;
	public Texture image;
	public Situation(string theText, Texture theImage) {
		text = theText;
		image = theImage;
	}
}

public enum IconType { Pareja, Amigos, Hermano, Hermana, Hijos, MamasPapas, none };
public enum AlienType { AdultoFemale, AdultoMale, KidMale, KidFemale, none };

public class SituationChooser : MonoBehaviour {

	public FGTable Kids_AdultoChicaAmigos; 		//
	public FGTable Kids_AdultoChicaParejas; 	//
	public FGTable Kids_AdultoChicoAmigos; 		//
	public FGTable Kids_AdultoChicoParejas; 	//
	public FGTable Kids_AdultoHijos; 			//
	public FGTable Kids_KidChicaAmigos; 		//
	public FGTable Kids_KidChicaMamasPapas; 	//
	public FGTable Kids_KidHermana; 			//
	public FGTable Kids_KidHermano; 			//
	public FGTable Kids_KidChicoAmigos;			//
	public FGTable Kids_KidChicoMamasPapas;		//

	public FGTable Teens_AdultoChicaAmigos;	//
	public FGTable Teens_AdultoChicaParejas;//
	public FGTable Teens_AdultoChicoAmigos;//
	public FGTable Teens_AdultoChicoParejas;//
	public FGTable Teens_AdultoHijos;//
	public FGTable Teens_TeenChicaAmigos; //
	public FGTable Teens_TeenChicaMamasPapas; //
	public FGTable Teens_TeenChicaParejas; //
	public FGTable Teens_TeenChicoAmigos;
	public FGTable Teens_KidChicoMamasPapas;
	public FGTable Teens_TeenChicoParejas;
	public FGTable Teens_TeenHermana; //
	public FGTable Teens_TeenHermano;//

	public static SituationChooser singleton;

	void Awake() {
		singleton = this;
	}


	// TODO not being able to tabulate this is the cost of using enums instead of indices
	private FGTable translateIconTypeToTable(string typeOfGame, AlienType alienType, IconType iconType) {
		
		if (PlayerPrefs.GetString ("TypeOfGame") == "Kids") {
			
			if (alienType == AlienType.AdultoFemale) {
				
				if (iconType == IconType.Amigos) {
					return Kids_AdultoChicaAmigos;
				}
				if (iconType == IconType.Pareja) {
					return Kids_AdultoChicaParejas;
				}
				if (iconType == IconType.Hijos) {
					return Kids_AdultoHijos;
				}

			} 

			else if (alienType == AlienType.AdultoMale) {
				
				if (iconType == IconType.Amigos) {
					return Kids_AdultoChicoAmigos;
				}
				if (iconType == IconType.Pareja) {
					return Kids_AdultoChicoParejas;
				}
				if (iconType == IconType.Hijos) {
					return Kids_AdultoHijos;
				}

			} 

			else if (alienType == AlienType.KidFemale) {

				if (iconType == IconType.Amigos) {
					return Kids_KidChicaAmigos;
				}
				if (iconType == IconType.MamasPapas) {
					return Kids_KidChicaMamasPapas;
				}
				if (iconType == IconType.Hermana) {
					return Kids_KidHermana;
				}
				if (iconType == IconType.Hermano) {
					return Kids_KidHermano;
				}

			} 

			else if (alienType == AlienType.KidMale) {

				if (iconType == IconType.Amigos) {
					return Kids_KidChicoAmigos;
				}
				if (iconType == IconType.MamasPapas) {
					return Kids_KidChicoMamasPapas;
				}
				if (iconType == IconType.Hermana) {
					return Kids_KidHermana;
				}
				if (iconType == IconType.Hermano) {
					return Kids_KidHermano;
				}

			}
		} 

		else if (PlayerPrefs.GetString ("TypeOfGame") == "Teens") {

			if (alienType == AlienType.AdultoFemale) {

				if (iconType == IconType.Amigos) {
					return Teens_AdultoChicaAmigos;
				}
				if (iconType == IconType.Pareja) {
					return Teens_AdultoChicaParejas;
				}
				if (iconType == IconType.Hijos) {
					return Teens_AdultoHijos;
				}

			} 

			else if (alienType == AlienType.AdultoMale) {

				if (iconType == IconType.Amigos) {
					return Teens_AdultoChicoAmigos;
				}
				if (iconType == IconType.Pareja) {
					return Teens_AdultoChicoParejas;
				}
				if (iconType == IconType.Hijos) {
					return Teens_AdultoHijos;
				}

			} 

			else if (alienType == AlienType.KidFemale) {

				if (iconType == IconType.Amigos) {
					return Teens_TeenChicaAmigos;
				}
				if (iconType == IconType.Pareja) {
					return Teens_TeenChicaParejas;
				}
				if (iconType == IconType.MamasPapas) {
					return Teens_TeenChicaMamasPapas;
				}
				if (iconType == IconType.Hermana) {
					return Teens_TeenHermana;
				}
				if (iconType == IconType.Hermano) {
					return Teens_TeenHermano;
				}

			} 

			else if (alienType == AlienType.KidMale) {

				if (iconType == IconType.Amigos) {
					return Teens_TeenChicoAmigos;
				}
				if (iconType == IconType.MamasPapas) {
					return Teens_KidChicoMamasPapas;
				}
				if (iconType == IconType.Hermana) {
					return Teens_TeenHermana;
				}
				if (iconType == IconType.Hermano) {
					return Teens_TeenHermano;
				}

			}

		}
		return new FGNullTable ();
	}

	private int numberOfTablesAreNotNull(FGTable table1, FGTable table2) {
		int result = 0;
		if (table1.GetType () != typeof(FGNullTable))
			++result;
		if (table2.GetType () != typeof(FGNullTable))
			++result;
		return result;
	}
		
	private FGTable getTableAtRandom() {

		return PlayerPrefs.GetString ("TypeOfGame") == "Kids" ? 
			
			(FGTable)FGUtils.selectObjectAtRandom (
			Kids_AdultoChicaAmigos,
			Kids_AdultoChicaParejas,
			Kids_AdultoChicoAmigos,
			Kids_AdultoChicoParejas,
			Kids_AdultoHijos,
			Kids_KidChicaAmigos,
			Kids_KidChicaMamasPapas,
			Kids_KidHermana,
			Kids_KidHermano,
			Kids_KidChicoAmigos,
			Kids_KidChicoMamasPapas) 
			
				:
			
			(FGTable)FGUtils.selectObjectAtRandom (
			Teens_AdultoChicaAmigos,
			Teens_AdultoChicaParejas,
			Teens_AdultoChicoAmigos,
			Teens_AdultoChicoParejas,
			Teens_AdultoHijos,
			Teens_TeenChicaAmigos,
			Teens_TeenChicaMamasPapas,
			Teens_TeenChicaParejas,
			Teens_TeenChicoAmigos,
			Teens_KidChicoMamasPapas,
			Teens_TeenChicoParejas,
			Teens_TeenHermana,
			Teens_TeenHermano);
	}

	public Situation chooseSituation(SituationType sitType) {

		FGTable selectedTable1 = translateIconTypeToTable(PlayerPrefs.GetString("TypeOfGame"), sitType.alienType, sitType.icon1Type);
		FGTable selectedTable2 = translateIconTypeToTable(PlayerPrefs.GetString("TypeOfGame"), sitType.alienType, sitType.icon2Type);

		FGTable selectedTable = new FGNullTable ();

		int NumberOfNotNullTables = numberOfTablesAreNotNull (selectedTable1, selectedTable2);
		if (NumberOfNotNullTables == 0) {
			selectedTable = getTableAtRandom ();
		} else if (NumberOfNotNullTables == 1) {
			selectedTable = (FGTable)FGUtils.getFirstNonNullElementWithTest ((e) => (e.GetType() != typeof(FGNullTable)), selectedTable1, selectedTable2);
		} else if (NumberOfNotNullTables == 2) {
			selectedTable = (FGTable)FGUtils.selectObjectAtRandom (selectedTable1, selectedTable2);
		}

		int row = selectedTable.getNextRowIndex ();

		string nameOfImage = ((string)selectedTable.getElement ("IMGNAME", row)).Trim ();
		string text = (string)selectedTable.getElement ("TXT", row);
		Texture texture = Resources.Load<Texture> ("Textures/Situations/" + nameOfImage);

		return new Situation("\n" + text, texture);
	}

}
