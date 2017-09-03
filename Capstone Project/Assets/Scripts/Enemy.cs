using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	public float moveSpeed = 4f;  // enemy move speed when moving
	public int damageAmount = 10; // probably deal a lot of damage to kill player immediately

	public GameObject stunnedCheck; // what gameobject is the stunnedCheck
    public GameObject MovingEnemy; // what gameobject is its parent

	public float stunnedTime = 3f;   // how long to wait at a waypoint
	
	public string stunnedLayer = "StunnedEnemy";  // name of the layer to put enemy on when stunned
	public string playerLayer = "Player";  // name of the layer to put enemy on when stunned
	
	public bool isStunned = false;  // flag for isStunned
	
	public GameObject[] myWaypoints; // to define the movement waypoints
	
	public float waitAtWaypointTime = 1f;   // how long to wait at a waypoint
	
	public bool loopWaypoints = true; // should it loop through the waypoints

	// SFXs
	public AudioClip stunnedSFX;
	public AudioClip attackSFX;
	public AudioClip deathSFX;

	
	// private variables below
	
	// store references to components on the gameObject
	Transform _transform;
	Rigidbody2D _rigidbody;
	Animator _animator;
	AudioSource _audio;
	
	// movement tracking
	int _myWaypointIndex = 0; // used as index for My_Waypoints
	float _moveTime; 
	float _vx = 0f;
	bool _moving = true;
	
	// store the layer number the enemy is on (setup in Awake)
	public static int _enemyLayer;

	// store the layer number the enemy should be moved to when stunned
	int _stunnedLayer;
    private bool isSmashed;

    void Awake() {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		
		_rigidbody = GetComponent<Rigidbody2D> ();
		if (_rigidbody==null) // if Rigidbody is missing
			Debug.LogError("Rigidbody2D component missing from this gameobject");
		
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");
		
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

		if (stunnedCheck==null) {
			Debug.LogError("stunnedCheck child gameobject needs to be setup on the enemy");
		}

        // setup moving defaults
        _moveTime = 0f;
		_moving = true;
		
		// determine the enemies specified layer
		_enemyLayer = this.gameObject.layer;

		// determine the stunned enemy layer number
		_stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

		// make sure collision are off between the playerLayer and the stunnedLayer
		// which is where the enemy is placed while stunned
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer, true); 
	}
	
	// if not stunned then move the enemy when time is > _moveTime
	void Update () {
		if (!isStunned && !isSmashed)
		{
			if (Time.time >= _moveTime) {
				EnemyMovement();
			} else
			{
				_animator.SetBool("Moving", false);
			}
		}
	}
	
	// Move the enemy through its rigidbody based on its waypoints
	void EnemyMovement() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving)) {
			
			// make sure the enemy is facing the waypoint (based on previous movement)
			Flip (_vx);
			
			// determine distance between waypoint and enemy
			_vx = myWaypoints[_myWaypointIndex].transform.position.x-_transform.position.x;
			
			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vx) <= 0.05f) {
				// At waypoint so stop moving
                Freeze();
                
				// increment to next index in array
				_myWaypointIndex++;
				
				// reset waypoint back to 0 for looping
				if(_myWaypointIndex >= myWaypoints.Length) {
					if (loopWaypoints)
						_myWaypointIndex = 0;
					else
						_moving = false;
				}
				
				// setup wait time at current waypoint
				_moveTime = Time.time + waitAtWaypointTime;
			} else {
				// enemy is moving
				_animator.SetBool("Moving", true);
				// Set the enemy's velocity to moveSpeed in the x direction.
				_rigidbody.velocity = new Vector2(_transform.localScale.x * moveSpeed, _rigidbody.velocity.y);
			}
			
		}
	}
	
	// flip the enemy to face torward the direction he is moving in
	public void Flip(float _vx) {
		
		// get the current scale
		Vector3 localScale = _transform.localScale;
		
		if ((_vx>0f)&&(localScale.x<0f))
			localScale.x*=-1;
		else if ((_vx<0f)&&(localScale.x>0f))
			localScale.x*=-1;
		
		// update the scale
		_transform.localScale = localScale;
	}
	
	// Attack player
	void OnTriggerEnter2D(Collider2D collision)
	{
        if ((collision.tag == "Player") && !isStunned && !isSmashed)
		{
	        CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
			if (player.playerCanMove)
            {
                // Make sure the enemy is facing the player on attack
                Flip(collision.transform.position.x-_transform.position.x);
				
				// attack sound
				playSound(attackSFX);
				
				// stop moving
				Freeze();

				// apply damage to the player
				player.ApplyDamage (damageAmount);
				
				// stop to enjoy killing the player
				_moveTime = Time.time + stunnedTime;
			}
		}
	    
    }

    public void Freeze()
    {
		_rigidbody.velocity = new Vector2(0, 0);
    }

    // if the Enemy collides with a MovingPlatform, then make it a child of that platform
    // so it will go for a ride on the MovingPlatform
    void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	    else if ((other.gameObject.tag == "Angry") && !isSmashed)
	    {
	        isSmashed = true;

	        // Make sure the enemy is facing the player on attack
	        Flip(other.transform.position.x - _transform.position.x);

            // attack sound
            playSound(deathSFX);

            StartCoroutine(KillEnemy());
	    }
	    if (other.gameObject.tag == "Projectile" && !isSmashed)
	    {
//            playSound(deathSFX);
	        _audio.clip = deathSFX;
            _audio.Play();
	        StartCoroutine(KillEnemy());
        }

    }
	
	// if the enemy exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
    }
	
	// play sound through the audiosource on the gameobject
	void playSound(AudioClip clip)
	{
		_audio.PlayOneShot(clip);
	}

    public IEnumerator KillEnemy()
    {
        // make sure collision are off between the (playerLayer or angryLayer) and the enemyLayer
        // which is where the enemy is smashed by animation
        Physics2D.IgnoreLayerCollision(_enemyLayer, LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(_enemyLayer, LayerMask.NameToLayer("AngryLayer"), true);
        
        // setting falling down animation
        _animator.SetTrigger("Smashed");

        yield return new WaitForSeconds(0.7f);

        // destroy object after 0.7 second
        Destroy(MovingEnemy);

        Physics2D.IgnoreLayerCollision(_enemyLayer, LayerMask.NameToLayer("Player"), false);
        Physics2D.IgnoreLayerCollision(_enemyLayer, LayerMask.NameToLayer("AngryLayer"), false);
    }
	// setup the enemy to be stunned
	public void Stunned()
	{
		if (!isStunned && !isSmashed) 
		{
			isStunned = true;
			
			// provide the player with feedback that enemy is stunned
			playSound(stunnedSFX);
			_animator.SetTrigger("Stunned");
			
			// stop moving
            Freeze();
			
			// switch layer to stunned layer so no collisions with the player while stunned
			this.gameObject.layer = _stunnedLayer;
			stunnedCheck.layer = _stunnedLayer;

			// start coroutine to stand up eventually
			StartCoroutine (Stand ());
		}
	}
	
	// coroutine to unstun the enemy and stand back up
	IEnumerator Stand()
	{
		yield return new WaitForSeconds(stunnedTime); 
		
		// no longer stunned
		isStunned = false;
		
		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;
		stunnedCheck.layer = _enemyLayer;
		
		// provide the player with feedback
		_animator.SetTrigger("Stand");
	}
}
