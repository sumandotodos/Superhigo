using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 
 *    FGProgram.cs
 * 
 *    V1.1  2017  Emilio Pomares
 * 
 * 
 */

public enum FGPHandlerType { variable, task };

public class FGPHandler {

	public int id;
	public int startPC;
	public int endPC;
	public FGPHandlerType hType;

	public FGPHandler(int newId, int start, int end) {
		id = newId;
		startPC = start;
		endPC = end;
		hType = FGPHandlerType.task;
	}

	public FGPHandler(int newId, int instr) {
		id = newId;
		startPC = instr;
		endPC = instr;
		hType = FGPHandlerType.task;
	}

	public FGPHandler(int newId) {
		id = newId;
		startPC = endPC = -1;
		hType = FGPHandlerType.variable;
	}



}

public class FGProgram : MonoBehaviour {

	// Things to watch in inspector:

	//[HideInInspector]
	public List<FGProgram> waiters;
	//[HideInInspector]
	public int waitCounter;
	public int _state; // private



	public const string CONTINUE = "";
	//private List<FGPLocalVariable>;
	//List<FGPLocalVariable<float>> floatVars;
	List<FGPConstant> constants;
	List<FGProgram> otherPrograms;
	List<FGPLocalVariable> lVariables;
	List<FGPVariable> variables;
	List<FGPMethodCall> methodCalls;
	List<FGPExpression> expressions;
	List<FGPROExpression> roExpressions;
	List<FGPCondition> conditions;
	Dictionary<string, int> subprograms;

	//[HideInInspector]
	public string sReturnValue;
	//[HideInInspector]
	public int iReturnValue;
	//[HideInInspector]
	public object oReturnValue;

	public FGNetworkManager network_N;

	public string currentSubprogramName = ""; // protected



	FGPLocalVariable lastResult;

	Dictionary <string, System.Action> executionCore;

	Stack<int> PCStack;
	Dictionary<int, int> LoopExitPC;
	Dictionary<int, int> LoopEntryPC;
	bool whileMustContinue;



	// DELAY_state vars
	float timer;
	float delayValue;

	// WAITCOND vars
	int waitExpr1, waitExpr2;
	int waitCond;
	string waitComp;

	// WAITRMC vars
	int resultVar;
	int RMCindex = 0;
	[HideInInspector]
	public List<int> waitingForRMC;

	// these are arbitrary
	const int DELAY_state = 100;
	const int WAITCOND = 200;
	const int WAITRMC_state = 400;
	const int FINISHED = 999;
	const int WAITPRG = 300;
	const int SYNCING = 500;
	const int NEXTINSTRUCTION = 1;
	const int STOPPED = 0;

	int delayExecuteMethodCall = -1;

	string[] execution_parameters;

