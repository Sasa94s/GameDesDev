using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // include so we can manipulate SceneManager

public static class PlayerPrefManager {

	public static float GetHighscore() {
		if (PlayerPrefs.HasKey("Highscore")) {
			return PlayerPrefs.GetFloat("Highscore");
		} else {
			return 0;
		}
	}

	public static void SetHighscore(float highscore) {
		PlayerPrefs.SetFloat("Highscore",highscore);
	}


	// story the current player state info into PlayerPrefs
	public static void SavePlayerState(float highScore) {
		// save currentscore and lives to PlayerPrefs for moving to next level
		PlayerPrefs.SetFloat("Highscore",highScore);
	}
	
	// reset stored player state and variables back to defaults
	public static void ResetPlayerState(bool resetHighscore) {
		Debug.Log ("Player State reset.");

        if (resetHighscore)
			PlayerPrefs.SetFloat("Highscore", 0);
	}

	// store a key for the name of the current level to indicate it is unlocked
	public static void UnlockLevel() {
		// get current scene
		Scene scene = SceneManager.GetActiveScene();
		PlayerPrefs.SetInt(scene.name,1);
	}

	// determine if a levelname is currently unlocked (i.e., it has a key set)
	public static bool LevelIsUnlocked(string levelName) {
		return (PlayerPrefs.HasKey(levelName));
	}

	// output the defined Player Prefs to the console
	public static void ShowPlayerPrefs() {
		// store the PlayerPref keys to output to the console
		string[] values = {"Highscore"};

		// loop over the values and output to the console
		foreach(string value in values) {
			if (PlayerPrefs.HasKey(value)) {
				Debug.Log (value+" = "+PlayerPrefs.GetInt(value));
			} else {
				Debug.Log (value+" is not set.");
			}
		}
	}
}
