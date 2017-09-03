using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDestroyer : MonoBehaviour
{
    private Transform _player;       // to hold player position
    private float platformWidth;    // to hold platform width

	void Awake ()
	{
        // finding player game object
	    _player = FindObjectOfType<CharacterController2D>().transform;

        // getting platform width value
	    platformWidth = GetComponent<BoxCollider2D>().size.x;
	}
	
	void Update () {
        // checking if player passes a platform
	    if (_player.position.x > transform.position.x + platformWidth)
	    {
	        //Destroy(gameObject);
            gameObject.SetActive(false);
	    }

	}
}
