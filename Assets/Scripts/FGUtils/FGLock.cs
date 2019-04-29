using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FGLock : MonoBehaviour {

	bool Locked = false;

	public abstract void action ();
	public virtual void attempt() {
		if (Locked)
			return;
		Locked = true;
		action ();
	}
	public virtual void reset() {
		Locked = false;
	}
	public virtual void Lock() {
		Locked = true;
	}

}

/*
 * 
 * 
 * vs
 * 
 * 
 * public class FGLock : MonoBehaviour {

	bool Locked = false;

	System.Func<GameObject, bool> action;
	GameObject target;

	public void setAction(System.Func<GameObject, bool> _action, GameObject _target) {
		action = _action;
		target = _target;
	}

	public void attempt() {
		if (Locked)
			return;
		Locked = true;
		action (target);
	}
	public void reset() {
		Locked = false;
	}

}


*/
