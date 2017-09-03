using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D col)
    {
        Destroy(this.gameObject);
    }
	
}
