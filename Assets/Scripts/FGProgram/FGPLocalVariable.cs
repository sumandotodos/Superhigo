using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FGPLocalVariable : FGPExpression {

	private object value;


	public override object getValue() {
		return value;
	}

	public override void setValue(object v) {
		value = v;
	}

	public override void setValue(FGPExpression expr) {
		value = expr.getValue ();
	}

	public FGPLocalVariable(object value) {
		value = value;
	}



}
