using UnityEngine;
using System.Collections;

public class TimedObjectDestructor : MonoBehaviour {

	public float timeOut = 1.0f;
	public bool detachChildren = false;

	// invote the DestroyNow funtion to run after timeOut seconds
	void Awake () {
		Invoke ("DestroyNow", timeOut);
	}

	// destroy the gameobject
	void DestroyNow ()
	{
		if (detachChildren) { // detach the children before destroying if specified
			transform.DetachChildren ();
		}

		// destroy the game Object
		Destroy(gameObject);
	}
}
