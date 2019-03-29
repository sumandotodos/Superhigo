using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EaseType { linear, tanh, cubicOut, boingOut, boingOutMore };

public class TweenTransforms {

	public static float linear(float origin, float current, float dest) {
		return current;
	}

	public static float boingOut(float origin, float current, float dest) {
		float r = 0.0f;
		if (dest == origin)
			return current;
		float t = (current - origin) / (dest - origin);
		r = (56.0f * t * t * t * t * t + -175.0f * t * t * t * t + 200.0f * t * t * t -100.0f * t * t + 20.0f * t);
		return origin + r * (dest - origin);
	}

	public static float boingOutMore(float origin, float current, float dest){

		dest -= origin;

		float d = 1f;
		float p = d * .3f;
		float s = 0;
		float a = 0;

		if (current == 0) return origin;

		if ((current /= d) == 1) return origin + dest;

		if (a == 0f || a < Mathf.Abs(dest)){
			a = dest;
			s = p * 0.25f;
		}else{
			s = p / (2 * Mathf.PI) * Mathf.Asin(dest / a);
		}

		return (a * Mathf.Pow(2, -10 * current) * Mathf.Sin((current * d - s) * (2 * Mathf.PI) / p) + dest + origin);
	}		
				
	public static float cubicOut(float origin, float current, float dest) {
		float r = 0.0f;
		if (dest == origin)
			return current;
		float t = (current - origin) / (dest - origin);
		r = (t * t * t + -3 * t * t + 3*t);
		return origin + r * (dest - origin);
	}

	public static float tanh(float origin, float current, float dest) {
		float r = 0.0f;
		if (dest == origin)
			return current;
		if (current == origin)
			r = 0.0f;
		else if (current == dest)
			r = 1.0f;
		else {
			float t = (current - origin) / (dest - origin); // 0..1 space parameter
			r = 0.5f + ((Mathf.Exp ((-2.5f) + t * 5.0f) - Mathf.Exp (-((-2.5f) + t * 5.0f))) / (Mathf.Exp ((-2.5f) + t * 5.0f) + Mathf.Exp (-((-2.5f) + t * 5.0f)))) / 2.0f; // 0..1 space result
		}
		return origin + r*(dest-origin);

	}

}



public abstract class SoftVector {

	public float speed = 1.0f;

	public bool accumulativeMode = true;



	abstract public Vector3 getValue ();
	//abstract public void setValue (Vector2 v);
	//abstract public void setValueImmediate (Vector2 v);

	abstract public bool update ();
	abstract protected float getLinValue ();
}

[System.Serializable]
public class SoftVector2 : SoftVector {

	public Vector2 origin;
	public Vector2 dest;
	public Vector2 current;
	public float localT;
	public int seg;
	public float pathDist;

	public List<Vector2> points;
	public List<float> lengths;
	public List<float> dists;

	SoftFloat linVar;

	public float totalLength;

	public SoftVector2() {
		linVar = new SoftFloat ();
		origin = current = dest = Vector2.zero;
		points = new List<Vector2> ();
		lengths = new List<float> ();
		dists = new List<float> ();

	}

	public void setValueImmediate(Vector2 v) {
		dest = origin = current = v;
		points = new List<Vector2> ();
		lengths = new List<float> ();
		dists = new List<float> ();
		totalLength = 0.0f;
		linVar.setValueImmediate (0.0f);
	}

	public void setValue(Vector2 v) {
		if (points.Count == 0) {
			
			lengths.Add((v - origin).magnitude);
			totalLength += lengths [0];

		} else {
			lengths.Add(  (v - points[points.Count-1]).magnitude  );
			totalLength += lengths [lengths.Count - 1];
		}
		dists.Add (totalLength);
		dest = v;
		origin = current;
		points.Add (v);
		linVar.setValueImmediate (0.0f);
		linVar.setValue (1.0f);
		linVar.setSpeed (speed);//speed / (dest - origin).magnitude);
	}

	public void setEasyType(EaseType t) {
		linVar.setEasyType (t);
	}

	override public Vector3 getValue() {
		return current;
	}

	override protected float getLinValue() {
		return linVar.getValue();
	}

	private int getSegment(float param) {
		if (totalLength == 0.0f)
			return 0;
		pathDist = param * totalLength;
		float accumParam = lengths[0]/totalLength;
		if(param < accumParam) return 0;
		for (int i = 1; i < lengths.Count; ++i) {
			accumParam += (lengths [i] / totalLength);
			if (accumParam >= param) {
				return i;
			}
		}
		return lengths.Count - 1;


	}

