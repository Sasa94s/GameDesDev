using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can load new scenes

public class CharacterController2D : MonoBehaviour {
    
    // player animation flash
    [Range(0.2f,1.0f)] // create a slider in the editor and set limits on transparency
    public float[] AngryAlpha;
    public float AngryAlphaDelay = 0.25f;
    [Range(0.2f, 1.0f)] // create a slider in the editor and set limits on transparency
    public float[] InvisibleAlpha;
    public float InvisibleAlphaDelay = 0.1f;

    // player controls
    [Range(0.0f, 10.0f)] // create a slider in the editor and set limits on moveSpeed
	public float moveSpeed = 3f;

	public float jumpForce = 350f;

    public GameObject projectile;

    [Range(3.0f, 8.0f)] // create a slider in the editor and set limits on fireSpeed
    public float fireSpeed = 5.0f;
    [Range(0.2f,1.0f)]
    public float fireCooldown = 0.4f;

    // player health
    public int playerHealth = 1;

	// LayerMask to determine what is considered ground for the player
	public LayerMask whatIsGround;

	// Transform just below feet for checking if player is grounded
	public Transform groundCheck;

    // Transform above head and next to both hands for checking if they touch ground
    public Transform[] airSensors;

    private int _airSensorsIndex = 0;

    public string angryLayer = "AngryLayer"; // name of the layer to put enemy on when player angry
    public float angryTime = 1.5f;

    public string invisibleLayer = "Invisible"; // name of the layer to let enemy blind when player invisible
    public float invisibleTime = 1.5f;

    public string projectileLayer = "AcidShot"; // name of the layer of projectile shots


    // player can move?
    // we want this public so other scripts can access it but we don't want to show in editor as it might confuse designer
    [HideInInspector]
	public bool playerCanMove = true;

    // player can shoot?
    private bool canShoot = true;

	// SFXs
	public AudioClip coinSFX;
	public AudioClip deathSFX;
	public AudioClip fallSFX;
	public AudioClip jumpSFX;
	public AudioClip victorySFX;
    public AudioClip attackSFX;
    public AudioClip shotSFX;


    // private variables below

    // store references to components on the gameObject
    Transform _transform;
	Rigidbody2D _rigidbody;
	Animator _animator;
	AudioSource _audio;
    SpriteRenderer _renderer;

    // hold player motion in this timestep
    float _vx;
	float _vy;

	// player tracking
	bool facingRight = true;
	bool isGrounded = false;
	bool isRunning = false;
    private bool _canDoubleJump = false;
    private bool isAngry;
    private bool isInvisible;

    // store the layer the player is on (setup in Awake)
    int _playerLayer;

	// number of layer that Platforms are on (setup in Awake)
	int _platformLayer;

    // store the layer number the enemy should be moved to when player angry
    int _angryLayer;

    // store the layer number the enemy should be moved to when player invisible
    int _invisibleLayer;


    void Awake () {
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

        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null) // if renderer is missing
            Debug.LogError("Sprite Renderer component missing from this gameobject");

        // determine the player's specified layer
        _playerLayer = this.gameObject.layer;

		// determine the platform's specified layer
		_platformLayer = LayerMask.NameToLayer("Platform");

        // determine the angry's specified layer
        _angryLayer = LayerMask.NameToLayer(angryLayer);

