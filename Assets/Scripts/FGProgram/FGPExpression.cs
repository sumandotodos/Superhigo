using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FGPExpression : FGPROExpression {

	public abstract void setValue (object newValue);
	public abstract void setValue (FGPExpression newValue);

}