	// core execution functions
	private void halt_execute() {
		_state = STOPPED;
		//notifyFinish ();
		release = true;
	}
	private void returnstring_execute() {
		returnString (execution_parameters [1]);
		release = false;
	}
	private void returnint_execute() {
		int retVal;
		int.TryParse (execution_parameters [1], out retVal);
		returnInteger (retVal);
		release = false;
	}
	private void returnvar_execute() {
		int varIndex;
		int.TryParse (execution_parameters [1], out varIndex);
		returnobject (variables [varIndex].getValue ());
		release = false;
	}
	private void notifyend_execute() {
		notifyFinish ();
		release = true;
	}
	private void if_execute() {
		int c;
		int.TryParse (execution_parameters [1], out c);
		bool cResult = conditions [c].eval ();
		if (cResult) {
			if (execution_parameters [2].Equals ("_continue")) {
				// PC is unchanged	
			} else {
				int destPC = subprograms [execution_parameters [2]];
				currentSubprogramName = execution_parameters[2];
				PC = destPC - 1;
				release = false;
			}
		} else {
			if (execution_parameters [3].Equals ("_continue")) {
				// PC is unchanged	
			} else {
				int destPC = subprograms [execution_parameters [3]];
				currentSubprogramName = execution_parameters[3];
				PC = destPC - 1;
				release = false;
			}
		}
	}
	private void loop_execute() {
		PC = 0;
	}
	private void logmc_execute() {
		int methodIndex;
		int.TryParse (execution_parameters [1], out methodIndex);
		FGPMethodCall m = methodCalls [methodIndex];
		string res = (string)m.getValue ();

	}
	private void log_execute() {
		Debug.Log (execution_parameters [1]);
	}
	private void goto_execute() {
		currentSubprogramName = execution_parameters[1];
		int destPC = subprograms [execution_parameters [1]];
		_state = NEXTINSTRUCTION;
		PC = destPC - 1;
		release = false;
	}
	private void delay_execute() {
		delayExecuteMethodCall = -1;
		_state = DELAY_state;
		float del;
		float.TryParse (execution_parameters [1], out del);
		delayValue = del;
		release = true;
	}
	private void rmc_execute() {
		if (!network_N) {
			Debug.Log ("Warning: cannot execute RMC: network component not set");
			return;
		}
		int varIndex;
		int.TryParse (execution_parameters [3], out resultVar);
		int.TryParse (execution_parameters [2], out varIndex);
		string recipient = (string)variables [varIndex].getValue ();
		network_N.rmc (recipient, execution_parameters [4], execution_parameters [5]);
		//network_N.sendCommand (recipient, FGNetworkManager.makeClientCommand ("rmc", execution_parameters[1], RMCindex, execution_parameters [4], execution_parameters [5]));
		//++RMCindex;

		if (execution_parameters [1].Equals ("sync")) {
			_state = WAITRMC_state;
			release = true;
		} else {
			release = false;
		}
		if (!(recipient.Equals ("all") || recipient.Equals ("All"))) {
			waitingForRMC.Add (network_N.RMCindex - 1);
		}
	}
	private void delayExec_execute() {
		int mc;
		int.TryParse (execution_parameters [2], out mc);
		delayExecuteMethodCall = mc;
		_state = DELAY_state;
		float del;
		float.TryParse (execution_parameters [1], out del);
		delayValue = del;
		release = true;
	}
	private void waitProgram_execute() {
		_state = WAITPRG;
		int p;
		int.TryParse (execution_parameters [1], out p);
		otherPrograms[p].registerWaiter(this); // WARNING decidir cómo se aplica registerWaiter
		waitCounter++;
		if(!otherPrograms[p].isRunning()) {
			otherPrograms [p].run (); // start the other program
		}
		release = true;
	}
	private void retrAndWaitProgram_execute() {
		_state = WAITPRG;
		int pVar;
		int.TryParse (execution_parameters [1], out pVar);
		FGProgram pToExecute;
		pToExecute = (FGProgram)variables [pVar].getValue ();
		pToExecute.registerWaiter(this); // WARNING decidir cómo se aplica registerWaiter
		waitCounter++;
		if(!pToExecute.isRunning()) {
			pToExecute.run (); // start the other program
		}
		release = true;
	}
	private void startProgram_execute() {
		_state = NEXTINSTRUCTION;
		int p;
		int.TryParse (execution_parameters [1], out p);
		otherPrograms [p].run (); // start the other program
		release = false;
	}
	private void waitCondition_execute() {
		int c1;
		int.TryParse (execution_parameters [1], out c1);
		waitCond = c1;
		_state = WAITCOND;
		release = true;
	}
	private void callWait_execute() {

		int m;
		int.TryParse (execution_parameters [1], out m);
		methodCalls [m].getValue (); // check type!
		release = true;
	}
	private void callMethod_execute() {

		int m;
		int.TryParse (execution_parameters [1], out m);
		methodCalls [m].getValue (); // check type!
		release = false;
	}
	private void startTask_execute() {

		release = true;

	}
	private void delayeval_execute() {
		int e;
		int.TryParse (execution_parameters [1], out e);
		FGPExpression expr = expressions [e];
		float delaytime = (float)expr.getValue ();
		delayExecuteMethodCall = -1;
		_state = DELAY_state;
		delayValue = delaytime;
		release = true;
	}
	private void opassign_execute() {

		int v, e;
		int.TryParse (execution_parameters [1], out v);
		int.TryParse (execution_parameters [2], out e);
		FGPVariable var = variables [v];
		FGPROExpression exp = roExpressions [e];
		string op = execution_parameters [3];
		if (op.Equals ("=")) {

			object destVal = var.getValue ();
			object srcVal = exp.getValue ();

			if (destVal is int) {
				if (srcVal is int) {
					var.setValue (exp.getValue ());
				}
				if (srcVal is string) {
					int iVal;
					int.TryParse ((string)srcVal, out iVal);
					var.setValue (iVal);
				}
			}
			if (destVal is string) {
				if (srcVal is int) {
					var.setValue ("" + (int)exp.getValue ());
				}
				if (srcVal is string) {
					var.setValue (exp.getValue ());
				}
			}

		} 

		else if (op.Equals ("+")) {
			
			object destVal = var.getValue ();
			object srcVal = exp.getValue ();
			if (destVal is int) { // WARNING:  complete matrix
				if (srcVal is int) {
					var.setValue (((int)var.getValue ()) + ((int)exp.getValue ()));
				} else if (srcVal is float) {
					var.setValue (((int)var.getValue ()) + (int)((float)exp.getValue ()));
				}
			} else if (destVal is float) {
				var.setValue (((float)var.getValue ()) + ((float)exp.getValue ()));
			} else if (destVal is string) {
				if (srcVal is int) {
					var.setValue (((string)var.getValue ()) + ((int)exp.getValue ()));
				} else if (srcVal is float) {
					var.setValue (((string)var.getValue ()) + ((float)exp.getValue ()));
				} else if (srcVal is string) {
					var.setValue (((string)var.getValue ()) + ((string)exp.getValue ()));
				} else if (srcVal is bool) {
					string bStr = "false";
					if ((bool)srcVal)
						bStr = "true";
					var.setValue (((string)var.getValue ()) + bStr);
				}

			}
		}

		else if (op.Equals ("-")) {

			object destVal = var.getValue ();
			object srcVal = exp.getValue ();
			if (destVal is int) { // WARNING:  complete matrix
				if (srcVal is int) {
					var.setValue (((int)var.getValue ()) - ((int)exp.getValue ()));
				} else if (srcVal is float) {
					var.setValue (((int)var.getValue ()) - (int)((float)exp.getValue ()));
				}
			} else if (destVal is float) {
				var.setValue (((float)var.getValue ()) - ((float)exp.getValue ()));
			} 
		}

	}
	private void startWhile_execute() {

		int c;
		int.TryParse(execution_parameters[1], out c);
		bool executeBody = conditions [c].eval ();
		if (!executeBody) { // skip body
			PC = LoopExitPC [PC]; // lookup exit PC for this loop
		}
			
		release = false;

	}
	private void endWhile_execute() {

		PC = LoopEntryPC [PC] - 1;
		release = true;

	}