	override public bool update() {

		if (points.Count == 0)
			return false;

		bool res;
		float t = getLinValue ();
		int s = getSegment (t);
		seg = s;
		Vector2 localOrigin;
		Vector2 localDest;
		if (s == 0)
			localOrigin = origin;
		else {
			localOrigin = points [s-1];
		}
		localDest = points [s];
		localT = t;
		if (s == 0) {
			if (lengths.Count == 0 || lengths [0] == 0.0f) {
				localT = 0.0f;
			}
			else
			localT = t*totalLength / lengths [0];
		} else
			localT = (t*totalLength - dists [s - 1]) / lengths [s];
		current = localOrigin + (localDest - localOrigin) * localT;
		res = linVar.update();
		return res;

	}

}

[System.Serializable]
public class SoftVector3 : SoftVector {

	public Vector3 origin;
	public Vector3 dest;
	public Vector3 current;
	public float localT;
	public int seg;
	public float pathDist;

	SoftFloat linVar;

	public float totalLength;

	public SoftVector3() {
		linVar = new SoftFloat ();
		origin = current = dest = Vector3.zero;
	}

	public void setValueImmediate(Vector3 v) {
		dest = origin = current = v;
		linVar.setValueImmediate (0.0f);
	}

	public void setValue(Vector3 v) {
		dest = v;
		origin = current;
		linVar.setValueImmediate (0.0f);
		linVar.setValue (1.0f);
		linVar.setSpeed (speed);//speed / (dest - origin).magnitude);
	}

	public void setEasyType(EaseType t) {
		linVar.setEasyType (t);
	}

	override public Vector3 getValue() {
		return current;
	}

	override protected float getLinValue() {
		return linVar.getValue();
	}
		

	override public bool update() {

	
		bool res;
		float t = getLinValue ();

		current = origin + (dest - origin) * t;
		res = linVar.update();
		return res;

	}

}

public abstract class SoftVariable {

	float target;
	float t;
	protected float speed;
	protected System.Func<float, float, float, float> transformation;
	public void setSpeed(float newSpeed) {
		speed = newSpeed;
	}
	public abstract float getValue();
	public abstract void setValue (float newValue);
	public abstract void setValueImmediate (float newValue);
}

[System.Serializable]
public class DBSLSoftFloat:SoftVariable {

	public  float value;
	public  float target;
	protected System.Func<float, float, float, float> calculateSpeed;

	public float speed;
	public float origin;

	public float maxSpeed = 1.0f;

	public float threshold = 0.01f;

	public float factor = 0.5f;

	public DBSLSoftFloat() {
		value = target = origin = 0;
		calculateSpeed = diffSquared;
	}

	public DBSLSoftFloat(float initial) {
		value = target = origin = initial;
		calculateSpeed = diffSquared;
	}

	private float diffSquared(float value, float target, float factor) {
		float sign = 1.0f;
		if (target < value)
			sign = -1.0f;
		return sign * (target - value) * (target - value) * factor;
	}

	public void update() {
		
		if (Mathf.Abs (origin - target) > threshold) {
			if ((value - origin) / (target - origin) >= (1.0f - threshold)) {
				speed = 0;
				value = target;
			} else {
				speed = calculateSpeed (value, target, factor);
				if (Mathf.Abs (speed) > maxSpeed) {
					if (speed > 0.0f)
						speed = maxSpeed;
					else
						speed = -maxSpeed;
				}
				value += speed * Time.deltaTime;
			}
		}

	}

	override public float getValue() {
		return value;
	}

	override public void setValueImmediate(float newValue) {
		target = value = origin = newValue;
		speed = 0;
	}

	override public void setValue(float newValue) {
		
		origin = value;
		target = newValue;
	}

}
[System.Serializable]
public class SoftFloat:SoftVariable {

	public float prevValue;
	private float sfvalue;
	public float linSpaceTarget;
	public float linSpaceValue;
	public float linSpaceOrigin;
	public float tParam;

	public void setEasyType(EaseType t) {
		switch (t) {
		case EaseType.boingOut:
			setTransformation (TweenTransforms.boingOut);
			break;
		case EaseType.boingOutMore:
			setTransformation (TweenTransforms.boingOutMore);
			break;
		case EaseType.cubicOut:
			setTransformation (TweenTransforms.cubicOut);
			break;
		case EaseType.linear:
			setTransformation (TweenTransforms.linear);
			break;
		case EaseType.tanh:
			setTransformation (TweenTransforms.tanh);
			break;
		}
	}

