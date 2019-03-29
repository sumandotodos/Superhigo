using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGPCondition  {

	FGPROExpression left;
	string comparator;
	FGPROExpression right;

	public FGPCondition(FGPROExpression e1, string comp, FGPROExpression e2) {
		left = e1;
		comparator = comp;
		right = e2;
	}

	public bool eval() {
		object value1, value2;
		value1 = left.getValue ();
		value2 = right.getValue ();
		if (value1 is int) {
			return (compareValues ((int)value1, (int)value2, comparator));
		}
		if (value1 is float) {
			return (compareValues ((float)value1, (float)value2, comparator));
		}
		if (value1 is string) {
			return (compareValues ((string)value1, (string)value2, comparator));
		}
		if (value1 is bool) {
			return (compareValues ((bool)value1, (bool)value2, comparator));
		}

		return false;
				
	}

	private bool compareValues(int v1, int v2, string comp) {

		if (comp.Equals (">")) {
			return v1 > v2;
		}
		if (comp.Equals ("<")) {
			return v1 < v2;
		}
		if (comp.Equals ("==")) {
			return v1 == v2;
		}
		if (comp.Equals ("!=")) {
			return v1 != v2;
		}
		if (comp.Equals (">=")) {
			return v1 >= v2;
		}
		if (comp.Equals ("<=")) {
			return v1 <= v2;
		}
		return false;

	}
	private bool compareValues(float v1, float v2, string comp) {

		if (comp.Equals (">")) {
			return v1 > v2;
		}
		if (comp.Equals ("<")) {
			return v1 < v2;
		}
		if (comp.Equals ("==")) {
			return v1 == v2;
		}
		if (comp.Equals ("!=")) {
			return v1 != v2;
		}
		if (comp.Equals (">=")) {
			return v1 >= v2;
		}
		if (comp.Equals ("<=")) {
			return v1 <= v2;
		}
		return false;

	}
	private bool compareValues(bool v1, bool v2, string comp) {


		if (comp.Equals ("==")) {
			return v1 == v2;
		}
		if (comp.Equals ("!=")) {
			return v1 != v2;
		}
		return false;

	}
	private bool compareValues(string v1, string v2, string comp) {


		if (comp.Equals ("==")) {
			return v1.Equals(v2);
		}
		if (comp.Equals ("!=")) {
			return v1.Equals(v2);
		}
		return false;

	}

}
