using UnityEngine;
using System.Collections;

public class TargetExit : MonoBehaviour
{
	public float exitAfterSeconds = 10f; // how long to exist in the world
	public float exitAnimationSeconds = 1f; // should be >= time of the exit animation

	private bool startDestroy = false;
	private float targetTime;

	// Use this for initialization
	void Start ()
	{
		// set the targetTime to be the current time + exitAfterSeconds seconds
		targetTime = Time.time + exitAfterSeconds;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// continually check to see if past the target time
		if (Time.time >= targetTime) {
			if (this.GetComponent<Animator> () == null)
				// no Animator so just destroy right away
				Destroy (gameObject);
			else if (!startDestroy) {
				// set startDestroy to true so this code will not run a second time
				startDestroy = true;

				// trigger the Animator to make the "Exit" transition
				this.GetComponent<Animator> ().SetTrigger ("Exit");

				// Call KillTarget function after exitAnimationSeconds to give time for animation to play
				Invoke ("KillTarget", exitAnimationSeconds);
			}
		}
	}

	// destroy the gameObject when called
	void KillTarget ()
	{
		// remove the gameObject
		Destroy (gameObject);
	}
}