	public SoftFloat() {
		prevValue = 0.0f;
		sfvalue = 0.0f;
		speed = 1;
		linSpaceValue = 0.0f;
		linSpaceTarget = 0.0f;
		linSpaceOrigin = 0.0f;
		transformation = TweenTransforms.linear;
	}

	public SoftFloat(float initial) {
		prevValue = initial;
		sfvalue = initial;
		linSpaceValue = initial;
		linSpaceTarget = initial;
		linSpaceOrigin = initial;
		transformation = TweenTransforms.linear;
	}

	public void setTransformation(System.Func<float, float, float, float> transformFunc) {
		transformation = transformFunc;
	}

	override public float getValue() {
		if (linSpaceValue == linSpaceTarget)
			return linSpaceTarget;
		if (linSpaceValue == linSpaceOrigin)
			return linSpaceOrigin;
		sfvalue = transformation (linSpaceOrigin, linSpaceValue, linSpaceTarget);
		return sfvalue;
	}

	override public void setValueImmediate(float newValue) {
		prevValue = newValue;
		sfvalue = newValue;
		linSpaceTarget = newValue;
		linSpaceOrigin = newValue;
		linSpaceValue = newValue;
	}

	override public void setValue(float newFloat) {
		prevValue = sfvalue;
		sfvalue = newFloat;
		linSpaceTarget = sfvalue;
		linSpaceValue = prevValue;
		linSpaceOrigin = prevValue;
	}

	public bool update() {
		
		if (linSpaceValue > linSpaceTarget) {
			linSpaceValue -= speed * Time.deltaTime;
			if (linSpaceValue < linSpaceTarget) {
				linSpaceValue = linSpaceTarget;
				return false;
			}
			return true;
		} else if (linSpaceValue < linSpaceTarget) {
			linSpaceValue += speed * Time.deltaTime;
			if (linSpaceValue > linSpaceTarget) {
				linSpaceValue = linSpaceTarget;
				return false;
			}
			return true;
		} else
			return false;

	}

}

public class BootStrapData {

	public string loginServer;
	public int loginServerPort;
	public string socketServer;
	public int socketServerPort;
	public string commandServer;
	public int commandServerPort;

	public BootStrapData(string ls, int lsp, string ss, int ssp, string cs, int csp) {
		loginServer = ls;
		loginServerPort = lsp;
		socketServer = ss;
		socketServerPort = ssp;
		commandServer = cs;
		commandServerPort = csp;
	}
}

public class FGUtils : MonoBehaviour {

	/* constants */


	public const string BootstrapURL = "http://apps.flygames.org/bootstrap";



	//public const string flygamesLoginCheck = "https://apps.flygames.org:9090";
	public const string flygamesSSLAuthHost = "apps.flygames.org";
	public const string getCountryListScript = "/listCountries";
	public const string getLocalitiesListScript = "/listLocalitiesByCountry";
	public const string getOrganizationsListScript = "/listOrganizationByLocCoun";
	public const string getClassroomsListScript = "/listClassroomsByOrgLocCoun";
	public const string getDebateDB = "/retrieveDebateDB";
	public const string getUserNickname = "/getUserNickname";
	public const string setUserNickname = "/setUserNickname";
	public const string RecoveryScript = "/requestNewPassword";
	public const string CheckUserScript = "/checkUser";
	public const string NewUserScript = "/newUser";
	public const string GetFreshRoomID = "/nextRoomID.php";
	public const string ReleaseRoomID = "/clearRoomID";
	//public const string GameRelayServer = "apps.flygames.org"; // primary Linode
	public const string localGamePrefix = "AnimCreat";

	public const int socketPort = 993;

	public const string fallbackLoginServer = "https://apps.flygames.org";
	public const int fallbackLoginServerPort = 25;
	public const string fallbackSocketServer = "apps.flygames.org";
	public const int fallbackSocketServerPort = 443;
	public const string fallbackCommandServer = "apps.flygames.org";
	public const int fallbackCommandServerPort = 110;

	public const string appsPSKSecret = "g2T21X48tJ21pqx7571ad90";

	public const float delta = 0.01f;

	public const int facesRandomSeed = 11131979;

	public const float virtualWidth = 1920.0f;

	public const int build = 23;

	public static void bootstrapServers(FGNetworkManager agent) {

		WWW www = new WWW (BootstrapURL);
		while (!www.isDone) {
			// oh, no!
		}
		string jsonRep = www.text;
		if (jsonRep.Equals ("")) {
			agent.bootstrapData = new BootStrapData (fallbackLoginServer,
				fallbackLoginServerPort,
				fallbackSocketServer,
				fallbackSocketServerPort,
				fallbackCommandServer,
				fallbackCommandServerPort);
		}
		else { agent.bootstrapData = JsonUtility.FromJson<BootStrapData> (jsonRep);

		}


	}

