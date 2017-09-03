using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor { // extend the Editor class

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		GameManager myGM = (GameManager)target;

		// add a custom button to the Inspector component
		if(GUILayout.Button("Reset Player State"))
		{
			// if button pressed, then call function in script
			PlayerPrefManager.ResetPlayerState(myGM.startLives, false);
		}

		// add a custom button to the Inspector component
		if(GUILayout.Button("Reset Highscore"))
		{
			// if button pressed, then call function in script
			PlayerPrefManager.SetHighscore(0);
		}

		// add a custom button to the Inspector component
		if(GUILayout.Button("Output Player State"))
		{
			// if button pressed, then call function in script
			PlayerPrefManager.ShowPlayerPrefs();
		}
	}
}
