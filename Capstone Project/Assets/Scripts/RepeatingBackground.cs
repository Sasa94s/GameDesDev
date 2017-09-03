using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{

    private SpriteRenderer groundCollider;
    private float groundHorizontalLength;

	// Use this for initialization
	void Start ()
	{
	    groundCollider = GetComponent<SpriteRenderer>();
	    groundHorizontalLength = groundCollider.size.x;

	}
	
	// Update is called once per frame
	void Update () {
	    if (transform.position.x < -groundHorizontalLength)
	    {
	        RepositionBackground();
            
	    }
	}

    private void RepositionBackground()
    {
        Vector2 groundOffset = new Vector2(groundHorizontalLength * 2f, 0);
        transform.position = (Vector2)transform.position + groundOffset;
    }
}