	// WARNING: not exhaustive
	public static bool isValideMail(string email) {

		string[] at = email.Split ('@');
		if (at.Length != 2)
			return false;
		string[] dots = email.Split ('.');
		if (dots.Length < 2)
			return false;
		return true;

	}

	public static Vector2 physicalToVirtualCoordinates(Vector2 phys) {

		Vector2 res = new Vector2 ();
		res.x = phys.x * (virtualWidth / Screen.width) - virtualWidth / 2.0f;
		res.y = phys.y * (virtualWidth / Screen.width) - (virtualWidth * Screen.height/Screen.width)/2.0f;

		return res;

	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public static int pseudoRandom(int index, int max) {
		Random.InitState (facesRandomSeed + index);
		return Random.Range (0, max);

	}

	public void mecagoentodo() {

	}

	public static char decToHexChar(int d) {
		switch (d) {
		case 0:
			return '0';
		case 1:
			return '1';
		case 2:
			return '2';
		case 3:
			return '3';
		case 4:
			return '4';
		case 5:
			return '5';
		case 6:
			return '6';
		case 7:
			return '7';
		case 8:
			return '8';
		case 9:
			return '9';
		case 10:
			return 'A';
		case 11:
			return 'B';
		case 12:
			return 'C';
		case 13:
			return 'D';
		case 14:
			return 'E';
		case 15:
			return 'F';
		}
		return '0';
	}

	public static string valueToHexstring(float v) {

		int iVal = (int)(v*255.0f);

		int lo = iVal & 15;
		int hi = (iVal >> 4) & 15;

		return "" + decToHexChar (hi) + decToHexChar (lo);

	}

	public static string chopSpaces(string s) {
		string[] strs = s.Split (' ');
		string res = strs [0];
		for (int i = 1; i < strs.Length; ++i) {
			res += "\n" + strs [i];
		}
		return res;
	}


	//public static float updateSoftVariable(float val, float target

	/*
	public static bool updateSoftVariable(ref float val, float target, float speed, System.Func<float, float> transf) {

	}

	public static bool updateSoftVariable(ref float val, float target, float speed) {

		bool hasChanged = false;

		if (val < (target-delta)) {
			val += speed * Time.deltaTime;
			hasChanged = true;
			if (val > target)
				val = target;
		}

		if (val > (target+delta)) {
			val -= speed * Time.deltaTime;
			hasChanged = true;
			if (val < target)
				val = target;
		}

		if (!hasChanged)
			val = target;

		return hasChanged;

	}*/



	/*public static void queueMessage(string msg) {

		string uuid = SystemInfo.deviceUniqueIdentifier;
		GameObject MailQueueGO = new GameObject ();
		MailQueueGO.name = "MailQueueAgent";
		MailQueueGO.AddComponent<QueueMailAgent> ().initialize (uuid, msg);
		DontDestroyOnLoad (MailQueueGO);


	}*/

	public static int pseudoRandom(int input) {

		int shiftBits = 7;
		int hash = 5381;
		int c;
		int i = 0;

		for (i = 0; i < (input+3); ++i) {
			c = i*3;
			hash = ((hash << shiftBits) + hash) + c; /* hash * 33 + c */
		}

		if (hash < 0)
			return -hash;
		return hash;

	}

	public static List<int> nTom(int n, int m) {

		int delta = 1;
		int start = n;
		List<int> res = new List<int>();
		int nElements = m - n;
		if (nElements < 0) {
			start = m;
			delta = -1;
			nElements = -nElements;
		}
		nElements++;
		for (int i = 0; i < nElements; ++i) {
			res.Add (start);
			start += delta;
		}
		return res;

	}

	public static List<int> scrambleList(List<int> l) {

		List<int> res = new List<int> ();
		int remain = l.Count;
		while (remain > 0) {
			int r = Random.Range (0, l.Count);
			while(res.Contains(l[r])) {
				r = Random.Range (0, l.Count);
			}
			res.Add(r);
			--remain;
		}
		return res;

	}

	public static float RangeRemap(float inValue, float rangeInMin, float rangeInMax, float rangeOutMin, float rangeOutMax) {
		return (rangeOutMin  +  (inValue - rangeInMin) / (rangeInMax - rangeInMin) * (rangeOutMax - rangeOutMin));
	}

}
