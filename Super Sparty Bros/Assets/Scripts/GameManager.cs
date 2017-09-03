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
	public int score = 0;
	public int highscore = 0;
	public int startLives = 3;
	public int lives = 3;

	// UI elements to control
	public Text UIScore;
	public Text UIHighScore;
	public Text UILevel;
	public GameObject[] UIExtraLives;
	public GameObject UIGamePaused;

	// private variables
	GameObject _player;
	Vector3 _spawnLocation;
	Scene _scene;

	// set things up here
	void Awake () {
		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

		// setup all the variables, the UI, and provide errors if things not setup properly.
		setupDefaults();
	}

	// game loop
	void Update() {
		// if ESC pressed then pause the game
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Time.timeScale > 0f) {
				UIGamePaused.SetActive(true); // this brings up the pause UI
				Time.timeScale = 0f; // this pauses the game action
			} else {
				Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
				UIGamePaused.SetActive(false); // remove the pause UI
			}
		}
	}

	// setup all the variables, the UI, and provide errors if things not setup properly.
	void setupDefaults() {
		// setup reference to player
		if (_player == null)
			_player = GameObject.FindGameObjectWithTag("Player");
		
		if (_player==null)
			Debug.LogError("Player not found in Game Manager");

		// get current scene
		_scene = SceneManager.GetActiveScene();

		// get initial _spawnLocation based on initial position of player
		_spawnLocation = _player.transform.position;

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
		
		// get stored player prefs
		refreshPlayerState();

		// get the UI ready for the game
		refreshGUI();
	}

	// get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
	void refreshPlayerState() {
		lives = PlayerPrefManager.GetLives();

		// special case if lives <= 0 then must be testing in editor, so reset the player prefs
		if (lives <= 0) {
			PlayerPrefManager.ResetPlayerState(startLives,false);
			lives = PlayerPrefManager.GetLives();
		}
		score = PlayerPrefManager.GetScore();
		highscore = PlayerPrefManager.GetHighscore();

		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}

	// refresh all the GUI elements
	void refreshGUI() {
		// set the text elements of the UI
		UIScore.text = "Score: "+score.ToString();
		UIHighScore.text = "Highscore: "+highscore.ToString ();
		UILevel.text = _scene.name;
		
		// turn on the appropriate number of life indicators in the UI based on the number of lives left
		for(int i=0;i<UIExtraLives.Length;i++) {
			if (i<(lives-1)) { // show one less than the number of lives since you only typically show lifes after the current life in UI
				UIExtraLives[i].SetActive(true);
			} else {
				UIExtraLives[i].SetActive(false);
			}
		}
	}

	// public function to add points and update the gui and highscore player prefs accordingly
	public void AddPoints(int amount)
	{
		// increase score
		score+=amount;

		// update UI
		UIScore.text = "Score: "+score.ToString();

		// if score>highscore then update the highscore UI too
		if (score>highscore) {
			highscore = score;
			UIHighScore.text = "Highscore: "+score.ToString();
		}
	}

	// public function to remove player life and reset game accordingly
	public void ResetGame() {
		// remove life and update GUI
		lives--;
		refreshGUI();

		if (lives<=0) { // no more lives
			// save the current player prefs before going to GameOver
			PlayerPrefManager.SavePlayerState(score,highscore,lives);

			// load the gameOver screen
			SceneManager.LoadScene(levelAfterGameOver);
		} else { // tell the player to respawn
			_player.GetComponent<CharacterController2D>().Respawn(_spawnLocation);
		}
	}

	// public function for level complete
	public void LevelCompete() {
		// save the current player prefs before moving to the next level
		PlayerPrefManager.SavePlayerState(score,highscore,lives);

		// use a coroutine to allow the player to get fanfare before moving to next level
		StartCoroutine(LoadNextLevel());
	}

	// load the nextLevel after delay
	IEnumerator LoadNextLevel() {
		yield return new WaitForSeconds(3.5f);
		SceneManager.LoadScene(levelAfterVictory);
	}
}
