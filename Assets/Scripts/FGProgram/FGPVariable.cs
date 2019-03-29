using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FGPVariable : FGPExpression {

	private Object targetObject;
	private string propName;

	public Object getTargetObject() {
		return targetObject;
	}
	public string getVarName() {
		return propName;
	}

	public void setValueFromStringRep(string rep) {
		PropertyInfo pInfo = targetObject.GetType ().GetProperty (propName);
		System.Type theType = null;
		if (pInfo != null) {
			theType = pInfo.PropertyType;
		} else {
			theType = targetObject.GetType ().GetField (propName).FieldType;
		}
		if (theType != null) {
			if (theType == typeof(int)) {
				int iValue;
				int.TryParse (rep, out iValue);
				setValue (iValue);
			}
			if (theType == typeof(float)) {
				float fValue;
				float.TryParse(rep, out fValue);
				setValue(fValue);
			}
			if(theType == typeof(bool)) {
				bool bValue;
				bool.TryParse (rep, out bValue);
				setValue (bValue);
			}
			if (theType == typeof(string)) {
				setValue (rep);
			}
		}
	}

	public override void setValue(object v) {

		PropertyInfo pInfo = targetObject.GetType ().GetProperty (propName);
		if (pInfo != null) {
			pInfo.SetValue (targetObject, v, null);
		} else {
			targetObject.GetType ().GetField (propName).SetValue (targetObject, v);
		}

	}

	public override void setValue(FGPExpression expr) {

		PropertyInfo pInfo = targetObject.GetType ().GetProperty (propName);
		if (pInfo != null) {
			pInfo.SetValue (targetObject, expr.getValue(), null);
		} else {
			targetObject.GetType ().GetField (propName).SetValue (targetObject, expr.getValue());
		}

	}

	public override object getValue() {

		PropertyInfo pInfo = targetObject.GetType ().GetProperty (propName);
		if (pInfo != null) {
			return pInfo.GetValue (targetObject, null);
		} else {
			return targetObject.GetType ().GetField (propName).GetValue (targetObject);
		}

	}

	public FGPVariable(Object to, string pName) {
		targetObject = to;
		propName = pName;
	}

}
