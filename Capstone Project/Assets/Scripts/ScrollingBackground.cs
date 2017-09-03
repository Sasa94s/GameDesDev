using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public bool scrolling, parallax;    // checkbox for using scrolling or parallax behaviour
    public float backgroundSize;        // the size of the texture or scrolling background has to be set manually
    public float parallaxSpeed;         // smoothing of parallaxing


    private Transform cameraTransform;  // transform object of the camera
    private Transform[] layers;         // layers to be scrolled
    private float viewZone;             // view zone for detecting the limits for scrolling
    private int leftIndex;
    private int rightIndex;
    private float lastCameraX;          // holding last x position of camera

    void Awake ()
	{
        // defining camera transform
	    cameraTransform = Camera.main.transform;
        
	    lastCameraX = cameraTransform.position.x;
        // number of childs of the background empty object
        layers = new Transform[transform.childCount];
	    for (int i = 0; i < transform.childCount; i++)
	    {
	        layers[i] = transform.GetChild(i);
	    }
	    leftIndex = 0;
	    rightIndex = layers.Length - 1;
	    viewZone = 10.0f;
	}
	
	void Update ()
	{
	    if (parallax)
	    {
	        float deltaX = cameraTransform.position.x - lastCameraX;
            transform.position += Vector3.right * (deltaX * parallaxSpeed);
        }
	    lastCameraX = cameraTransform.position.x;
        if (scrolling)
	    {
	        if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + viewZone))
	            ScrollLeft();
	        if (cameraTransform.position.x > (layers[rightIndex].transform.position.x - viewZone))
	            ScrollRight();
        }
    }

    private void ScrollLeft()
    {
        layers[rightIndex].position = new Vector3(layers[leftIndex].position.x - backgroundSize, layers[leftIndex].position.y, layers[leftIndex].position.z);
        leftIndex = rightIndex;
        --rightIndex;
        if (rightIndex < 0)
            rightIndex = layers.Length - 1;

    }

    private void ScrollRight()
    {
        layers[leftIndex].position = new Vector3(layers[rightIndex].position.x + backgroundSize, layers[rightIndex].position.y, layers[rightIndex].position.z);
        rightIndex = leftIndex;
        ++leftIndex;
        if (leftIndex == layers.Length)
            leftIndex = 0;
    }
}