	List<string> program;


	bool release;

	int PC;

	int taskID;


	// Use this for initialization
	public FGProgram () {

		program = new List<string> ();
		constants = new List<FGPConstant> ();
		methodCalls = new List<FGPMethodCall> ();
		expressions = new List<FGPExpression> ();
		roExpressions = new List<FGPROExpression> ();
		otherPrograms = new List<FGProgram> ();
		conditions = new List<FGPCondition> ();
		subprograms = new Dictionary<string, int> ();
		lVariables = new List<FGPLocalVariable> ();
		variables = new List<FGPVariable> ();
		waiters = new List<FGProgram> ();
		waitingForRMC = new List<int> ();
		PCStack = new Stack<int>();
		LoopExitPC = new Dictionary<int, int>();
		LoopEntryPC = new Dictionary<int, int> ();
		currentSubprogramName = "_main";
		PC = 0;
		waitCounter = 0;
		taskID = 0;
		timer = 0.0f;
		executionCore = new Dictionary<string, System.Action> ();
		_state = STOPPED;
		executionCore.Add ("delay", delay_execute);
		executionCore.Add ("delayeval", delayeval_execute);
		executionCore.Add ("delayexec", delayExec_execute);
		executionCore.Add ("call", callMethod_execute);
		executionCore.Add ("callwait", callWait_execute);
		executionCore.Add ("waitcond", waitCondition_execute);
		executionCore.Add ("startwhile", startWhile_execute);
		executionCore.Add ("endwhile", endWhile_execute); 
		executionCore.Add ("waitprogram", waitProgram_execute);
		executionCore.Add ("startprogram", startProgram_execute);
		executionCore.Add ("notifyend", notifyend_execute);
		executionCore.Add ("halt", halt_execute);
		executionCore.Add ("goto", goto_execute);
		executionCore.Add ("if", if_execute);
		executionCore.Add ("log", log_execute);
		executionCore.Add ("logmc", logmc_execute);
		executionCore.Add ("loop", loop_execute);
		executionCore.Add ("opassign", opassign_execute);
		executionCore.Add ("rmc", rmc_execute);
		executionCore.Add ("returnstring", returnstring_execute);
		executionCore.Add ("returnint", returnint_execute);
		executionCore.Add ("returnvar", returnvar_execute);
		executionCore.Add ("retrieveandwaitprogram", retrAndWaitProgram_execute);


	}

