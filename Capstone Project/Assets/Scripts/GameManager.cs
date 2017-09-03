using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager

public class GameManager : MonoBehaviour {

	// static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager gm;

	// levels to move to on victory and lose
	public string levelAfterVictory;
	public string levelAfterGameOver;


    // game performance

    [HideInInspector]
    public bool scoreIncreasing = false;

    public float score = 0;
    public float pointsPerSecond;
	public float highscore = 0;

    public int coins = 0;

    // player
    public CharacterController2D player;

    // platform
    public Transform platformGenerator;

    // UI elements to control
    public Text UIScore;
	public Text UIHighScore;
	public Text UILevel;
    public Text UICoins;
	public GameObject UIGamePaused;

    /// the time splash gameobject
    public GameObject UITimeSplash;

    /// the game object that contains the heads up display (avatar, health, points...)
    public GameObject UIHUD;

    /// the screen used for all fades
    public Image UIFader;

    public bool paused = false;

	// private variables
    Vector3 _spawnLocation;
    Vector3 platformStartPoint;
    Scene _scene;

    // storage
    private float _savedTimeScale;

    // set things up here
    void Awake () {
        
        // setup reference to game manager
        if (gm == null)
			gm = this.GetComponent<GameManager>();

        // setup all the variables, the UI, and provide errors if things not setup properly.
        GameManager.gm.setupDefaults();
    }

	// game loop
	void Update()
	{
		// if ESC pressed then pause the game
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Pause();
		}

	    if (player.isRunning && player.playerCanMove)
	    {
	        AddPoints(pointsPerSecond * Time.deltaTime);
	        scoreIncreasing = true;
	    }
	    else
	    {
	        scoreIncreasing = false;
	    }
	}

	// setup all the variables, the UI, and provide errors if things not setup properly.
	public void setupDefaults()
	{
	    

		// setup reference to player
		if (player == null)
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
		
		if (player==null)
			Debug.LogError("Player not found in Game Manager");

		// get current scene
		_scene = SceneManager.GetActiveScene();

		// get initial _spawnLocation based on initial position of player
		_spawnLocation = player.transform.position;

        // get initial platform start point
	    platformStartPoint = platformGenerator.position;

		// if levels not specified, default to current level
		if (levelAfterVictory=="") {
			Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
			levelAfterVictory = _scene.name;
		}
		
		if (levelAfterGameOver=="") {
			Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
			levelAfterGameOver = _scene.name;
		}

		// friendly error messages
		if (UIScore==null)
			Debug.LogError ("Need to set UIScore on Game Manager.");
		
		if (UIHighScore==null)
			Debug.LogError ("Need to set UIHighScore on Game Manager.");
		
		if (UILevel==null)
			Debug.LogError ("Need to set UILevel on Game Manager.");
		
		if (UIGamePaused==null)
			Debug.LogError ("Need to set UIGamePaused on Game Manager.");

        if (UICoins == null)
            Debug.LogError("Need to set UICoins on Game Manager.");

        // get stored player prefs
        refreshPlayerState();

		// get the UI ready for the game
		refreshGUI();
	}

	// get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
	void refreshPlayerState() {

		highscore = PlayerPrefManager.GetHighscore();

		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}

	// refresh all the GUI elements
	void refreshGUI() {
        // set the text elements of the UI
        if (UIScore != null)
            UIScore.text = "$" + Mathf.Round(score).ToString();
        if (UIHighScore != null)
            UIHighScore.text = "$" + Mathf.Round(highscore).ToString();
	    if (UILevel != null)
            UILevel.text = _scene.name;
	    if (UICoins != null)
	        UICoins.text = "Coinsx" + coins.ToString();

	}

	// public function to add points and update the gui and highscore player prefs accordingly
	public void AddPoints(float amount)
	{
		// increase score
		score+=amount;

        // update UI

	    if (UIScore != null) UIScore.text = "$" + Mathf.Round(score).ToString();

	    // if score>highscore then update the highscore UI too
		if (score>highscore)
		{
		    highscore = score;
		    if (UIHighScore != null) UIHighScore.text = "$" + Mathf.Round(score).ToString();
		}
	}

    public void AddCoin()
    {
        ++coins;
        UICoins.text = "Coinsx" + coins.ToString();
    }

    // public function to remove player life and reset game accordingly
    public void ResetGame() {
		refreshGUI();

			// save the current player prefs before going to GameOver
			PlayerPrefManager.SavePlayerState(highscore);

			// load the gameOver screen
			SceneManager.LoadScene(levelAfterGameOver);
		// tell the player to respawn
		    platformGenerator.position = platformStartPoint;
            player.Respawn(_spawnLocation);
		
	}


	// load the nextLevel after delay
	IEnumerator LoadLevel() {
		yield return new WaitForSeconds(3.5f);
		SceneManager.LoadScene(levelAfterVictory);
	}

    /// <summary>
    /// Sets the HUD active or inactive
    /// </summary>
    /// <param name="state">If set to <c>true</c> turns the HUD active, turns it off otherwise.</param>
    public void SetHUDActive(bool state)
    {
        UIHUD.SetActive(state);
        UIScore.enabled = state;
        UILevel.enabled = state;
    }

    /// <summary>
    /// Sets the time splash.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, turns the timesplash on.</param>
    public void SetTimeSplash(bool state)
    {
        UITimeSplash.SetActive(state);
    }

 

    /// <summary>
    /// Sets the pause.
    /// </summary>
    /// <param name="state">If set to <c>true</c>, sets the pause.</param>
    public void SetPause(bool state)
    {
        UIGamePaused.SetActive(state);
    }

    /// <summary>
    /// Pauses the game
    /// </summary>
    public void Pause()
    {
        // if time is not already stopped		
        if (Time.timeScale > 0.0f)
        {
            SetTimeScale(0.0f);
            paused = true;
            SetPause(true);
        }
        else
        {
            ResetTimeScale();
            paused = false;
            SetPause(false);
        }
    }

    /// <summary>
    /// sets the timescale to the one in parameters
    /// </summary>
    /// <param name="newTimeScale">New time scale.</param>
    public void SetTimeScale(float newTimeScale)
    {
        _savedTimeScale = Time.timeScale;
        Time.timeScale = newTimeScale;
    }

    /// <summary>
    /// Resets the time scale to the last saved time scale.
    /// </summary>
    public void ResetTimeScale()
    {
        Time.timeScale = _savedTimeScale;
    }
}
