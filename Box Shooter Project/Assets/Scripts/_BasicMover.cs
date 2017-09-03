using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _BasicMover : MonoBehaviour {

    public float spinSpeed = 180.0f;
    public float motionMagnitude = 0.1f;
    public enum motionOptions {Horizontal, Vertical, zPlane, Round, Test1, Test2}
    public motionOptions motionState = motionOptions.Horizontal;
    public bool doSpin;
    public bool doMotion;

	
	// Update is called once per frame
	void Update () {
        if(doSpin)
            gameObject.transform.Rotate(Vector3.up * spinSpeed * Time.deltaTime);
        if(doMotion)
        {
            switch (motionState)
            {
                case motionOptions.Horizontal:
                    gameObject.transform.Translate(Vector3.right * motionMagnitude * Mathf.Cos(Time.timeSinceLevelLoad));
                    break;
                case motionOptions.Vertical:
                    gameObject.transform.Translate(Vector3.up * motionMagnitude * Mathf.Cos(Time.timeSinceLevelLoad));
                    break;
                case motionOptions.zPlane:
                    gameObject.transform.Translate(Vector3.forward * motionMagnitude * Mathf.Cos(Time.timeSinceLevelLoad));
                    break;
                case motionOptions.Test1:
                    gameObject.transform.Rotate(Vector3.forward * motionMagnitude * Mathf.Tan(Time.timeSinceLevelLoad));
                    break;
                case motionOptions.Test2:
                    gameObject.transform.Translate(Vector3.forward * motionMagnitude * Mathf.Sin(Time.timeSinceLevelLoad));
                    break;
                case motionOptions.Round:
                    gameObject.transform.Rotate(Vector3.forward * motionMagnitude * Mathf.Round(Time.timeSinceLevelLoad));
                    break;
            }
        }
    }
}