	public void waitFinish() {
		if(waitCounter > 0)
		--waitCounter;
	}

	public void registerWaiter(FGProgram waiter) {
		waiters.Add (waiter);
	}

	public bool isFinished() {
		return _state == FINISHED;
	}

	public bool isRunning() {
		return !((_state == FINISHED) || (_state == STOPPED));
	}

	public void notifyFinish(string res) {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].sReturnValue = res;
		}
		notifyFinish ();
	}

	public void notifyFinish(int res) {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].iReturnValue = res;
		}
		notifyFinish ();
	}

	public void notifyFinish() {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].waitFinish ();
		}
		waiters = new List<FGProgram> ();
	}
	public void returnString(string s) {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].sReturnValue = s;
		}
	}
	public void returnInteger(int v) {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].iReturnValue = v;
		}
	}
	public void returnobject(object o) {
		for (int i = 0; i < waiters.Count; ++i) {
			waiters [i].oReturnValue = o;
		}
	}

	void Update() {
		update ();
	}
	
	// Update is called once per frame
	public void update () {

		release = false;



		if (waitingForRMC.Count > 0) {
			for (int k = 0; k < waitingForRMC.Count; ++k) {
				if (network_N.rmcResults.ContainsKey (waitingForRMC[k])) {
					string res = network_N.rmcResults [waitingForRMC[k]];
					network_N.rmcResults.Remove (waitingForRMC[k]);
					if ((!res.Equals ("")) && resultVar != -1) {
						variables [resultVar].setValueFromStringRep (res);
					}
					waitingForRMC.RemoveAt (k);
					--k;
				}
			}
			if ((waitingForRMC.Count == 0) && _state == WAITRMC_state) {
				_state = NEXTINSTRUCTION;
			}
		}

		if (_state == WAITRMC_state) {
			return;
		}

		if (_state == STOPPED) {
			return;
		}

		if (_state == FINISHED) {
			return;
		}

		if (_state == NEXTINSTRUCTION) {
			string instr;
			while ((PC < program.Count) && (!release)) {
				instr = program [PC];
				execution_parameters = instr.Split(' ');
				if(executionCore.ContainsKey(execution_parameters[0])) {
					executionCore [execution_parameters [0]] ();
				}
				++PC;
			}
			if (PC == program.Count) {
				_state = STOPPED;
				//notifyFinish ();
				return;
			}
		}


		// built-in _states
		if (_state == DELAY_state) {
			if (delayExecuteMethodCall != -1) {
				methodCalls [delayExecuteMethodCall].getValue ();
			}
			timer += Time.deltaTime;
			if (timer > delayValue) {
				_state = NEXTINSTRUCTION;
				timer = 0.0f;
			}
		}

		if (_state == WAITCOND) {
			FGPCondition c = conditions [waitCond];
			if (c.eval ())
				_state = NEXTINSTRUCTION;

		}

		if (_state == WAITPRG) {
			
			if (waitCounter == 0)
				_state = NEXTINSTRUCTION;

		}

	}

	public void delay(float time, FGPMethodCall mCall) {
		methodCalls.Add (mCall);
		string newInstr = "delayexec " + time + " " + (methodCalls.Count - 1);
		program.Add (newInstr);
	}

	// delays, but executing the passed function every update
	public void delay(float time, Object obj, string mName, params string[] parameters) {
		FGPMethodCall newMCall = new FGPMethodCall (obj, mName, parameters);
		delay (time, newMCall);
	}

	public void delay(float time) {
		string newInstr = "delay " + time;
		program.Add (newInstr);
	}





	public FGPConstant constant(int initial) {
		FGPConstant newConst= new FGPConstant (initial);
		constants.Add (newConst);
		return newConst;
	}

	public FGPConstant constant(float initial) {
		FGPConstant newConst= new FGPConstant (initial);
		constants.Add (newConst);
		return newConst;
	}

	public FGPConstant constant(string initial) {
		FGPConstant newConst= new FGPConstant (initial);
		constants.Add (newConst);
		return newConst;
	}

	public void startWhile(float fValue, string comparator, Object obj, string mName, params string[] parameters) {
		startWhile (new FGPCondition(new FGPConstant (fValue), comparator, new FGPMethodCall (obj, mName, parameters)));
	}
	public void startWhile(int iValue, string comparator, Object obj, string mName, params string[] parameters) {
		startWhile (new FGPCondition(new FGPConstant (iValue), comparator, new FGPMethodCall (obj, mName, parameters)));
	}
	public void startWhile(Object obj, string mName, params string[] parameters) {
		startWhile (new FGPCondition(new FGPConstant (true), "==", new FGPMethodCall (obj, mName, parameters)));
	}

	public void startWhile(FGPCondition cond) {
		conditions.Add (cond);
		int c = conditions.Count - 1;
		string newInstr = "startwhile " + c;
		PCStack.Push (program.Count); // store beginning of the loop PC
		program.Add (newInstr);

	}

	public void endWhile() {
		string newInstr = "endwhile";
		int openingWhile = PCStack.Pop ();
		LoopExitPC [openingWhile] = program.Count; // store end of loop PC
		LoopEntryPC[program.Count] = openingWhile; // bidirectionally
		program.Add (newInstr);
	}

	/// <summary>
	/// Executes a single method call
	/// </summary>
	/// <param name="mc">The FGPMethodCall to call</param>
	public void execute(FGPMethodCall mc) {
		methodCalls.Add (mc);
		string newStr = "call "+(methodCalls.Count-1);
		program.Add (newStr);
	}

	public void executeWait(FGPMethodCall mc) {
		methodCalls.Add (mc);
		string newStr = "callwait "+(methodCalls.Count-1);
		program.Add (newStr);
	}

	public FGPHandler variable(int iVal) {
		FGPLocalVariable newVar = new FGPLocalVariable (iVal);
		lVariables.Add (newVar);
		return new FGPHandler (lVariables.Count - 1);
	}
	public FGPHandler variable(float fVal) {
		FGPLocalVariable newVar = new FGPLocalVariable (fVal);
		lVariables.Add (newVar);
		return new FGPHandler (lVariables.Count - 1);
	}
	public FGPHandler variable(bool bVal) {
		FGPLocalVariable newVar = new FGPLocalVariable (bVal);
		lVariables.Add (newVar);
		return new FGPHandler (lVariables.Count - 1);
	}
	public FGPHandler variable(string sVal) {
		FGPLocalVariable newVar = new FGPLocalVariable (sVal);
		lVariables.Add (newVar);
		return new FGPHandler (lVariables.Count - 1);
	}

	public FGPHandler add(FGPHandler val, int iVal) {
		if (val.hType != FGPHandlerType.variable) {
			Debug.Log ("Warning: wrong handler type at instruction " + program.Count);
			return val;
		}
		int varVal = (int)lVariables [val.id].getValue();
		lVariables [val.id].setValue (varVal + iVal);
		return val;
	}
	public FGPHandler add(FGPHandler val, float fVal) {
		if (val.hType != FGPHandlerType.variable) {
			Debug.Log ("Warning: wrong handler type at instruction " + program.Count);
			return val;
		}
		float varVal = (float)lVariables [val.id].getValue();
		lVariables [val.id].setValue (varVal + fVal);
		return val;
	}

	public void programNotifyFinish( FGPVariable var) {
		variables.Add (var);
		string newInstr = "returnvar " + (variables.Count-1);
		program.Add (newInstr);
		programNotifyFinish ();
	}
	public void programNotifyFinish(string retString) {
		string newInstr = "returnstring " + retString;
		program.Add (newInstr);
		programNotifyFinish ();
	}
	public void programNotifyFinish(int retInt) {
		string newInstr = "returnint " + retInt;
		program.Add (newInstr);
		programNotifyFinish ();
	}
	public void programNotifyFinish() {
		string newInstr = "notifyend";
		program.Add (newInstr);
	}

	/// <summary>
	/// Executes a single method call
	/// </summary>
	/// <param name="obj">The object that encapsulates the method</param>
	/// <param name="mName">Name of the method</param>
	/// <param name="parameters">List of method parameters</param>
	public void execute(Object obj, string mName, params object[] parameters) {
		execute(new FGPMethodCall(obj, mName, parameters));
	}

	public FGPHandler startProgram(FGProgram otherProgram) {
		otherPrograms.Add (otherProgram);
		int p = otherPrograms.Count - 1;
		string newInstr = "startprogram " + p;
		program.Add (newInstr);
		return new FGPHandler (p, program.Count - 1);
	}

	public void waitForProgram(FGPVariable p) {
		variables.Add (p);
		string newInstr = "retrieveandwaitprogram " + (variables.Count - 1);
		program.Add (newInstr);
	}

	public FGPHandler waitForProgram(FGPHandler handler) {
		if (otherPrograms [handler.id].isFinished())
			return handler;
		string newInstr = "waitprogram " + handler.id;
		program.Add (newInstr);
		return handler;
	}

	public void waitForProgramImmediate(FGProgram otherProgram) {
		_state = WAITPRG;

		otherProgram.registerWaiter(this); 
		waitCounter++;
		if(!otherProgram.isRunning()) {
			otherProgram.run (); // start the other program
		}
		release = true;
	}

	public FGPHandler waitForProgram(FGProgram otherProgram) {
		otherPrograms.Add (otherProgram);
		int p = otherPrograms.Count - 1;
		string newInstr = "waitprogram " + p;
		program.Add (newInstr);
		return new FGPHandler (p, program.Count - 1);
	}

	public void createSubprogram(string name) {
		name = name.Replace ("_", ""); // _ are forbidden
		string newInstr = "halt";
		program.Add (newInstr);
		subprograms [name] = program.Count;
	}

	public void programGoTo(string name) {
		name = name.Replace ("_", "");
		string newInstr = "goto " + name;
		program.Add (newInstr);
	}

	public void goTo(string name) {
		name = name.Replace ("_", "");
		if (subprograms.ContainsKey (name)) {
			currentSubprogramName = name;
			int destPC = subprograms [name];
			_state = NEXTINSTRUCTION;
			PC = destPC;
			release = false;
		}
	}

	public void loop() {
		program.Add ("loop");
	}

	public void debug(string message) {
		program.Add ("log " + message);
	}

	public void debug(Object obj, string mName, params object[] parameters) {
		FGPMethodCall newCall = new FGPMethodCall (obj, mName, parameters);
		//debug (newCall);
	}
	public void debug(FGPMethodCall m) {
		methodCalls.Add (m);
		string instr = "logmc " + (methodCalls.Count-1);
		program.Add(instr);
	}

	public string subprogramName() {
		return currentSubprogramName;
	}

	public FGPHandler waitForTask(FGPMethodCall m) {
		execute (m);
		return waitForProgram ((FGProgram)m.getTargetObject());
	}

	public FGPHandler waitForTask(FGProgram obj, string mName, params object[] parameters) {
		
		FGPMethodCall newCall = new FGPMethodCall (obj, mName, parameters);
		return waitForTask (newCall);

	}

	public void programIf(string trueSubprogram, string falseSubprogram, string sValue, string comparator, Object obj, string mName, params object[] parameters) {
		programIf(new FGPCondition(new FGPConstant (sValue), comparator, new FGPMethodCall (obj, mName, parameters)), trueSubprogram, falseSubprogram);
	}
	public void programIf(string trueSubprogram, string falseSubprogram, float fValue, string comparator, Object obj, string mName, params object[] parameters) {
		programIf(new FGPCondition(new FGPConstant (fValue), comparator, new FGPMethodCall (obj, mName, parameters)), trueSubprogram, falseSubprogram);
	}
	public void programIf(string trueSubprogram, string falseSubprogram, int iValue, string comparator, Object obj, string mName, params object[] parameters) {
		programIf(new FGPCondition(new FGPConstant (iValue), comparator, new FGPMethodCall (obj, mName, parameters)), trueSubprogram, falseSubprogram);
	}
	public void programIf(string trueSubprogram, string falseSubprogram, bool bValue, string comparator, Object obj, string mName, params object[] parameters) {
		programIf(new FGPCondition(new FGPConstant (bValue), comparator, new FGPMethodCall (obj, mName, parameters)), trueSubprogram, falseSubprogram);
	}

	public void programIf(FGPCondition cond, string trueSubprogram, string falseSubprogram) {
		conditions.Add (cond);
		int c = conditions.Count - 1;
		if (falseSubprogram.Equals (""))
			falseSubprogram = "_continue";
		if (trueSubprogram.Equals (""))
			trueSubprogram = "_continue";
		string newInstr = "if " + c + " " + trueSubprogram + " " + falseSubprogram;
		program.Add(newInstr);
	}


	public FGPHandler waitForCondition( FGPVariable var ) {
		return waitForCondition (var.getTargetObject (), var.getVarName ());
	}
	public FGPHandler waitForCondition(Object obj, string mName, params object[] parameters) {
		return waitForCondition (true, "==", obj, mName, parameters);
	}
	public FGPHandler waitForCondition(bool bValue, string comparator, Object obj, string mName, params object[] parameters) {
		// decide if method call or variable access
		System.Reflection.MethodInfo m = obj.GetType().GetMethod(mName);
		if (m != null) {
			return waitForCondition (new FGPCondition (new FGPConstant (bValue), comparator, new FGPMethodCall (obj, mName, parameters)));
		} else {
			return waitForCondition (new FGPCondition (new FGPConstant (bValue), comparator, new FGPVariable (obj, mName)));
		}
	}

	public FGPHandler waitForCondition( FGPMethodCall m) {
		return waitForCondition (new FGPCondition(new FGPConstant (true), "==", m));
	}

	public FGPHandler waitForCondition(int iValue, string comparator, Object obj, string mName, params object[] parameters) {
		return waitForCondition (new FGPCondition(new FGPConstant (iValue), comparator, new FGPMethodCall (obj, mName, parameters)));
	}

	public FGPHandler waitForCondition(float fValue, string comparator, Object obj, string mName, params object[] parameters) {
		return waitForCondition (new FGPCondition(new FGPConstant (fValue), comparator, new FGPMethodCall (obj, mName, parameters)));
	}

	public FGPHandler waitForCondition(FGPCondition cond) {
		conditions.Add (cond);
		int c1 = conditions.Count - 1;
		string newInstr = "waitcond " + c1;
		program.Add (newInstr);
		return new FGPHandler (taskID++, program.Count-1);
	}

	public FGPHandler startTask(FGPMethodCall m) {
		return new FGPHandler (taskID++, program.Count);
	}

	public void delay(FGPExpression time) {
		
		expressions.Add (time);
		int e = expressions.Count - 1;
		string newInstr = "delayeval " + e;
		program.Add (newInstr);

	}
		

	public void opAssign(FGPVariable dest, FGPROExpression srcExpr, string op) {
		variables.Add (dest);
		roExpressions.Add (srcExpr);
		int v = variables.Count - 1;
		int e = roExpressions.Count - 1;
		string newInstr = "opassign " + v + " " + e + " " + op;
		program.Add (newInstr);
	}

	public void opAssign(Object obj, string propName, object cvalue, string op) {
		opAssign (new FGPVariable (obj, propName), new FGPConstant(cvalue), op);
	}

	public void opAssign(Object destObj, string destPName, Object srcObj, string srcPName, string op) {
		opAssign (new FGPVariable (destObj, destPName), new FGPVariable (srcObj, srcPName), op);
	}

	public void RMC(bool sync, FGPVariable dest, FGPVariable resultVar_N, FGPMethodCall mCall) {
		int d;
		variables.Add (dest);
		d = variables.Count - 1;
		int v = -1;
		if (resultVar_N != null) {
			variables.Add (resultVar_N);
			v = variables.Count - 1;
		} 
		string netParams = "";
		for (int i = 0; i < mCall.getParamsArray().Length; ++i) {
			netParams += mCall.getParamsArray()[i].ToString ();
		}
		string newInstr = "rmc ";
		if (!sync)
			newInstr += "a";
		newInstr += ("sync " + d + " " + v + " " + mCall.getTargetObject().name + "." + mCall.getTargetObject().GetType().Name + "." + mCall.getMethodName() + " " + netParams);
		program.Add (newInstr);
	}

	public void RMCSync(FGPVariable dest, FGPVariable resultVar_N, FGPMethodCall mCall) { 
		RMC (true, dest, resultVar_N, mCall);
	}

	public void RMCAsync(FGPVariable dest, FGPVariable resultVar_N, FGPMethodCall mCall) {
		RMC (false, dest, resultVar_N, mCall);
	}

	public string allTarget = "all";

	public void sync() {

		if (network_N != null) {
			execute (new FGPMethodCall (network_N, "addSync"));
			RMCAsync (new FGPVariable (this, "allTarget"), null, new FGPMethodCall (network_N, "addSync")); // sync others
			waitForCondition (new FGPMethodCall (network_N, "synced"));
		} else
			Debug.Log ("WARNING: Trying to sync with networkController disconnected!");
			
	}



	public void run() {
		PC = 0;
		_state = NEXTINSTRUCTION;
	}

	public void cancelDelay() {
		if (_state == DELAY_state) {
			timer = delayValue;
		}
	}

	public void cancelWaitForCondition() {
		if (_state == WAITCOND) {
			_state = NEXTINSTRUCTION;
		}
	}

	public void cancelWaitForProgram() {
		if (_state == WAITPRG) {
			_state = NEXTINSTRUCTION;
		}
		waiters = new List<FGProgram>();
		waitCounter = 0;
	}



}
