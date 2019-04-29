using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GalleryOKButtonLock : FGLock {

	YodaGalleryController controller;

	public GalleryOKButtonLock(YodaGalleryController _controller) {
		controller = _controller;
		controller.okButtonPress.setEnabled (true);
	}

	public override void action () {
		controller.okButtonPress.setEnabled (false);
		controller.goTo ("Exit");
	}

}

public class DadoLock : FGLock {
    YodaGalleryController controller;
    public DadoLock(YodaGalleryController _controller) {
        controller = _controller;
    }
    public override void action()
    {
        controller.okButtonFader.fadeToOpaque();
        controller.dadoFader.fadeToTransparent();
        controller.showRandomContent();
    }
}

public class YodaGalleryController : FGProgram {

	public static YodaGalleryController singleton;

    public UIGeneralFader dadoFader;

	public UIFader fader;
	public UITextFader textFader;
	public FGTable table;

	public GameObject Gallery3DElements;

	public string FullSizePath;

	public Image outerFrame;
	public RawImage pictureFrame;

	public string OutputAtlasPath;
	public FGTable OutputSizeTable;
	public int OutputAtlasSize = 1024;
	public int numberOfThumbsInRow = 20;

    public ThumbnailCarousel thumbCarousel;

	GalleryOKButtonLock okButtonLock;
    DadoLock dadoLock;
    public UIFader okButtonFader;
	public UIButtonPress okButtonPress;

	void Awake() {
		singleton = this;
	}


	// Use this for initialization
	void Start () {
		
		fader.Start ();

		fader.fadeToTransparent ();

		execute (this, "initialize");
		execute (UICanvasSelector.singleton, "ActivateCanvas", "YodaGalleryScreen"); 
		execute (Gallery3DElements, "SetActive", true);
		execute (fader, "fadeToTransparent");



		createSubprogram ("ChangeLabel");
		waitForTask (textFader, "fadeToTransparentTask", this);
		execute (this, "newLabel");


		createSubprogram ("Exit");
		waitForTask (fader, "fadeToOpaqueTask", this);
		execute (Gallery3DElements, "SetActive", false);
		programNotifyFinish ();
	

	}


	bool labelSet = false;
	int nextLabel;

	public void initialize() {
        thumbCarousel.hideLienzo();
        clearLabel();
		okButtonLock = new GalleryOKButtonLock (this);
        dadoLock = new DadoLock(this);
        dadoFader.Start();
        dadoFader.fadeToOpaqueImmediately();
        okButtonFader.Start();
        okButtonFader.fadeToTransparentImmediately();
	}

	private int translateLabel(int n) {
		

		return n;
	}

	public void newLabel() {
		labelSet = true;
		textFader.GetComponent<Text> ().text = (string)table.getElement (1, translateLabel (nextLabel));//FGUtils.chopLines((string)table.getElement (1, translateLabel(nextLabel)), 32);
		textFader.fadeToOpaque ();
	}

    public void clearLabel() {
        textFader.GetComponent<Text>().text = "";
    }

	public void updateLabel(int n) {

		GameObject.Find ("RosettaWrapper").GetComponent<RosettaWrapper> ().Start ();
		if (!labelSet) {
			int N = translateLabel (n);
			string txt = (string)table.getElement (1, N);
			textFader.Start ();
			textFader.GetComponent<Text> ().text = txt;//FGUtils.chopLines(txt, 32);
			textFader.fadeToOpaque ();
			labelSet = true;
		} else {
			goTo ("ChangeLabel");
			nextLabel = n;
		}
	}

    public void showRandomContent() {
        int n = Random.Range(0, thumbCarousel.thumbs.Length);
        thumbCarousel.showImage(n);
    }

	int state = 0;


	public void rebuildAtlas() {
		
		float thumbSize = (((float)OutputAtlasSize) / ((float)numberOfThumbsInRow));
		RenderTexture blockTex = new RenderTexture ((int)thumbSize, (int)thumbSize, 24);
		Texture2D resultTex = new Texture2D(OutputAtlasSize, OutputAtlasSize);
		RenderTexture.active = blockTex;

		DirectoryInfo dir = new DirectoryInfo (FullSizePath);
		FileInfo[] dirContents = dir.GetFiles ("*.jpg");
		int i = 0, j = 0;

		List<int> W = new List<int>();
		List<int> H = new List<int>();
		
		foreach (FileInfo file in dirContents) {
			
			Debug.Log (file.FullName);
			byte[] fileData = File.ReadAllBytes (file.FullName);
			Texture2D fileTexture = new Texture2D (2, 2);
			fileTexture.LoadImage (fileData);
			Graphics.Blit (fileTexture, blockTex, Vector2.one, Vector2.zero);
			W.Add (fileTexture.width);
			H.Add (fileTexture.height);
			Texture2D saveBlockTexture = new Texture2D ((int)thumbSize, (int)thumbSize);
			saveBlockTexture.ReadPixels (new Rect (0, 0, blockTex.width, blockTex.height), 0, 0);
			saveBlockTexture.Apply ();
			CopyTexRect (resultTex, (int)(thumbSize * (float)j), (int)(thumbSize * (float)i), saveBlockTexture);

			++j;
			if (j == numberOfThumbsInRow) {
				j = 0;
				++i;
			}

		}

		OutputSizeTable.reset ();
		OutputSizeTable.addIntColumnFromList (W, "W");
		OutputSizeTable.addIntColumnFromList (H, "H");
			
		byte[] resultFileBytes = resultTex.EncodeToJPG ();

		File.WriteAllBytes (OutputAtlasPath + "/atlas.jpg", resultFileBytes);

	}

	// NOTE: aunque sea muy lento, sólo lo queremos ejecutar en tiempo de Edición, así que no importa demasiado
	public static void CopyTexRect(Texture2D bigger, int offsetX, int offsetY, Texture2D smaller) {
		for (int i = 0; i < smaller.height; ++i) {
			for (int j = 0; j < smaller.width; ++j) {
				bigger.SetPixel (offsetX + j, 
					bigger.height - smaller.height - offsetY + i, 
					//bigger.height - offsetY - i,
					smaller.GetPixel (j, i));
			}
		}
	}




	// UI Events

	public void touchOnOKButton() {
		okButtonLock.attempt ();
	}

    public void touchOnDado() {
        dadoLock.attempt();
    }
}
