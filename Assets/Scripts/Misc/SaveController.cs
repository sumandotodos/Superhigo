using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SparseMatrixData { // this is what get serialized and saved

	public int currentYear;
	public int currentMonth;
	public List<string> keys;
	public List<bool> values;

	public SparseMatrixData() {
		keys = new List<string> ();
		values = new List<bool> ();
		currentYear = 2018;
		currentMonth = 4;
	}

}

[System.Serializable]
public class SaveData {
	public bool explainYY;
	public int redGood, redBad, yellowGood, yellowBad, greenGood, greenBad;

	public bool vrEnabled;

	public SaveData() {
		vrEnabled = false;
		explainYY = false;
		redGood = redBad = yellowGood = yellowBad = greenGood = greenBad = 0;
	}

}

[System.Serializable]
public class GallerySaveData {
	public List<int> unlockedImages;
	public GallerySaveData() {
		unlockedImages = new List<int> ();
	}
}

public class SaveController : MonoBehaviour {

	public static SaveData saveDat = new SaveData();
	public static GallerySaveData gallerySaveData = new GallerySaveData();
	public static SparseMatrixData sparseMatrixData = new SparseMatrixData();

	public const int gallerySize = 332;

	// Use this for initialization
	void Start () {
	
	}

	public static void setSavedYearMonth(int y, int m) {
		sparseMatrixData.currentMonth = m;
		sparseMatrixData.currentYear = y;
	}

	public static void saveSparseData() {
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/calendar.dat", FileMode.Create);
		formatter.Serialize (file, sparseMatrixData);
		file.Close ();
	}
		
	public static void saveGallery() {
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/gallery.dat", FileMode.Create);
		formatter.Serialize (file, gallerySaveData);
		file.Close ();
	}

	public static void loadGallery() {
		if (File.Exists (Application.persistentDataPath + "/gallery.dat")) {
			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/gallery.dat", FileMode.Open);
			gallerySaveData = (GallerySaveData)formatter.Deserialize (file);
			file.Close ();
		} else {
			gallerySaveData = new GallerySaveData ();
		}
	}

	public static void loadSparseData() {
		if (File.Exists (Application.persistentDataPath + "/calendar.dat")) {
			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/calendar.dat", FileMode.Open);
			sparseMatrixData = (SparseMatrixData)formatter.Deserialize (file);
			file.Close ();
		} else {
			sparseMatrixData = new SparseMatrixData ();
		}
	}

	public static void saveUserData() {
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/userData.dat", FileMode.Create);
		formatter.Serialize (file, saveDat);
		file.Close ();
	}

	public static void loadUserData() {
		if (File.Exists (Application.persistentDataPath + "/userData.dat")) {
			BinaryFormatter formatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/userData.dat", FileMode.Open);
			saveDat = (SaveData)formatter.Deserialize (file);
			file.Close ();
		} else {
			saveDat = new SaveData ();
		}
	}

	public static void unblockImage(int n) {
		if (!gallerySaveData.unlockedImages.Contains (n))
			gallerySaveData.unlockedImages.Add (n);
	}

	public static void unblockRandomImage() {
		if (gallerySaveData.unlockedImages.Count == gallerySize)
			return;
		int n = Random.Range (0, gallerySize);
		while (gallerySaveData.unlockedImages.Contains (n)) {
			n = Random.Range (0, gallerySize);
		}
		gallerySaveData.unlockedImages.Add (n);
	}
}