        // determine the invisible's specified layer
        _invisibleLayer = LayerMask.NameToLayer(invisibleLayer);
    }

	// this is where most of the player controller magic happens each game event loop
	void Update()
	{
		// exit update if player cannot move or game is paused
		if (!playerCanMove || (Time.timeScale == 0f))
			return;

		// determine horizontal velocity change based on the horizontal input
		_vx = Input.GetAxisRaw ("Horizontal");

		// Determine if running based on the horizontal movement
		if (_vx != 0) 
		{
			isRunning = true;
		} else {
			isRunning = false;
		}

		// set the running animation state
		_animator.SetBool("Running", isRunning);

		// get the current vertical velocity from the rigidbody component
		_vy = _rigidbody.velocity.y;

		// Check to see if character is grounded by raycasting from the middle of the player
		// down to the groundCheck position and see if collected with gameobjects on the
		// whatIsGround layer
		isGrounded = Physics2D.Linecast(_transform.position, groundCheck.position, whatIsGround);

        // Check to see if character touches ground by head or hands by raycasting
        // to force it returning to idle state
	    while (_airSensorsIndex < airSensors.Length)
	    {
	        if (Physics2D.Linecast(_transform.position, airSensors[_airSensorsIndex++].position, whatIsGround))
	        {
				if (!isAngry) {
                    // setting idle animation to let the player get standing immediately
                    _animator.SetTrigger("Respawn");
				}
	            break;
	        }
	        _airSensorsIndex++;
	    }
	    _airSensorsIndex = 0;
        //allow double jump after grounded
        if (isGrounded)
	    {
	        _canDoubleJump = true;
        }

        // Set the grounded animation states
        _animator.SetBool("Grounded", isGrounded);

		if(isGrounded && Input.GetButtonDown("Jump")) // If grounded AND jump button pressed, then allow the player to jump
		{
			DoJump();
		} else if (_canDoubleJump && Input.GetButtonDown("Jump")) //if candoublejump and jump button pressed, then allow player to double jump
		{
            DoJump();
            //disable double jump since you can do it only once
		    _canDoubleJump = false;
            // Set double jump animation
            _animator.SetTrigger("DoubleJump");
		}
	
		// If the player stops jumping mid jump and player is not yet falling
		// then set the vertical velocity to 0 (he will start to fall from gravity)
		if(Input.GetButtonUp("Jump") && _vy>0f)
		{
			_vy = 0f;
		}

        // if left shit pressed and the player is able to shoot, then allow the player to shoot
	    if (Input.GetKeyDown(KeyCode.LeftShift) && canShoot)
	    {
	        StartCoroutine(Shoot());
	    }
        

        // Change the actual velocity on the rigidbody
        _rigidbody.velocity = new Vector2(_vx * moveSpeed, _vy);

		// if moving up then don't collide with platform layer
		// this allows the player to jump up through things on the platform layer
		// NOTE: requires the platforms to be on a layer named "Platform"
		Physics2D.IgnoreLayerCollision(this.gameObject.layer, _platformLayer, (_vy > 0.0f));
	}

	// Checking to see if the sprite should be flipped
	// this is done in LateUpdate since the Animator may override the localScale
	// this code will flip the player even if the animator is controlling scale
	void LateUpdate()
	{
		// get the current scale
		Vector3 localScale = _transform.localScale;

		if (_vx > 0) // moving right so face right
		{
			facingRight = true;
		} else if (_vx < 0) { // moving left so face left
			facingRight = false;
		}

		// check to see if scale x is right for the player
		// if not, multiple by -1 which is an easy way to flip a sprite
		if (((facingRight) && (localScale.x<0)) || ((!facingRight) && (localScale.x>0))) {
			localScale.x *= -1;
		}

		// update the scale
		_transform.localScale = localScale;
        // update the renderer reference
	    _renderer = GetComponent<SpriteRenderer>();
    }

    // if the player collides with a MovingPlatform, then make it a child of that platform
    // so it will go for a ride on the MovingPlatform
    void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	}

	// if the player exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}

    
    void playSound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    //make the player jump
    void DoJump()
    {
        // reset current vertical motion to 0 prior to jump
        _vy = 0f;
        // add a force in the up direction
        _rigidbody.AddForce(new Vector2(0, jumpForce));
        // play the jump sound
        PlaySound(jumpSFX);
    }

	// do what needs to be done to freeze the player
 	void FreezeMotion() {
		playerCanMove = false;
		_rigidbody.isKinematic = true;
	     _rigidbody.simulated = false;
	 }

	// do what needs to be done to unfreeze the player
	void UnFreezeMotion() {
		playerCanMove = true;
		_rigidbody.isKinematic = false;
	}

	// play sound through the audiosource on the gameobject
	void PlaySound(AudioClip clip)
	{
		_audio.PlayOneShot(clip);
	}

	// public function to apply damage to the player
	public void ApplyDamage (int damage) {
		if (playerCanMove) {
			playerHealth -= damage;

			if (playerHealth <= 0) { // player is now dead, so start dying
				PlaySound(deathSFX);
				StartCoroutine (KillPlayer ());
			}
		}
	}

	// public function to kill the player when they have a fall death
	public void FallDeath () {
		if (playerCanMove) {
			playerHealth = 0;
			PlaySound(fallSFX);
			StartCoroutine (KillPlayer ());
		}
	}

	// coroutine to kill the player
	IEnumerator KillPlayer()
	{
		if (playerCanMove)
		{
			// freeze the player
			FreezeMotion();

			// play the death animation
			_animator.SetTrigger("Death");
			
			// After waiting tell the GameManager to reset the game
			yield return new WaitForSeconds(2.0f);

			if (GameManager.gm) // if the gameManager is available, tell it to reset the game
				GameManager.gm.ResetGame();
			else // otherwise, just reload the current level
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	public void CollectCoin(int amount) {
		PlaySound(coinSFX);

		if (GameManager.gm) // add the points through the game manager, if it is available
			GameManager.gm.AddPoints(amount);
	}

	// public function on victory over the level
	public void Victory() {
		PlaySound(victorySFX);
		FreezeMotion ();
		_animator.SetTrigger("Victory");

		if (GameManager.gm) // do the game manager level compete stuff, if it is available
			GameManager.gm.LevelCompete();
	}

    // applying invisible status on player
    public void Invisible()
    {
        if (!isInvisible)
        {
            isInvisible = true;

            UpdateColor(0.4f);

            StartCoroutine(InvisibleFlash(InvisibleAlpha, InvisibleAlphaDelay));

            this.gameObject.layer = _invisibleLayer;
            this.gameObject.tag = "Invisible";

            StartCoroutine(NoInvisible());
        }
    }

    IEnumerator NoInvisible()
    {
        yield return new WaitForSeconds(invisibleTime);

        isInvisible = false;

        this.gameObject.layer = _playerLayer;
        this.gameObject.tag = "Player";
    }

    // applying angry status on player
    public void Angry()
    {
        if (!isAngry)
        {
            isAngry = true;

            StartCoroutine(AngryFlash(AngryAlpha, AngryAlphaDelay));

            // setting angry animation
            _animator.SetBool("Angry", true);
            _animator.SetTrigger("BeAngry");

            // switch layer to angry layer so no collisions while player is angry
            this.gameObject.layer = _angryLayer;
            this.gameObject.tag = "Angry";

            // start coroutine to stand up eventually
            StartCoroutine(NoAngry());
        }
    }

    IEnumerator NoAngry()
    {
        yield return new WaitForSeconds(angryTime);

        // no longer angry
        isAngry = false;

        // switch layer back to regular layer for regular collisions with the player
        this.gameObject.layer = _playerLayer;
        this.gameObject.tag = "Player";

        // disabling angry animation returning to idle state
        _animator.SetBool("Angry", false);
        _animator.SetTrigger("Respawn");
    }

    IEnumerator Shoot()
    {
        playSound(shotSFX);

        canShoot = false;

        // setting up bullet position
        Vector2 firePosition;
        if (facingRight)
            firePosition = new Vector2(transform.position.x + 0.3f, transform.position.y - 0.15f);
        else
            firePosition = new Vector2(transform.position.x - 0.3f, transform.position.y - 0.15f);

        // performing shot
        GameObject bullet = Instantiate(projectile, firePosition, Quaternion.identity) as GameObject;
        // setting speed
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(_transform.localScale.x * fireSpeed, 0);

        yield return new WaitForSeconds(fireCooldown);

        canShoot = true;
    }


    // public function to respawn the player at the appropriate location
    public void Respawn(Vector3 spawnloc) {
		UnFreezeMotion();
		playerHealth = 1;
		_transform.parent = null;
		_transform.position = spawnloc;
		_animator.SetTrigger("Respawn");
    }
    
    public void EnemyBounce()
    {
        DoJump();
    }

    // function to change transparency of the player
    void UpdateColor(float alpha)
    {
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, alpha);
    }

    // changing transparency by modifying sprite renderer's alpha
    // to make a flash animation for angry state animation
    IEnumerator AngryFlash(float[] keyframes, float samples)
    {
        if (isAngry)
        {
            for (int i = 0; i < keyframes.Length; ++i)
            {
                UpdateColor(keyframes[i]);
                yield return new WaitForSeconds(samples);
            }
            yield return new WaitForSeconds(samples*Time.deltaTime);
            StartCoroutine(AngryFlash(keyframes, samples));
        }
        UpdateColor(1.0f);
    }

    // changing transparency by modifying sprite renderer's alpha
    // to make a flash animation for invisible state
    IEnumerator InvisibleFlash(float[] keyframes, float samples)
    {
        if (isInvisible)
        {
            for (int i = 0; i < keyframes.Length; ++i)
            {
                UpdateColor(keyframes[i]);
                yield return new WaitForSeconds(samples);
            }
            yield return new WaitForSeconds(samples * Time.deltaTime);
            StartCoroutine(InvisibleFlash(keyframes, samples));
        }
        UpdateColor(1.0f);
    }
}
