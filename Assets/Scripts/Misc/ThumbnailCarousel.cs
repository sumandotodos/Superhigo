using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbnailCarousel : MonoBehaviour {

	public TextMesh debug_N;

    public string suffix = " copia";
	public Sprite[] thumbCollection;
	public string fullImageBasePath;
	Sprite[] unlockedThumbCollection;
	int[] unlockedName;
	public YodaGalleryController galleryController;

	public Transform cameraTransform;
	public Transform cameraLookAt;
	public float maxRaycastDist = 100f;

	public float separation;
	public float frontAngle;
	public float topAngle;

	public Camera camera_A;

	public Thumbnail[] thumbs;

	public AdjustSize frameAdjustSize;

	public int size;

	public Renderer lienzo;

	bool stereoMode = false;

	bool [] unlockedImages;

	const int PAD = 3;

	// Use this for initialization
	void Start () {

		SaveController.loadGallery (); // WARNING habilitar en build final
		for(int i = 0; i < 293; ++i) SaveController.unblockImage(i); // WARNING quitar para build final

		// generar lista de imágenes
		int nImages = 0;
		unlockedImages = new bool[thumbCollection.Length];
		for (int i = 0; i < thumbCollection.Length; ++i) {
			//if (SaveController.gallerySaveData.unlockedImages.Contains (i)) {
				unlockedImages [i] = true;
				++nImages;
			//}
			//else
				//unlockedImages [i] = false;
		}
		unlockedThumbCollection = new Sprite[nImages];
		unlockedName = new int[nImages];
		int iIndex = 0;
		for (int i = 0; i < thumbCollection.Length; ++i) {
			if (unlockedImages [i]) {
				unlockedThumbCollection [iIndex] = thumbCollection [i];
				unlockedName [iIndex] = i;
				++iIndex;
			}
		}


	
		if (unlockedThumbCollection.Length == 0) {
			for (int i = 0; i < thumbs.Length; ++i) {
				thumbs [i].gameObject.SetActive (false);
			}
			frameAdjustSize.gameObject.SetActive (false);
		} else if(unlockedThumbCollection.Length == 1) {
			for (int i = 0; i < thumbs.Length; ++i) {
				thumbs [i].gameObject.SetActive (false);
			}
			indexAtFront = 0;
			lienzo.sharedMaterial.mainTexture = null;
			//refreshImage ();
		}else {

			for (int i = 0; i < ((thumbs.Length) - (thumbs.Length / 2)); ++i) {
				thumbs [i].Start ();
				thumbs [i].SetSprite (unlockedThumbCollection[i % unlockedThumbCollection.Length]);
				thumbs [i].frontRotation = frontAngle;
				thumbs [i].topRotation = topAngle;
				thumbs [i].immediatelyDisplaceTo (i);
			}
			int k = -((thumbs.Length / 2));
			for (int i = ((thumbs.Length) - (thumbs.Length / 2)); i < thumbs.Length; ++i) {
				thumbs [i].Start ();
				int off = (unlockedThumbCollection.Length + k);
				while (off < 0)
					off += unlockedThumbCollection.Length;
				off = off % unlockedThumbCollection.Length;
				thumbs [i].SetSprite (unlockedThumbCollection[off]);
				thumbs [i].frontRotation = frontAngle;
				thumbs [i].topRotation = topAngle;
				thumbs [i].immediatelyDisplaceTo (k++);
			}
			lienzo.sharedMaterial.mainTexture = null;
			//refreshImage ();
		}


	}

	int indexAtFront;


	int showingIndex=-1;
	private void refreshImage() {

		Material m = lienzo.sharedMaterial;

        lienzo.enabled = true;

		if (indexAtFront != showingIndex) {
			string DaPath = fullImageBasePath + "/" + indexToStringWithPadding(PAD, unlockedName [indexAtFront]+1) + suffix;//fullImageName [indexAtFront];
			Texture newSpr = Resources.Load<Texture> (DaPath);
			if (m.mainTexture != null) {
				Resources.UnloadAsset (m.mainTexture);
				Resources.UnloadUnusedAssets ();
				System.GC.Collect ();
			}
			galleryController.updateLabel (unlockedName [indexAtFront]);
		
			m.mainTexture = newSpr;

			frameAdjustSize.imageIndex = indexAtFront;
			frameAdjustSize.updateSize ();

			showingIndex = indexAtFront;

		}

	}

    public void hideLienzo() {
        lienzo.enabled = false;

    }

    public void showImage(int n) 
    {
        indexAtFront = n;
        refreshImage();
    }

        public void rotateLeft() {

		indexAtFront = (indexAtFront + 1) % unlockedThumbCollection.Length;
		for (int i = 0; i < thumbs.Length; ++i) {
			int thumbPos = thumbs [i].getPosition ();
			if (thumbPos == -((thumbs.Length / 2))) { // limit: reinsert at right
				thumbPos = ((thumbs.Length) - (thumbs.Length / 2));
				thumbs [i].immediatelyDisplaceTo (thumbPos - 1);
				thumbs [i].SetSprite (unlockedThumbCollection[((indexAtFront + ((thumbs.Length) - (thumbs.Length / 2) - 1)) % unlockedThumbCollection.Length)]);
			} else {
				//thumbs [i].position = thumbPos - 1;
				thumbs [i].displaceTo (thumbPos - 1);
			}
		}


	}

	public void release(int ndisps) {
		for (int i = 0; i < thumbs.Length; ++i) {
			
			float o = thumbs[i].getOffset();
			o -= separation * (float)ndisps;
			thumbs [i].setOffsetImmediately (o);
			thumbs[i].setOffset(0);

		}
		refreshImage ();
	}

	public void rotateRight() {

		indexAtFront = (indexAtFront + unlockedThumbCollection.Length - 1) % unlockedThumbCollection.Length;
		for (int i = 0; i < thumbs.Length; ++i) {
			int thumbPos = thumbs [i].getPosition ();
			if (thumbPos == (((thumbs.Length) - (thumbs.Length / 2)))-1) { // limit: reinsert at left
				thumbPos = -((thumbs.Length / 2))-1;
				thumbs [i].immediatelyDisplaceTo (thumbPos + 1);
				int off = (indexAtFront + (unlockedThumbCollection.Length-((thumbs.Length / 2))));
				while (off < 0)
					off += unlockedThumbCollection.Length;
				off = off % unlockedThumbCollection.Length;
				thumbs[i].SetSprite(unlockedThumbCollection[off]);
			} else {
				thumbs [i].displaceTo (thumbPos + 1);
			}
		}
	
	}


	private void setThumbSpeed(float s) {
		for (int i = 0; i < thumbs.Length; ++i) {
			thumbs [i].setSpeed (s);
		}
	}

	public bool isTouching = false;
	int touchIndex = 0;
	int currentIndex;
	Vector3 touchPoint;
	Vector3 offsetTouchPoint;
	public int nDisplacements = 0;
	public float offset = 0f;
	void Update () {

		if (FGInput.IsTouchingScreenOnce()) {

			touchPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);

			RaycastHit rh;
			Ray ray = camera_A.ScreenPointToRay (touchPoint);
			if (Physics.Raycast (ray, out rh)) {
				if (rh.collider.tag == "GalleryHandle") {
					isTouching = true;
					setThumbSpeed (60f);

					offsetTouchPoint = touchPoint;

					touchIndex = 0;
				}
			}

		}
		else if (Input.GetMouseButtonUp (0) && isTouching) {

			setThumbSpeed (1f);

			for (int j = 0; j < thumbs.Length; ++j)
				thumbs [j].setOffset (0f);
			isTouching = false;

			if (nDisplacements < 0) {
				for (int i = 0; i < (-nDisplacements); ++i) {
					rotateLeft ();
				}
			} else {
				for (int i = 0; i < nDisplacements; ++i) {
					rotateRight ();
				}
			}
			release (nDisplacements);

		}


		if (isTouching) {
			Vector3 curPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0);
			offset = (curPoint.x - offsetTouchPoint.x) * 16f / ((float)Screen.height);
			nDisplacements = (int)((curPoint.x - touchPoint.x) * 5.3f / (float)Screen.width);

				for (int j = 0; j < thumbs.Length; ++j)
					thumbs [j].setOffset (offset);

		}
			


	}


	private string indexToStringWithPadding(int nDigits, int value) {

		int nDigitsInValue = 1;
		if(value > 0) {
			nDigitsInValue = ((int)Mathf.Log10 (value))+1;
		}
		string result = "";
		for (int i = 0; i < nDigits - nDigitsInValue; ++i) {
			result += "0";
		}
		result += ("" + value);
		return result;

	}
}
