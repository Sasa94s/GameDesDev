using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    // Infinite Platform Generator
    // Creating an empty game object child for camera and set position away from x-axis around 20 "PlatformGeneratorPoint"
    // Creating an empty game object in scene and set position centered at the second platform "PlatformGenerator" and attach the script to it

    public ObjectPooler[] theObjectPools;      // platform to be generated
    public Transform generationPoint;   // point to be generated at

    // is the distance between platforms constant or random?
    public bool constantDistance;
    public float distanceBetween = 3.0f;       // distance between each generated platform

    public bool randomDistance;
    public float minimumDistance = 3.0f;        // minimum distance between each generated platform
    public float maximumDistance = 6.0f;        // maximum distance between each generated platform

    private float[] platformWidth;        // x size of platform

    private int platformSelector;               // index of pooled platforms

    private float minHeight;
    public Transform maxHeightPoint;
    private float maxHeight;
    public float maxHeightChange;
    private float heightChange;

    

	void Awake ()
	{
        platformWidth = new float[theObjectPools.Length];
        
	}

    void Start()
    {
        // getting size of platform from the box collider
        for (int i = 0; i < theObjectPools.Length; i++)
        {
            platformWidth[i] = theObjectPools[i].pooledObject.GetComponent<BoxCollider2D>().size.x;
        }

        minHeight = transform.position.y;
        maxHeight = maxHeightPoint.position.y;
    }
	
	void Update () {

        if (transform.position.x < generationPoint.position.x)
	    {
	        platformSelector = Random.Range(0, theObjectPools.Length);
            if (randomDistance)
	        {
	            distanceBetween = Random.Range(minimumDistance, maximumDistance);
	        }

            // changing height in random range relevant to the position of platform generator
	        heightChange = transform.position.y + Random.Range(maxHeightChange, -maxHeightChange);
            
            // if the height change is above the max or below the min
	        if (heightChange > maxHeight)
	        {
	            heightChange = maxHeight;
	        } else if (heightChange < minHeight)
	        {
	            heightChange = minHeight;
	        }

	        transform.position = new Vector3(transform.position.x + (platformWidth[platformSelector] / 2) + distanceBetween, heightChange, transform.position.z);

            // Instantiating a platform in a specific position based on PlatformGenerationPoint and DistanceBetween
            // Issue: lags for friction of seconds due to high resources consumption
            //Instantiate(theObjectPools[platformSelector].pooledObject, transform.position, transform.rotation);
            GameObject newPlatform = theObjectPools[platformSelector].GetPooledObject();
	        
            newPlatform.transform.position = transform.position;
	        newPlatform.transform.rotation = transform.rotation;
	        
            newPlatform.SetActive(true);
	        // Randomizing coins on platform
	        newPlatform.GetComponent<CoinPlatform>().randomizeCoins();
            transform.position = new Vector3(transform.position.x + (platformWidth[platformSelector] / 2) + distanceBetween, transform.position.y, transform.position.z);

        }
    }
}
