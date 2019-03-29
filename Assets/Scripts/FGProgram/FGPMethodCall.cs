using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class FGPMethodCall : FGPROExpression {

	private Object targetObject;
	private string methodName;
	private List<string> paramsStr;
	private List<object> paramsObj;
	private object[] paramsArray;


	public override object getValue() {

		MethodInfo m = targetObject.GetType ().GetMethod (methodName);
		ParameterInfo[] paramInfoList = m.GetParameters ();

		List<object> nativeParamList = new List<object> ();
	

		return m.Invoke (targetObject, paramsArray); // nativeParamList.ToArray ());

	}

	public object[] getParamsArray() {
		return paramsArray;
	}

	public string getMethodName() {
		return methodName;
	}

	public Object getTargetObject() {
		return targetObject;
	}

	public FGPMethodCall(Object obj, string mName, params object[] parameters) {
		//System.Type ttt = averquepasa.GetType ();
		//	constantValue = cValue;
		targetObject = obj;
		methodName = mName;
		paramsObj = new List<object> ();
		MethodInfo m = targetObject.GetType ().GetMethod (mName);

		type = systemTypeToFGType (m.ReturnType);
		//for (int i = 0; i < parameters.Length; ++i) {

		//	paramsObj.Add (parameters [i]);
		//}
		paramsArray = parameters;
	}
//
//	public static void letMeSeeShit(System.Ac llamada) {
//		System.Type ttt = llamada.GetType();
//		string nnn = ttt.Name;
//	}

	public FGBasicTypes systemTypeToFGType(System.Type t) {
		if (t.Name.Equals ("Int32")) {
			return FGBasicTypes.Int;
		} else if (t.Name.Equals ("Single")) {
			return FGBasicTypes.Float;
		} else if (t.Name.Equals ("String")) {
			return FGBasicTypes.String;
		} else if (t.Name.Equals ("Boolean")) {
			return FGBasicTypes.Bool;
		}
		return FGBasicTypes.Void;
	}


}
