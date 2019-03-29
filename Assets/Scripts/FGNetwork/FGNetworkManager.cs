/*
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;

public interface FGNetworkSendIndicator {

	void signalSend();

}

public interface FGNetworkCommandProcessor {

	void processCommand(string comm);

}

public interface FGNetworkAppPauseCallback {

	void pauseNetwork();
	void resumeNetwork();

}

[System.Serializable]
public class EnqueuedMessage {

	public int seq;
	public string dest;
	public string fullMessage;

}

public class FGNetworkManager : MonoBehaviour {

	public int totalSyncs = 0;

	public UIEnableImageOnTimeout noConnectionEnabler;

	public BootStrapData bootstrapData;

	public Dictionary <string, int> receiveSeq;
	public List<string> seenOrigins;
	public Dictionary <string, int> sendSeq;
	public List<EnqueuedMessage> sendList;

	[HideInInspector]
	public string bigLog = "";

	protected FGNetworkCommandProcessor commandProcessor;
	protected FGNetworkAppPauseCallback pauseCallback;
	protected FGNetworkSendIndicator sendIndicator;

	public ContinueGameController continueGameController_N;

	bool connected = false;

	public Queue<string> commandQueue;
	public List<string> commandHistory;

	bool initialized = false;


//	public int connectionId;
//	public int hostId;
//	public int reliableChannel;
	public int bytesRead;

	protected TcpClient tcpClient;
	NetworkStream ns;
	//SslStream sslNs;

	StreamWriter sw; 

	string GameServerURL;
	int GameServerPort;

	protected Thread marcoPoloThread;
	protected Thread readThread;
	public bool isMarcoThreadRunning = false;
	public bool isThreadRunning = false;
	bool dataAvailable = false;
	string readData;
	const int poloMs = 5000;
	public float poloElapsedTime;
	public const float poloTimeout = 10.0f;
	public bool tryingToReconnect = false;
	protected float reconnectElapsedTime = 0.0f;
	protected const float reconnectRetry = 4.0f;

	[HideInInspector]
	public int RMCindex = 0;

	//[HideInInspector]
	public string localLogin;
	[HideInInspector]
	public string roomID;



	[HideInInspector]
	public Dictionary<int, string> rmcResults;

	byte[] bytes;

	[HideInInspector]
	public bool firstConnectionStablished = false;


	public void initialize(string serverURL, int port) {

		if (initialized)
			return;

		//commandList = new List<string> ();
		commandQueue = new Queue<string> ();
		commandHistory = new List<string> ();

		GameServerURL = serverURL;
		GameServerPort = port;
		bytes = new byte[1024];

		isThreadRunning = false;
		isMarcoThreadRunning = false;



		initialized = true;

	}

	public int connect() {
		return connect (GameServerURL, GameServerPort);

	}

	public string consumeData() {

		string res;
		res = commandQueue.Dequeue ();
		return res;

	}

	public void marcoThreadCycle() {
		while (isMarcoThreadRunning) {
			Thread.Sleep (poloMs);
			sw.WriteLine ("marco");
			//sendIndicator.signalSend ();

			// resend all enqueued messages
			for (int i = 0; i < sendList.Count; ++i) {
				sendMessage (sendList[i].fullMessage);
			}

			//sw.Flush ();
		}
	}

	public void threadCycle() {

		while (isThreadRunning) {

			if (ns != null) {
				bytesRead = ns.Read (bytes, 0, bytes.Length);
				//int bytesRead = sslNs.Read(bytes, 0, bytes.Length);
				if (bytesRead > 0) {

					bytes [bytesRead] = 0;

					string newData = System.Text.Encoding.UTF8.GetString (bytes);
					//if (newData.EndsWith ("\\n"))
					newData = newData.Substring (0, bytesRead);

					//vomitNetworkOutput.text = newData;
					//commandList.Add (newData);

					bigLog += (System.DateTime.Now.ToString() + " " + newData);

					commandQueue.Enqueue (newData);
					commandHistory.Add (newData);

				} else {
				
					isThreadRunning = false;

				}
			} else {
				//Debug.Log ("For some reason, sslNs is null");
			}

		}
	}

	void OnDestroy() {
		isThreadRunning = false;
	}

	void OnApplicationPause( bool pauseStatus )
	{

		if (pauseStatus == true) {

			if (pauseCallback != null)
				pauseCallback.pauseNetwork ();
			else
				pauseNetwork ();

		}


		else if (pauseStatus == false) { // returning from pause

			if (pauseCallback != null)
				pauseCallback.resumeNetwork ();
			else
				resumeNetwork ();


		}


	}


	
	public void disconnect() {
		isThreadRunning = false;
		isMarcoThreadRunning = false;
		if(marcoPoloThread != null)
		marcoPoloThread.Join ();
		if (readThread != null)
		readThread.Join ();
		if(ns != null)
		ns.Close ();
		if (tcpClient != null)
		tcpClient.Close ();
		connected = false;
		noConnectionEnabler.stop ();
	}


	// connect method: can connect either to the wisdomini.flygames.org relay
	// or directly to another user??
	public int connect(string url, int port) {

//		GameServerPort = port;
//		GameServerURL = url;

		initialize (url, port);

		int result;
		try {
			tcpClient = new TcpClient (url, port);
			result = 0;
		}
		catch(SocketException e) {
			result = -1;
			return result;
		}

		X509Certificate2 clientCertificate = new X509Certificate2();
		X509Certificate2[] cerCol = { clientCertificate };
		X509CertificateCollection clientCertificateCollection = new X509Certificate2Collection (cerCol);
		//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

		ns = tcpClient.GetStream ();
		

		ns.ReadTimeout = Timeout.Infinite;
		sw = new StreamWriter (ns) {

			AutoFlush = true

		};
		sw.AutoFlush = true;
		readThread = new Thread (threadCycle); // create a new thread
		marcoPoloThread = new Thread(marcoThreadCycle); // create another thread
		isThreadRunning = true; // make the thread loop go!
		isMarcoThreadRunning = true; // set marco thread running too!
		try {

			readThread.Start ();

		}
		catch(ThreadStartException e) {
			// do nothing, really
			Debug.Log("readThread did not start!!!");
		}
		try {
			marcoPoloThread.Start();
		}
		catch(ThreadStartException e) {
			Debug.Log ("marcoThread did not start!!!");
		}

		//byte error;
		//connectionId = NetworkTransport.Connect(hostId, Utils.GameRelayServer, Utils.GameRelayPort, 0, out error);

		//return error;
		if (result == 0) {
			connected = true;
			noConnectionEnabler.go ();
		}
		return result;

	}

	public void idPlayer(string id) {
		sendMessage ("id " + id);

	}

	public void makeRoom(string roomname) {
		sendMessage ("makeroom " + roomname);
	}

	public void joinRoom(string roomname) {
		sendMessage ("joinroom " + roomname);
	}

	public int receiveSeqFor(string origin) {

		if(!seenOrigins.Contains(origin)) { // WARNING REMOVE
			seenOrigins.Add (origin);
		}

//		if (origin.Equals ("")) // prevent this (PATCH)
//			return 0;
		if((origin.IndexOf("@")) == -1) {
			//Debug.Log ("Strange commando: " + origin);
		}


		if (receiveSeq.ContainsKey (origin)) {
			return receiveSeq [origin];
		} else {
			receiveSeq [origin] = 0;
			return 0;
		}

	}

	public int sendSeqFor(string dest) {

		if (sendSeq.ContainsKey (dest)) {
			return sendSeq [dest];
		} else {
			sendSeq [dest] = 0;
			return 0;
		}

	}

	public void incReceiveSeqFor(string origin) {
		receiveSeq [origin]++;
	}

	public void incSendSeqFor(string dest) {
		sendSeq [dest]++;
	}

	// remove from sendlist once message has been acknowledged
	public void ack(int seq, string origin) {
		int rSeq; 
		rSeq = receiveSeqFor (origin);
		for (int i = 0; i < sendList.Count; ++i) {
			EnqueuedMessage msg = sendList [i];
			if ((msg.seq == seq) && (msg.dest.Equals (origin))) { 
				sendList.RemoveAt (i);
				--i;
			}
		}
	}

	public void sendCommand(string recipient, string command) {

		if (recipient.IndexOf('$') != -1)
			recipient = recipient.Substring (0, recipient.IndexOf('$'));
		int seq = sendSeqFor (recipient);
		string safeCommand = seq + "#" + localLogin + "#" + command;
		string fullMessage = "sendmessage " + recipient + " " + safeCommand;
		//Debug.Log ("ENQUEUE: " + fullMessage);
		sendMessage (fullMessage);
		incSendSeqFor (recipient);
		EnqueuedMessage newMessage = new EnqueuedMessage ();
		newMessage.seq = seq;
		newMessage.dest = recipient;
		newMessage.fullMessage = fullMessage;
		sendList.Add (newMessage);


	}

	public void sendCommandUnsafe(string recipient, string command) {

		sendMessage ("sendmessage " + recipient + " " + command);

	}

	public void sendMessage(string command) {

		if (sw == null)
			return;
		sw.WriteLine (command + "$"); // $ is end of command

		//sw.Flush ();

	}

	public void broadcast(string command) {

		// a bunch of individual sends
		foreach(var dest in receiveSeq.Keys)
		{

			if((!dest.Equals(localLogin)) && (!dest.Equals(""))) { // do not self-send

				sendCommand (dest, command);

			}
		}

	}

	public void rmc(string recipient, string method, params object[] p) {
		//network_N.sendCommand (recipient, FGNetworkManager.makeClientCommand ("rmc", execution_parameters[1], RMCindex, execution_parameters [4], execution_parameters [5]));
		string netparams = "";
		if (p.Length > 1) {
			int a;
			a = 666;
		}
		for (int i = 0; i < p.Length; ++i) {
			netparams += (p [i] + ":");
		}
		netparams += ":";
		if (recipient.Equals ("all") || recipient.Equals ("All")) {
			broadcast (FGNetworkManager.makeClientCommand ("rmc", true, -1, method, netparams));
		} else {
			sendCommand(recipient, FGNetworkManager.makeClientCommand("rmc", true, RMCindex, method, netparams));
			++RMCindex;
		}

	}

	public void broadcastUnsafe(string command) {
		sendMessage ("broadcast " + command);
	}
		

	// Use this for initialization
	void Start () {
		seenOrigins = new List<string> ();
		receiveSeq = new Dictionary<string, int> ();
		sendSeq = new Dictionary<string, int> ();
		sendList = new List<EnqueuedMessage> ();
		rmcResults = new Dictionary<int, string> ();
		//pauseCallback = this;
		commandProcessor = null;
		//sendIndicator = this;
		sw = null;
	}

	// Update is called once per frame
	protected void Update () {

		if (!initialized)
			return;

		if(connected) poloElapsedTime += Time.deltaTime;

		while (commandQueue.Count > 0) {


			string command = consumeData ();
			processCommand (command);
			if(commandProcessor != null) commandProcessor.processCommand(command);


		}

		if (tryingToReconnect) {
			reconnectElapsedTime += Time.deltaTime;
			if (reconnectElapsedTime > reconnectRetry) {
				reconnectElapsedTime = 0.0f;
				int res = connect ();
				if (res == 0) {
					tryingToReconnect = false;
					sendMessage ("initgame " + localLogin + " " + roomID);
				}
			}
		}

		if (poloElapsedTime > (poloTimeout)) {
			poloElapsedTime = 0.0f;
			disconnect ();
			tryingToReconnect = true;
			reconnectElapsedTime = reconnectRetry + 1.0f;
		}

	}

	public static string makeClientCommand(params object[] arg) 
	{
		string res = "";
		for (int i = 0; i < arg.Length; ++i) {
			res += (arg [i].ToString() + ":");
		}
		return res;
	}
		

	public int commandsReceived = 0;

	public Text debugText;


	public void pauseNetwork() {
		sendMessage ("endinvalidate");
		disconnect ();
		if(tcpClient != null) {
			while (tcpClient.Connected) {   } // OoO oh no, don't do it!!
		}
	}

	public void resumeNetwork() {
		if (tcpClient == null)
			connect ();
		if (tcpClient.Connected == false) {
			isMarcoThreadRunning = false;
			isThreadRunning = false;
			marcoPoloThread.Join ();
			readThread.Join (); // wait for threads to finish
			initGame();
		}
	}


	public void sendBigLog() {

		string timeDate = System.DateTime.Now.ToString ();;
		string idString = "\n\n\n\n\n==============================================\n\nEspejo//" + localLogin + "//" + timeDate + "\n\nBEGIN LOG\n";
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("data", idString + bigLog + "\nEND LOG\n\n"); 
		WWW myWWW = new WWW ("https://apps.flygames.org:50000/log", wwwform);
		while (myWWW.isDone) {		}  // I know this is not very sound, but fuck it!

		//controllerHub.masterController.nuke ();

	}

	public int sync = 0;
	public int syncTarget = 4; // this must equal the number of players

	public void resetSync() {
		//controllerHub.masterController.AppDebug ("ResetSync");
		sync = 0;
	}
	public void addSync() {
		++totalSyncs;
		//controllerHub.masterController.AppDebug ("Sync: " + sync);
		++sync;
	}
	public int getSync() {
		return sync;
	}
	public bool synced() {
		if (sync >= syncTarget) {
			sync = 0;
			return true;
		} else
			return false;
	}



	WWW www;
	WWWForm wwwForm;

	public void createHTTPRequest(string url, params string[] data) {

		WWWForm theForm = new WWWForm();
		for(int i = 0; i < data.Length/2; ++i) {
			theForm.AddField(data[i*2], data[i*2 + 1]);
		}
		if (data.Length == 0) {
			theForm.AddField ("dummy", "3.1416");
		}

		www = new WWW (url, theForm);

	}

	public void getFreshRoomID() {

		createHTTPRequest (bootstrapData.loginServer + ":" + bootstrapData.loginServerPort + FGUtils.GetFreshRoomID);

	}

	public void checkAppUser(string user, string passwd) {

		createHTTPRequest (bootstrapData.loginServer + ":" + bootstrapData.loginServerPort + FGUtils.CheckUserScript, "email", user, "passwd", passwd);

	}

	public bool loginHasBeenSet() {
		return !localLogin.Equals ("");
	}

	public void enterRoom() {
		sendMessage("initgame " + localLogin + " " + roomID);
	}

	public void initGame() {
		localLogin = "";
		connect (bootstrapData.socketServer, bootstrapData.socketServerPort);
		sendMessage ("roomuuid " + roomID);
		//sendMessage("initgame " + localLogin + " " + roomID);
	}

	public bool httpRequestIsDone() {

		return www.isDone;

	}

	public string wwwResult() {

		return www.text;

	}




	int comN = 0;

	public void processCommand(string comm) {

		//Debug.Log ("Processing command " + comm);

		comm = comm.Replace ("\n", ""); // remove any \n coming down the stream, we don't want them

		string[] commands = comm.Split ('$'); // split back to back commands

		char[] charcomm = comm.ToCharArray ();
		int nCommands = 0;
		for (int i = 0; i < charcomm.Length; ++i) {
			if (charcomm [i] == '$')
				++nCommands;
		}

		commandsReceived += nCommands;

		for (int i = 0; i < nCommands; ++i) {

			string command = commands [i];

			command = command.Trim ('\n'); // remove all \n 's


			int safeIndex = command.IndexOf ('#');
			string[] safeArg;
			bool processThisCommand = true;
			bool safeCommand = false;
			int safeSeqNum = -1;
			string originOfSafeCommand = "";
			int expectedSeq = -1;

			if (safeIndex != -1) { // safe command
				safeCommand = true;
				safeArg = command.Split('#');
				int.TryParse (safeArg [0], out safeSeqNum);
				command = safeArg [2];
				originOfSafeCommand = safeArg [1];
				int fix = originOfSafeCommand.IndexOf ('$');
				if (fix != -1) {
					//Debug.Log ("STRANGE ORIGIN: " + originOfSafeCommand);
					originOfSafeCommand = originOfSafeCommand.Substring (0, fix);
				}
				expectedSeq = receiveSeqFor (originOfSafeCommand);
				if (safeSeqNum != expectedSeq) {
					processThisCommand = false;
					//sendCommandUnsafe (originOfSafeCommand, "ACK:" + safeSeqNum + ":" + localUserLogin);
				}

			}

			if (processThisCommand) {


				string[] arg = command.Split (':');

				if (arg.Length > 0) {

					// We could replace this by a dictionary <string, Func> if we really wanted to

					if (command.StartsWith ("reportcontinue")) {
						int ttl;
						int.TryParse (arg [3], out ttl);
						if (continueGameController_N != null) {
							continueGameController_N.ReportContinue (arg [1], arg [2], ttl);
						}

					}
					else if (command.StartsWith ("rmcresult")) {
						int rmcIndex;
						int.TryParse (arg [1], out rmcIndex);
						if (rmcIndex >= 0) {
							rmcResults [rmcIndex] = arg [2];
						}
					}
					else if (command.StartsWith ("rmc")) {

						// rmc sync/async  rmcIndex    methodName	params
						//  0      1            2			   3		  4
						// 
						// set rmcIndex to -1 for no return

						//Debug.Log ("rmc received: " + command);

						int rmcIndex;
						int.TryParse (arg [2], out rmcIndex);
						string[] methodName = arg [3].Split ('.');
						List<string> strParams = new List<string> ();
						for (int k = 4; k < arg.Length; ++k) {
							strParams.Add(arg [k]);
						}
						GameObject go = GameObject.Find (methodName [0]);
						List<object> paramList = new List<object> ();
						Component destComp = go.GetComponent (methodName [1]);
						if (go != null) {
							Object obj = go;

							System.Reflection.MethodInfo[] allMethods = destComp.GetType ().GetMethods ();
							System.Reflection.MethodInfo m = destComp.GetType ().GetMethod (methodName [2]);

							System.Reflection.ParameterInfo[] paramInfo = m.GetParameters ();
							for(int k = 0; k < paramInfo.Length; ++k) {
								//System.Type myType = paramInfo [i].ParameterType;
								//System.Type myType2 = paramInfo [i].Attributes.GetType ();
								if (paramInfo [k].ParameterType == typeof(int)) {
									int intVal;
									int.TryParse (strParams [k], out intVal);
									paramList.Add (intVal);
								}
								if (paramInfo [k].ParameterType == typeof(string)) {
									paramList.Add (strParams [k]);
								}
								if (paramInfo [k].ParameterType == typeof(float)) {
									float floatVal;
									float.TryParse (strParams [k], out floatVal);
									paramList.Add (floatVal);
								}
							}
							object res = m.Invoke (destComp, paramList.ToArray ());
							if (rmcIndex != -1) {
								sendCommand (originOfSafeCommand, "rmcresult:" + rmcIndex + ":" + res + ":");
							}
						}

					}
					if (command.StartsWith ("roomplayers")) {

						int id = 1;
						while (!arg [id].Equals ("null")) {
							Debug.Log ("user " + arg [id] + " registered as roommate");
							receiveSeqFor (arg [id]); // register roommate for broadcast

							id++;

						}

					}
					if (command.StartsWith ("roomuuid")) {

						Debug.Log ("Roomuuid: " + arg [1]);
						localLogin = arg [1];

					}
					if (command.StartsWith ("ACK")) {

						//string[] fields = newData.Split (':');
						int sq;
						int.TryParse (arg [1], out sq);
						string fromUser = arg [2].TrimEnd ('$', '\n');
						ack (sq, fromUser);
						if (fromUser.IndexOf ('$') != -1) {
							//Debug.Log ("STRANGE ORIGIN:  " + fromUser);
						}
					}
					if (command.StartsWith ("polo")) {
						poloElapsedTime = 0.0f;
						noConnectionEnabler.keepAlive ();
					}
					if (safeCommand) { // if safe command, acknowledge processing

						incReceiveSeqFor (originOfSafeCommand);
						sendCommandUnsafe (originOfSafeCommand, "ACK:" + safeSeqNum + ":" + localLogin);
					}
				}
			}
		}
	}
}

*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Threading;

public interface FGNetworkSendIndicator {

	void signalSend();

}

public interface FGNetworkCommandProcessor {

	void processCommand(string comm);

}

public interface FGNetworkAppPauseCallback {

	void pauseNetwork();
	void resumeNetwork();

}

[System.Serializable]
public class EnqueuedMessage {

	public int seq;
	public string dest;
	public string fullMessage;

}

public class FGNetworkManager : MonoBehaviour {

	public int totalSyncs = 0;

	//public UIEnableImageOnTimeout noConnectionEnabler;

	public BootStrapData bootstrapData;

	public Dictionary <string, int> receiveSeq;
	public List<string> seenOrigins;
	public Dictionary <string, int> sendSeq;
	public List<EnqueuedMessage> sendList;

	[HideInInspector]
	public string bigLog = "";

	protected FGNetworkCommandProcessor commandProcessor;
	protected FGNetworkAppPauseCallback pauseCallback;
	protected FGNetworkSendIndicator sendIndicator;

	//public ContinueGameController continueGameController_N;

	bool connected = false;

	public Queue<string> commandQueue;
	public List<string> commandHistory;

	bool initialized = false;


	//	public int connectionId;
	//	public int hostId;
	//	public int reliableChannel;
	public int bytesRead;

	protected TcpClient tcpClient;
	NetworkStream ns;
	//SslStream sslNs;

	StreamWriter sw; 

	string GameServerURL;
	int GameServerPort;

	protected Thread marcoPoloThread;
	protected Thread readThread;
	public bool isMarcoThreadRunning = false;
	public bool isThreadRunning = false;
	bool dataAvailable = false;
	string readData;
	const int poloMs = 5000;
	public float poloElapsedTime;
	public const float poloTimeout = 10.0f;
	public bool tryingToReconnect = false;
	protected float reconnectElapsedTime = 0.0f;
	protected const float reconnectRetry = 4.0f;

	[HideInInspector]
	public int RMCindex = 0;


	public string localLogin;
	[HideInInspector]
	public string roomID;



	[HideInInspector]
	public Dictionary<int, string> rmcResults;

	byte[] bytes;

	[HideInInspector]
	public bool firstConnectionStablished = false;


	public void initialize(string serverURL, int port) {

		if (initialized)
			return;

		//commandList = new List<string> ();
		commandQueue = new Queue<string> ();
		commandHistory = new List<string> ();

		GameServerURL = serverURL;
		GameServerPort = port;
		bytes = new byte[1024];

		isThreadRunning = false;
		isMarcoThreadRunning = false;



		initialized = true;

	}

	public int connect() {
		return connect (GameServerURL, GameServerPort);

	}

	public string consumeData() {

		string res;
		res = commandQueue.Dequeue ();
		return res;

	}

	public void marcoThreadCycle() {
		while (isMarcoThreadRunning) {
			Thread.Sleep (poloMs);
			sw.WriteLine ("marco");
			//sendIndicator.signalSend ();

			// resend all enqueued messages
			for (int i = 0; i < sendList.Count; ++i) {
				sendMessage (sendList[i].fullMessage);
			}

			//sw.Flush ();
		}
	}

	public void threadCycle() {

		while (isThreadRunning) {

			if (ns != null) {
				bytesRead = ns.Read (bytes, 0, bytes.Length);
				//int bytesRead = sslNs.Read(bytes, 0, bytes.Length);
				if (bytesRead > 0) {

					bytes [bytesRead] = 0;

					string newData = System.Text.Encoding.UTF8.GetString (bytes);
					//if (newData.EndsWith ("\\n"))
					newData = newData.Substring (0, bytesRead);

					//vomitNetworkOutput.text = newData;
					//commandList.Add (newData);

					bigLog += (System.DateTime.Now.ToString() + " " + newData);

					commandQueue.Enqueue (newData);
					commandHistory.Add (newData);

				} else {

					isThreadRunning = false;

				}
			} else {
				//Debug.Log ("For some reason, sslNs is null");
			}

		}
	}

	void OnDestroy() {
		isThreadRunning = false;
	}

	void OnApplicationPause( bool pauseStatus )
	{

		if (pauseStatus == true) {

			if (pauseCallback != null)
				pauseCallback.pauseNetwork ();
			else
				pauseNetwork ();

		}


		else if (pauseStatus == false) { // returning from pause

			if (pauseCallback != null)
				pauseCallback.resumeNetwork ();
			else
				resumeNetwork ();


		}


	}


	/*
	 * Disconnect from the server!
	 */
	public void disconnect() {
		isThreadRunning = false;
		isMarcoThreadRunning = false;
		if(marcoPoloThread != null)
			marcoPoloThread.Join ();
		if (readThread != null)
			readThread.Join ();
		if(ns != null)
			ns.Close ();
		if (tcpClient != null)
			tcpClient.Close ();
		connected = false;
		//noConnectionEnabler.stop ();
	}


	// connect method: can connect either to the wisdomini.flygames.org relay
	// or directly to another user??
	public int connect(string url, int port) {

		//		GameServerPort = port;
		//		GameServerURL = url;

		initialize (url, port);

		int result;
		try {
			tcpClient = new TcpClient (url, port);
			result = 0;
		}
		catch(SocketException e) {
			result = -1;
			return result;
		}

		X509Certificate2 clientCertificate = new X509Certificate2();
		X509Certificate2[] cerCol = { clientCertificate };
		X509CertificateCollection clientCertificateCollection = new X509Certificate2Collection (cerCol);
		//System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

		ns = tcpClient.GetStream ();
		/*sslNs = new SslStream (tcpClient.GetStream (), true, new RemoteCertificateValidationCallback
			(
				(srvPoint, certificate, chain, errors) => true//MyRemoteCertificateValidationCallback(srvPoint, certificate, chain, errors)
			));
		sslNs.AuthenticateAsClient (FGUtils.flygamesSSLAuthHost, clientCertificateCollection, SslProtocols.Tls, true);//, clientCertificateCollection, SslProtocols.Tls, false);
		//System.Net.ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors ) => true;
		System.Net.ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
			(
				(srvPoint, certificate, chain, errors) => true//MyRemoteCertificateValidationCallback(srvPoint, certificate, chain, errors)
			);
		*/

		ns.ReadTimeout = Timeout.Infinite;
		sw = new StreamWriter (ns) {

			AutoFlush = true

		};
		sw.AutoFlush = true;
		readThread = new Thread (threadCycle); // create a new thread
		marcoPoloThread = new Thread(marcoThreadCycle); // create another thread
		isThreadRunning = true; // make the thread loop go!
		isMarcoThreadRunning = true; // set marco thread running too!
		try {

			readThread.Start ();

		}
		catch(ThreadStartException e) {
			// do nothing, really
			Debug.Log("readThread did not start!!!");
		}
		try {
			marcoPoloThread.Start();
		}
		catch(ThreadStartException e) {
			Debug.Log ("marcoThread did not start!!!");
		}

		//byte error;
		//connectionId = NetworkTransport.Connect(hostId, Utils.GameRelayServer, Utils.GameRelayPort, 0, out error);

		//return error;
		if (result == 0) {
			connected = true;
			//noConnectionEnabler.go ();
		}
		return result;

	}

	public void idPlayer(string id) {
		sendMessage ("id " + id);

	}

	public void makeRoom(string roomname) {
		sendMessage ("makeroom " + roomname);
	}

	public void joinRoom(string roomname) {
		sendMessage ("joinroom " + roomname);
	}

	public int receiveSeqFor(string origin) {

		if(!seenOrigins.Contains(origin)) { // WARNING REMOVE
			seenOrigins.Add (origin);
		}

		//		if (origin.Equals ("")) // prevent this (PATCH)
		//			return 0;
		//		if((origin.IndexOf("@")) == -1) {
		//			Debug.Log ("Strange commando: " + origin);
		//		}


		if (receiveSeq.ContainsKey (origin)) {
			return receiveSeq [origin];
		} else {
			receiveSeq [origin] = 0;
			return 0;
		}

	}

	public int sendSeqFor(string dest) {

		if (sendSeq.ContainsKey (dest)) {
			return sendSeq [dest];
		} else {
			sendSeq [dest] = 0;
			return 0;
		}

	}

	public void incReceiveSeqFor(string origin) {
		receiveSeq [origin]++;
	}

	public void incSendSeqFor(string dest) {
		sendSeq [dest]++;
	}

	// remove from sendlist once message has been acknowledged
	public void ack(int seq, string origin) {
		int rSeq; 
		rSeq = receiveSeqFor (origin);
		for (int i = 0; i < sendList.Count; ++i) {
			EnqueuedMessage msg = sendList [i];
			if ((msg.seq == seq) && (msg.dest.Equals (origin))) { 
				sendList.RemoveAt (i);
				--i;
			}
		}
	}

	public void sendCommand(string recipient, string command) {

		if (recipient.IndexOf('$') != -1)
			recipient = recipient.Substring (0, recipient.IndexOf('$'));
		int seq = sendSeqFor (recipient);
		string safeCommand = seq + "#" + localLogin + "#" + command;
		string fullMessage = "sendmessage " + recipient + " " + safeCommand;
		//Debug.Log ("ENQUEUE: " + fullMessage);
		sendMessage (fullMessage);
		incSendSeqFor (recipient);
		EnqueuedMessage newMessage = new EnqueuedMessage ();
		newMessage.seq = seq;
		newMessage.dest = recipient;
		newMessage.fullMessage = fullMessage;
		sendList.Add (newMessage);


	}

	public void sendCommandUnsafe(string recipient, string command) {

		sendMessage ("sendmessage " + recipient + " " + command);

	}

	public void sendMessage(string command) {

		if (sw == null)
			return;
		sw.WriteLine (command + "$"); // $ is end of command

		//sw.Flush ();

	}

	public void broadcast(string command) {

		// a bunch of individual sends
		foreach(var dest in receiveSeq.Keys)
		{

			if((!dest.Equals(localLogin)) && (!dest.Equals(""))) { // do not self-send

				sendCommand (dest, command);

			}
		}

	}

	public void rmc(string recipient, string method, params object[] p) {
		//network_N.sendCommand (recipient, FGNetworkManager.makeClientCommand ("rmc", execution_parameters[1], RMCindex, execution_parameters [4], execution_parameters [5]));
		string netparams = "";
		if (p.Length > 1) {
			int a;
			a = 666;
		}
		for (int i = 0; i < p.Length; ++i) {
			netparams += (p [i] + ":");
		}
		netparams += ":";
		if (recipient.Equals ("all") || recipient.Equals ("All")) {
			broadcast (FGNetworkManager.makeClientCommand ("rmc", true, -1, method, netparams));
		} else {
			sendCommand(recipient, FGNetworkManager.makeClientCommand("rmc", true, RMCindex, method, netparams));
			++RMCindex;
		}

	}

	public void broadcastUnsafe(string command) {
		sendMessage ("broadcast " + command);
	}


	// Use this for initialization
	void Start () {
		seenOrigins = new List<string> ();
		receiveSeq = new Dictionary<string, int> ();
		sendSeq = new Dictionary<string, int> ();
		sendList = new List<EnqueuedMessage> ();
		rmcResults = new Dictionary<int, string> ();
		//pauseCallback = this;
		commandProcessor = null;
		//sendIndicator = this;
		sw = null;
	}

	// Update is called once per frame
	protected void Update () {

		if (!initialized)
			return;

		if(connected) poloElapsedTime += Time.deltaTime;

		while (commandQueue.Count > 0) {


			string command = consumeData ();
			processCommand (command);
			if(commandProcessor != null) commandProcessor.processCommand(command);


		}

		if (tryingToReconnect) {
			reconnectElapsedTime += Time.deltaTime;
			if (reconnectElapsedTime > reconnectRetry) {
				reconnectElapsedTime = 0.0f;
				int res = connect ();
				if (res == 0) {
					tryingToReconnect = false;
					enterRoom ();
				}
			}
		}

		if (poloElapsedTime > (poloTimeout)) {
			poloElapsedTime = 0.0f;
			disconnect ();
			tryingToReconnect = true;
			reconnectElapsedTime = reconnectRetry + 1.0f;
		}

	}

	public static string makeClientCommand(params object[] arg) 
	{
		string res = "";
		for (int i = 0; i < arg.Length; ++i) {
			res += (arg [i].ToString() + ":");
		}
		return res;
	}


	public int commandsReceived = 0;

	public Text debugText;


	public void pauseNetwork() {
		sendMessage ("endinvalidate");
		disconnect ();
		if(tcpClient != null) {
			while (tcpClient.Connected) {   } // OoO oh no, don't do it!!
		}
	}

	public void resumeNetwork() {
		if (tcpClient == null)
			connect ();
		if (tcpClient.Connected == false) {
			isMarcoThreadRunning = false;
			isThreadRunning = false;
			marcoPoloThread.Join ();
			readThread.Join (); // wait for threads to finish
			//initGame();
			connectAndEnterRoom();
		}
	}


	public void sendBigLog() {

		string timeDate = System.DateTime.Now.ToString ();;
		string idString = "\n\n\n\n\n==============================================\n\nEspejo//" + localLogin + "//" + timeDate + "\n\nBEGIN LOG\n";
		WWWForm wwwform = new WWWForm ();
		wwwform.AddField ("data", idString + bigLog + "\nEND LOG\n\n"); 
		WWW myWWW = new WWW ("https://apps.flygames.org:50000/log", wwwform);
		while (myWWW.isDone) {		}  // I know this is not very sound, but fuck it!

		//controllerHub.masterController.nuke ();

	}

	public int sync = 0;
	public int syncTarget = 4; // this must equal the number of players

	public void resetSync() {
		//controllerHub.masterController.AppDebug ("ResetSync");
		sync = 0;
	}
	public void addSync() {
		++totalSyncs;
		//controllerHub.masterController.AppDebug ("Sync: " + sync);
		++sync;
	}
	public int getSync() {
		return sync;
	}
	public bool synced() {
		if (sync >= syncTarget) {
			sync = 0;
			return true;
		} else
			return false;
	}



	WWW www;
	WWWForm wwwForm;

	public void createHTTPRequest(string url, params string[] data) {

		WWWForm theForm = new WWWForm();
		for(int i = 0; i < data.Length/2; ++i) {
			theForm.AddField(data[i*2], data[i*2 + 1]);
		}
		if (data.Length == 0) {
			theForm.AddField ("dummy", "3.1416");
		}

		www = new WWW (url, theForm);

	}

	public void getFreshRoomID() {

		createHTTPRequest (bootstrapData.loginServer + ":" + bootstrapData.loginServerPort + FGUtils.GetFreshRoomID);

	}

	public void checkAppUser(string user, string passwd) {

		createHTTPRequest (bootstrapData.loginServer + ":" + bootstrapData.loginServerPort + FGUtils.CheckUserScript, "email", user, "passwd", passwd);

	}

	public bool loginHasBeenSet() {
		return !localLogin.Equals ("");
	}

	public void connectAndEnterRoom() {
		connect (bootstrapData.socketServer, bootstrapData.socketServerPort);
		enterRoom ();
	}

	public void enterRoom() {
		Debug.Log ("Entering room " + roomID + " as " + localLogin);
		sendMessage("initgame " + localLogin + " " + roomID);
	}

	public void initGame() {
		localLogin = "";
		connect (bootstrapData.socketServer, bootstrapData.socketServerPort);
		sendMessage ("roomuuid " + roomID);
		//sendMessage("initgame " + localLogin + " " + roomID);
	}

	public bool httpRequestIsDone() {

		return www.isDone;

	}

	public string wwwResult() {

		return www.text;

	}




	int comN = 0;

	public void processCommand(string comm) {

		//Debug.Log ("Processing command " + comm);

		comm = comm.Replace ("\n", ""); // remove any \n coming down the stream, we don't want them

		string[] commands = comm.Split ('$'); // split back to back commands

		char[] charcomm = comm.ToCharArray ();
		int nCommands = 0;
		for (int i = 0; i < charcomm.Length; ++i) {
			if (charcomm [i] == '$')
				++nCommands;
		}

		commandsReceived += nCommands;

		for (int i = 0; i < nCommands; ++i) {

			string command = commands [i];

			command = command.Trim ('\n'); // remove all \n 's


			int safeIndex = command.IndexOf ('#');
			string[] safeArg;
			bool processThisCommand = true;
			bool safeCommand = false;
			int safeSeqNum = -1;
			string originOfSafeCommand = "";
			int expectedSeq = -1;

			if (safeIndex != -1) { // safe command
				safeCommand = true;
				safeArg = command.Split('#');
				int.TryParse (safeArg [0], out safeSeqNum);
				command = safeArg [2];
				originOfSafeCommand = safeArg [1];
				int fix = originOfSafeCommand.IndexOf ('$');
				if (fix != -1) {
					//Debug.Log ("STRANGE ORIGIN: " + originOfSafeCommand);
					originOfSafeCommand = originOfSafeCommand.Substring (0, fix);
				}
				expectedSeq = receiveSeqFor (originOfSafeCommand);
				if (safeSeqNum != expectedSeq) {
					processThisCommand = false;
					//sendCommandUnsafe (originOfSafeCommand, "ACK:" + safeSeqNum + ":" + localUserLogin);
				}

			}

			if (processThisCommand) {


				string[] arg = command.Split (':');

				if (arg.Length > 0) {

					// We could replace this by a dictionary <string, Func> if we really wanted to

					if (command.StartsWith ("reportcontinue")) {
						int ttl;
						int.TryParse (arg [3], out ttl);
//						if (continueGameController_N != null) {
//							continueGameController_N.ReportContinue (arg [1], arg [2], ttl);
//						}

					}
					else if (command.StartsWith ("rmcresult")) {
						int rmcIndex;
						int.TryParse (arg [1], out rmcIndex);
						if (rmcIndex >= 0) {
							rmcResults [rmcIndex] = arg [2];
						}
					}
					else if (command.StartsWith ("rmc")) {

						// rmc sync/async  rmcIndex    methodName	params
						//  0      1            2			   3		  4
						// 
						// set rmcIndex to -1 for no return

						//Debug.Log ("rmc received: " + command);

						int rmcIndex;
						int.TryParse (arg [2], out rmcIndex);
						string[] methodName = arg [3].Split ('.');
						List<string> strParams = new List<string> ();
						for (int k = 4; k < arg.Length; ++k) {
							strParams.Add(arg [k]);
						}
						GameObject go = GameObject.Find (methodName [0]);
						List<object> paramList = new List<object> ();
						Component destComp = go.GetComponent (methodName [1]);
						if (go != null) {
							Object obj = go;

							System.Reflection.MethodInfo[] allMethods = destComp.GetType ().GetMethods ();
							System.Reflection.MethodInfo m = destComp.GetType ().GetMethod (methodName [2]);

							System.Reflection.ParameterInfo[] paramInfo = m.GetParameters ();
							for(int k = 0; k < paramInfo.Length; ++k) {
								//System.Type myType = paramInfo [i].ParameterType;
								//System.Type myType2 = paramInfo [i].Attributes.GetType ();
								if (paramInfo [k].ParameterType == typeof(int)) {
									int intVal;
									int.TryParse (strParams [k], out intVal);
									paramList.Add (intVal);
								}
								if (paramInfo [k].ParameterType == typeof(string)) {
									paramList.Add (strParams [k]);
								}
								if (paramInfo [k].ParameterType == typeof(float)) {
									float floatVal;
									float.TryParse (strParams [k], out floatVal);
									paramList.Add (floatVal);
								}
							}
							object res = m.Invoke (destComp, paramList.ToArray ());
							if (rmcIndex != -1) {
								sendCommand (originOfSafeCommand, "rmcresult:" + rmcIndex + ":" + res + ":");
							}
						}

					}
					if (command.StartsWith ("roomplayers")) {

						int id = 1;
						while (!arg [id].Equals ("null")) {
							Debug.Log ("user " + arg [id] + " registered as roommate");
							receiveSeqFor (arg [id]); // register roommate for broadcast

							id++;

						}

					}
					if (command.StartsWith ("roomuuid")) {
//						if (!continueGameController_N.tryingToContinue) {
//							Debug.Log ("Roomuuid: " + arg [1]);
//							localLogin = arg [1];
//						}

					}
					if (command.StartsWith ("ACK")) {

						//string[] fields = newData.Split (':');
						int sq;
						int.TryParse (arg [1], out sq);
						string fromUser = arg [2].TrimEnd ('$', '\n');
						ack (sq, fromUser);
						if (fromUser.IndexOf ('$') != -1) {
							//Debug.Log ("STRANGE ORIGIN:  " + fromUser);
						}
					}
					if (command.StartsWith ("polo")) {
						poloElapsedTime = 0.0f;
						//noConnectionEnabler.keepAlive ();
					}
					if (safeCommand) { // if safe command, acknowledge processing

						incReceiveSeqFor (originOfSafeCommand);
						sendCommandUnsafe (originOfSafeCommand, "ACK:" + safeSeqNum + ":" + localLogin);
					}
				}
			}
		}
	}
}

