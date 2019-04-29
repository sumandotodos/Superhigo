using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TextFX { wiggle, wave }

public abstract class UITextFXType {
	protected float amplitude;
	protected float speed;
	public abstract Vector3 getLetterOffset(int index, float time);
	public static UITextFXType Generate(TextFX type, int nOfLetters, float _amplitude, float _speed) {
		switch (type) {
		case TextFX.wiggle:
			return new UITextFXWiggle (nOfLetters, _amplitude, _speed);
		case TextFX.wave:
			return new UITextFXWave (nOfLetters, _amplitude, _speed);
		default:
			throw new System.Exception ("Unknown effect type");
		}
	}
}

public class UITextFXWiggle : UITextFXType {
	float[] letterXPhases;
	float[] letterYPhases;
	public UITextFXWiggle(int nOfLetters, float _amplitude, float _speed) {
		amplitude = _amplitude;
		speed = _speed;
		letterXPhases = new float[nOfLetters];
		letterYPhases = new float[nOfLetters];
		for (int i = 0; i < letterXPhases.Length; ++i) {
			letterXPhases [i] = Random.Range (0.0f, 6.28f);
		}
		for (int i = 0; i < letterYPhases.Length; ++i) {
			letterYPhases [i] = Random.Range (0.0f, 6.28f);
		}
	}
	public override Vector3 getLetterOffset(int index, float deltaTime) {
		letterXPhases[index] += (Time.deltaTime * speed * Random.Range(1.0f, 1.1f));
		letterYPhases[index] += (Time.deltaTime * speed * Random.Range(1.0f, 1.1f));
		return amplitude * new Vector3 (Mathf.Cos (letterXPhases [index]), Mathf.Sin (letterYPhases [index]), 0.0f);
	}
}

public class UITextFXWave : UITextFXType {
	float[] letterPhases;
	const float waveLength = 0.3f;
	public UITextFXWave(int nOfLetters, float _amplitude, float _speed) {
		amplitude = _amplitude;
		speed = _speed;
		letterPhases = new float[nOfLetters];
		for (int i = 0; i < letterPhases.Length; ++i) {
			letterPhases [i] = - i * waveLength;
		}
	}
	public override Vector3 getLetterOffset(int index, float deltaTime) {
		letterPhases [index] += (Time.deltaTime * speed);
		return amplitude * new Vector3 (0.0f, Mathf.Sin (letterPhases [index]), 0.0f);
	}
}



[RequireComponent(typeof(UnityEngine.UI.Text))]
public class UITextFX : MonoBehaviour {

	public float fxSpeed;
	public float fxAmplitude;

	public TextFX type;

	public float space = 2.0f;

	UITextFXType fxGenerator;
	Vector3[] letterRestPositions;
	Text[] letterTextComponents;



	// Use this for initialization
	void Start () {

		Text textComponent = this.GetComponent<Text> ();
		string textContent = textComponent.text;
		TextGenerator textGen = new TextGenerator (textContent.Length);
		fxGenerator = UITextFXType.Generate (type, textContent.Length, fxAmplitude, fxSpeed);
		Vector2 extents = textComponent.gameObject.GetComponent<RectTransform>().rect.size;
		textGen.Populate (textContent, textComponent.GetGenerationSettings (extents));
		createLetterChildObjects (textContent, textComponent, textGen);
		textComponent.enabled = false;
	}

	private void createLetterChildObjects(string content, Text textComponent, TextGenerator textGenerator) {
		Vector3 anchorPoint = getLocalPositionOfCharacter (textGenerator, textComponent, 0);
		letterRestPositions = new Vector3[content.Length];
		letterTextComponents = new Text[content.Length];
		for (int i = 0; i < content.Length; ++i) {
			Text newText = createNewLetterGameObjectWithContentsAndParent (content.Substring(i, 1), textComponent.transform);
			copyTextComponentProperties (newText, textComponent);
			letterRestPositions [i] = (getLocalPositionOfCharacter (textGenerator, textComponent, i) - anchorPoint) * space;
			newText.gameObject.transform.localPosition = letterRestPositions [i];
			letterTextComponents [i] = newText;
			//addOffsetToGameObject (newText.gameObject, getLocalPositionOfCharacter (textGenerator, textComponent, i)-anchorPoint);
		}
	}

	private void addOffsetToGameObject(GameObject gameObject, Vector3 offset) {
		gameObject.transform.position += offset;
	}
		
	private Text createNewLetterGameObjectWithContentsAndParent(string content, Transform parent) {
		GameObject newGameObject = new GameObject ();
		newGameObject.transform.parent = parent;
		Text newText = newGameObject.AddComponent<Text> ();
		newText.text = content;
		return newText;
	}

	private void copyTextComponentProperties(Text newText, Text originalText) {
		newText.font = originalText.font;
		newText.color = originalText.color;
		newText.fontSize = originalText.fontSize;
		newText.rectTransform.anchoredPosition = originalText.rectTransform.anchoredPosition;
		newText.rectTransform.sizeDelta = originalText.rectTransform.sizeDelta;
		newText.transform.position = originalText.transform.position;
		newText.transform.localScale = originalText.transform.localScale;
	}

	private Vector3 getLocalPositionOfCharacter (TextGenerator textGenerator, Text textComponent, int index)
	{

		string textContent = textComponent.text;

		if (index >= textContent.Length) {
			throw new System.Exception ("Character index out of bounds");
		}

		//int newLine = textContent.Substring(0, index).Split('\n').Length - 1;
		//int whiteSpace = textContent.Substring(0, index).Split(' ').Length - 1;
		int indexOfTextQuad = (index * 4);// + (newLine * 4);
		if (indexOfTextQuad < textGenerator.vertexCount)
		{
			

			Vector3 pos = new Vector3 (
				              Mathf.Min (
					              textGenerator.verts [indexOfTextQuad + 0].position.x,
					              textGenerator.verts [indexOfTextQuad + 1].position.x,
					              textGenerator.verts [indexOfTextQuad + 2].position.x,
					              textGenerator.verts [indexOfTextQuad + 3].position.x
				              ),
				              Mathf.Min (
					              textGenerator.verts [indexOfTextQuad + 0].position.y,
					              textGenerator.verts [indexOfTextQuad + 1].position.y,
					              textGenerator.verts [indexOfTextQuad + 2].position.y,
					              textGenerator.verts [indexOfTextQuad + 3].position.y
				              ),
				              Mathf.Min (
					              textGenerator.verts [indexOfTextQuad + 0].position.z,
					              textGenerator.verts [indexOfTextQuad + 1].position.z,
					              textGenerator.verts [indexOfTextQuad + 2].position.z,
					              textGenerator.verts [indexOfTextQuad + 3].position.z
				              )
			              );
				
			return pos;

		}
		else {
			throw new System.Exception ("Character index out of bounds");
		}
	}


	// Update is called once per frame
	void Update () {
		for (int i = 0; i < letterRestPositions.Length; ++i) {
			letterTextComponents [i].transform.localPosition = letterRestPositions [i] + fxGenerator.getLetterOffset (i, Time.deltaTime);
		}

	}
}
