using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI; // include UI namespace so can reference UI elements

/// <summary>
/// Simple start screen class.
/// </summary>
public class StartScreen : MonoBehaviour 
{
	public string FirstLevel;
	
	private float _delayAfterClick=1f;

    /// the game object that contains the heads up display (avatar, health, points...)
    public GameObject UIHUD;
    // UI elements to control
    public Text UIScore;
    public Text UILevel;
    /// the screen used for all fades
    public Image UIFader;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
	{
	    UIHUD.SetActive(false);
	    UIScore.enabled = false;
	    UILevel.enabled = false;
        FaderOn(false,1f);
	}
	
	/// <summary>
	/// During update we simply wait for the user to press the "jump" button.
	/// </summary>
	void Update () 
	{
		if (!CrossPlatformInputManager.GetButtonDown("Jump"))
			return;
		
		FaderOn(true,_delayAfterClick);
		// if the user presses the "Jump" button, we start the first level.
		StartCoroutine(LoadFirstLevel());
	}
	
	IEnumerator LoadFirstLevel()
	{
        yield return new WaitForSeconds(_delayAfterClick);
		Application.LoadLevel(FirstLevel);	
	}

    /// <summary>
    /// Fades the fader in or out depending on the state
    /// </summary>
    /// <param name="state">If set to <c>true</c> fades the fader in, otherwise out if <c>false</c>.</param>
    public void FaderOn(bool state, float duration)
    {
        UIFader.gameObject.SetActive(true);
        if (state)
            StartCoroutine(FadeImage(UIFader, duration, new Color(0, 0, 0, 1f)));
        else
            StartCoroutine(FadeImage(UIFader, duration, new Color(0, 0, 0, 0f)));
    }

    /// <summary>
    /// Fades the specified image to the target opacity and duration.
    /// </summary>
    /// <param name="target">Target.</param>
    /// <param name="opacity">Opacity.</param>
    /// <param name="duration">Duration.</param>
    public static IEnumerator FadeImage(Image target, float duration, Color color)
    {
        if (target == null)
            yield break;

        float alpha = target.color.a;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration)
        {
            if (target == null)
                yield break;
            Color newColor = new Color(color.r, color.g, color.b, Mathf.SmoothStep(alpha, color.a, t));
            target.color = newColor;
            yield return null;
        }
    }
}
