using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FGPConstant : FGPROExpression {

	private object constantValue;

	public override object getValue() {
		return constantValue;
	}
		
	public FGPConstant(object value) {
		constantValue = value;
	}
		
}
